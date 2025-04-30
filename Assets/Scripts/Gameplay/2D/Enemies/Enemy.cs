using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;



[RequireComponent(typeof(Move2D), typeof(HackableObject))]
public class Enemy : MonoBehaviour
{
    //-------------------------------------------------------------------------------------
    private enum State { idle, moving, death };
    public enum SpecialState { none, hacked };

    #region Inspector
    //-------------------------------------------------------------------------------------
    [Header("Estados")]
    [SerializeField] private State state;
    public SpecialState specialState;

    //-------------------------------------------------------------------------------------
    [Header("Propiedades")]
    public EnemySpawn spawner;
    [SerializeField] private int initialHelth;
    [SerializeField] int health;
    public int damage;

    //-------------------------------------------------------------------------------------
    [Header("Movimiento")]
    public Transform target;
    [SerializeField, Range(1f, 5f)] float moveDispersion;

    //-------------------------------------------------------------------------------------
    [Header("Experiencia")]
    [SerializeField] ExpOrb expOrb;
    [SerializeField] Transform expContainer;

    [Header("Componentes hijos")]
    [SerializeField] Collider2D colliderToPlayer;
    [SerializeField] Animator animatorBordes;
    [SerializeField] SpriteRenderer bordes;


    #endregion

    #region Privadas
    //Componentes
    //-------------------------------------------------------------------------------------
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Move2D moveController;
    private HackableObject hackableObject;
    private BoxCollider2D boxCollider;

    //Movimiento
    //-------------------------------------------------------------------------------------
    private Vector3 randomDispersionPos;
    Utilidades.Timer timer_SetRandomDirection;
    #endregion


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moveController = GetComponent<Move2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hackableObject = GetComponent<HackableObject>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        health = 0 == 0 ? 100 : health;
        initialHelth = health;
        CheckMainReference();

        timer_SetRandomDirection = new Utilidades.Timer(5f);

        //Agrega los eventos al ser hackeado
        hackableObject.InitHack.AddListener(SetToHackedState);
        hackableObject.SuccesHack.AddListener(Die);
    }

    private void OnEnable()
    {
        Reset();
        randomDispersionPos = RandomVector(moveDispersion);
        if (initialHelth == 0) Debug.LogWarning("la vida incial de " + name + " no puede ser 0");
    }

    void Update()
    {
        if (target != null)
        {
            CalculateMoveDirection();
            if (isTooFar()) ReSpawnNearOf(target);
        }

        if (health > 0)
        {
            if (moveController.GetMoveDirection() == Vector2.zero)
            {
                state = State.idle;
            }
            else if (moveController.GetMoveDirection() != Vector2.zero)
            {
                state = State.moving;
            }
        }
        else state = State.death;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.idle:
                rb.velocity = Vector2.zero;
                animator.SetBool("isRunning", false);
                animator.SetBool("isStanding", true);
                animatorBordes.SetBool("isRunning", false);
                animatorBordes.SetBool("isStanding", true);
                break;

            case State.moving:
                moveController.Move(true);
                animator.SetBool("isRunning", true);
                animator.SetBool("isStanding", false);
                animatorBordes.SetBool("isRunning", true);
                animatorBordes.SetBool("isStanding", false);
                break;

            case State.death:
                rb.velocity = Vector2.zero;
                animator.SetBool("isDead", true);
                animatorBordes.SetBool("isDead", true);
                StartCoroutine(Death());
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target.position + randomDispersionPos, 0.2f);
    }



    void CheckMainReference()
    {
        if (target == null)
            target = Level2DManager.instance.player2D.transform;

        if (expOrb == null)
            Debug.LogWarning(name + "no tiene referencia su expOrb");
    }


    //----------------------------------------------------------------------------------------------------------
    #region Movimiento 
    void CalculateMoveDirection()
    {
        //Si la distancia con el enemigo es mayor a la distancia minima...
        //hay una posicion random que evita que se apilen lso enemigos sobre una misma direccion
        float minDistance = 25;//Al cuadrado
        if ((transform.position - target.position).sqrMagnitude >= minDistance)
        {
            timer_SetRandomDirection.Play();
            if (timer_SetRandomDirection.finished())
            {
                randomDispersionPos = RandomVector(moveDispersion);
                timer_SetRandomDirection.Reset();
            }
        }
        else randomDispersionPos = Vector3.zero;

        //MoveDirection debe ser normalizada para que la velocidad no cambie si va horizontal o diagonal, por ello se usa Vector2.zero
        float radianEnemyCharacter = Matematicas.RadianesEntre(transform.position, (target.position + randomDispersionPos));
        moveController.SetMoveDirection(Matematicas.PolaresToRectangulares(1, radianEnemyCharacter, Vector2.zero));
        Debug.DrawRay(transform.position, moveController.GetMoveDirection());
    }

    Vector2 RandomVector(float range)
    {
        return new Vector2(Random.Range(-range, range), Random.Range(-range, range));
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    bool isTooFar()
    {
        float maxDistance = 40*40;//Al cuadrado
        if ((transform.position - target.position).sqrMagnitude >= maxDistance)
        {
            return true;
        }
        return false;
    }

    void ReSpawnNearOf(Transform _target)
    {
        SpawnAt(RandomPositionAround(target.position));
    }

    public Vector3 RandomPositionAround(Vector3 _target)
    {
        float ratioX = 32;
        float ratioY = 22;
        float angle = Random.Range(0, 360);
        //Polares a rectangulares pero no uso la formula de "matematicas" porque los valores de x,y tendran diferente radio
        float posX = Mathf.Cos(angle * Mathf.Deg2Rad) * ratioX + _target.x;
        float posY = Mathf.Sin(angle * Mathf.Deg2Rad) * ratioY + _target.y;
        return new Vector3(posX, posY, _target.z);
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------


    //----------------------------------------------------------------------------------------------------------
    #region Ciclo de vida 
    public void SpawnAt(Vector3 postion)
    {
        transform.position = postion;
        gameObject.SetActive(true);
    }

    private void Reset()
    {
        state = State.idle;
        health = initialHelth;
        animator.SetBool("isDead", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isStanding", true);
        hackableObject.isBeingHacked = false;
        moveController.ResumeMoving();
        RemoveSpecialStates();

        bordes.enabled = false;
        boxCollider.enabled = true;
        colliderToPlayer.enabled = true;
        animatorBordes.SetBool("isDead", false);
        animatorBordes.SetBool("isRunning", false);
        animatorBordes.SetBool("isStanding", true);
    }

    public void Die()
    {
        health = 0;
        state = State.death;
        animator.SetBool("isRunning", false);
        animator.SetBool("isStanding", false);

        boxCollider.enabled = false;
        colliderToPlayer.enabled = false;
        animatorBordes.SetBool("isRunning", false);
        animatorBordes.SetBool("isStanding", false);
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(1.5f);
        specialState = SpecialState.none;
        gameObject.SetActive(false);
    }

    public void DropExp()
    {
        //Se ejecuta al final de la animacion de muerte
        if (expOrb != null)
        {
            expOrb.gameObject.SetActive(true);
            expOrb.transform.parent = expContainer != null ? expContainer : null;
        }
        else Debug.LogWarning(transform.name + " no tiene referenciada su exp");
    }

    public void GetBackExpOrb()
    {
        expOrb.target = null;
        expOrb.transform.parent = transform;
        expOrb.transform.localPosition = Vector3.zero;
        expOrb.transform.gameObject.SetActive(false);
        ReturnToSpawnPool();
    }

    void ReturnToSpawnPool()
    {
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.enemiesSpawned.Remove(this);
            spawner.enemiesPool.Add(this);
        }
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------



    //----------------------------------------------------------------------------------------------------------
    #region Estados especiales y efectos
    public IEnumerator Stunned(float duration)
    {
        moveController.StopMoving();
        yield return new WaitForSeconds(duration);
        moveController.ResumeMoving();
    }

    public void SetToHackedState()
    {
        bordes.enabled = true;
        specialState = SpecialState.hacked;
        StartCoroutine(Stunned(0.75f));

        //Sincroniza las animaciones
        animator.Play("Run", -1, 0);
        animatorBordes.Play("Run", -1, 0);

    }

    public void RemoveSpecialStates()
    {
        specialState = SpecialState.none;
        spriteRenderer.color = Color.white;
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------




}
