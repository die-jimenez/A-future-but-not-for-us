using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Move2D), typeof(Animator), typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public enum State { idle, moving };
    public enum MovementType { manual, auto };

    #region Inspector ----------------------------------------------
    [Header("General")]
    [SerializeField] private State state;
    [SerializeField] private Move2D moveController;
    [SerializeField] BodyPartAnimations bodyPartAnimations;


    [Header("Automatismos")]
    public bool automaticHack;
    public bool automaticShoot;
    public bool automaticMove;
    public IAManager iaManager;

    [Header("Arma")]
    public Weapon.type weaponType;
    public Weapon weapon;
    #endregion

    //Componentes
    private Rigidbody2D rb;
    private Animator animator;



    private void Awake()
    {
        state = State.idle;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moveController = GetComponent<Move2D>();
    }

    void Start()
    {
        weaponType = Level2DManager.instance.playerWeaponType;
        moveController.SetMoveDirection(Vector2.right);
        moveController.SetMoveDirection(Vector2.zero);
        SetReferenceToIAManager();
        ReferencesDebug();
    }


    void Update()
    {
        //Disparo
        //---------------------------------------------------------------------
        if (!automaticShoot)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                weapon.Shoot.Invoke();
            }
        }
        else if (automaticShoot && !automaticMove)
        {
            if (iaManager.autoShoot.isAnyEnemyInFront() && weapon.isLoaded())
            {
                weapon.Shoot.Invoke();
            }
        }


        //Input de Movimiento
        //---------------------------------------------------------------------
        if (!automaticMove)
        {
            ManualInput();
            //El cambio "state" se da por el input
            state = InputDirection() == Vector2.zero ? State.idle : State.moving;
        }
        if (automaticMove && !automaticShoot)
        {
            PeacefulAutoMove();
            //El cambio "state" se da por ela direccion dada por la IA
            state = moveController.GetMoveDirection() == Vector2.zero ? State.idle : State.moving;
        }
        else if (automaticMove && automaticShoot)
        {
            AutoMoveAndShot();
            //El cambio "state" se da por ela direccion dada por la IA
            state = moveController.GetMoveDirection() == Vector2.zero ? State.idle : State.moving;
        }


        //Input de Hackeo
        //---------------------------------------------------------------------
        if (automaticHack)
        {
            iaManager.autoHack.AutoHack();
        }
    }


    private void FixedUpdate()
    {
        if (state == State.idle)
        {
            rb.velocity = Vector2.zero;
            bodyPartAnimations.Idle();
        }
        if (state == State.moving)
        {
            moveController.Move(true);
            bodyPartAnimations.Run();
        }
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }


    #region Start Methods
    void ReferencesDebug()
    {
        if (weapon == null) Debug.LogWarning(transform.name + " no tiene referencia de su arma");
        if (iaManager == null) Debug.LogWarning(transform.name + " no tiene referencia de su IA");
        if (bodyPartAnimations == null) Debug.LogWarning(transform.name + " no tiene referencia de su animationController;");
    }

    void SetReferenceToIAManager()
    {
        if (iaManager == null) return;
        iaManager.SetBody(transform);
        iaManager.SetBodyRigidBody(rb);
        iaManager.SetWeaponReference(weapon);
        iaManager.Attack.AddListener(() => weapon.Shoot.Invoke());
    }
    #endregion

    #region Movimiento
    Vector2 InputDirection()
    {
        //Horizotnal_2 y Vertical_2 es un input personalizado igual a horizontal pero sacando las flechas
        return new Vector2(Input.GetAxisRaw("Horizontal_2"), Input.GetAxisRaw("Vertical_2"));
    }

    void ManualInput()
    {
        if (InputDirection() != Vector2.zero)
        {
            moveController.SetMoveDirection(InputDirection());
        }
    }

    void PeacefulAutoMove()
    {
        switch (iaManager.autoMove.GetMoveAction())
        {
            case IAMove.MoveAction.RunAway:
                iaManager.autoMove.RunAway();
                break;

            case IAMove.MoveAction.KeepDirection:
                break;
        }
        if (iaManager.autoMove.GetMoveAction() == IAMove.MoveAction.Chase) iaManager.autoMove.SetMoveAction(IAMove.MoveAction.RunAway);
    }

    void AutoMoveAndShot()
    {
        switch (iaManager.autoMove.GetMoveAction())
        {
            case IAMove.MoveAction.RunAway:
                iaManager.autoMove.RunAway();
                if (iaManager.autoMove.isReadyToChase())
                {
                    if (iaManager.autoMove.isSurrounded())
                    {
                        weapon.Shoot.Invoke();
                    }
                    else iaManager.autoMove.SelectTarget();
                }
                break;

            case IAMove.MoveAction.Chase:
                iaManager.autoMove.Chase();
                if (iaManager.autoMove.isReadyToAim())
                {
                    StartCoroutine(iaManager.autoMove.AimAndAttack());
                }
                break;

            case IAMove.MoveAction.Surround:
                iaManager.autoMove.Surround();
                break;

            case IAMove.MoveAction.KeepDirection:
                break;
        }
    }
    #endregion
}
