using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move2D))]
public class BodyPartAnimations : MonoBehaviour
{
    public enum AnimationDirection { straight, tiltUp, tiltDown, up, down }
    public enum BasicAnimations { Idle, Run }

    //Inspector
    [SerializeField] AnimationDirection animationDirection;
    [SerializeField] BasicAnimations currentAnimation;
    [SerializeField] Transform body;
    [Space(10)]
    public bool hasArms;
    [HideInInspector] public Transform arms;
    public bool hasLegs;
    [HideInInspector] public Animator legsAnimator;

    [Header("Control con Joystick")]
    [SerializeField] bool usingJoystick;
    [SerializeField] float cronometro;
    [SerializeField] float joystickDelay;

    //Ocultas
    Animator bodyAnimator;
    SpriteRenderer armsRenderer;
    [HideInInspector] public Move2D moveController;




    void Start()
    {
        if (arms != null) armsRenderer = arms.GetComponent<SpriteRenderer>();
        moveController = GetComponent<Move2D>();
        bodyAnimator = GetComponent<Animator>();
    }



    public void Idle()
    {
        SetDirection();
        currentAnimation = BasicAnimations.Idle;
        bodyAnimator.SetBool("isRunning", false);
        if (legsAnimator != null) legsAnimator.SetBool("isRunning", false);
    }

    public void Run()
    {
        if (!usingJoystick)
        {
            SetDirection();
        }
        else SetDirectionWithJoystick();

        switch (animationDirection)
        {
            case AnimationDirection.up:
                bodyAnimator.SetInteger("angle", 90);
                break;
            case AnimationDirection.tiltUp:
                bodyAnimator.SetInteger("angle", 45);
                break;
            case AnimationDirection.straight:
                bodyAnimator.SetInteger("angle", 0);
                break;
            case AnimationDirection.tiltDown:
                bodyAnimator.SetInteger("angle", -45);
                break;
            case AnimationDirection.down:
                bodyAnimator.SetInteger("angle", -90);
                break;
        }
        currentAnimation = BasicAnimations.Run;
        bodyAnimator.SetBool("isRunning", true);
        if (legsAnimator != null) legsAnimator.SetBool("isRunning", true);

    }


    void SetDirectionWithJoystick()
    {
        Vector2 moveDir = moveController.GetMoveDirection();

        if (moveDir == Vector2.up)
            ChangeAnimationJoystick(AnimationDirection.up);
        else if (moveDir.x != 0 && moveDir.y > 0)
            ChangeAnimationJoystick(AnimationDirection.tiltUp);
        else if (moveDir == Vector2.right || moveDir == Vector2.left)
            ChangeAnimationJoystick(AnimationDirection.straight);
        else if (moveDir.x != 0 && moveDir.y < 0)
            ChangeAnimationJoystick(AnimationDirection.tiltDown);
        else if (moveDir == Vector2.down)
            ChangeAnimationJoystick(AnimationDirection.down);
    }

    void ChangeAnimationJoystick(AnimationDirection _dir)
    {
        //cronometro += Time.deltaTime;
        if (animationDirection != _dir)
        {
            if (cronometro >= joystickDelay)
            {
                animationDirection = _dir;
                cronometro = 0;
            }
            cronometro += Time.deltaTime;
        }
    }


    void SetDirection()
    {
        Vector2 moveDir = moveController.GetMoveDirection();

        if (moveDir == Vector2.up)
            animationDirection = AnimationDirection.up;
        else if (moveDir.x != 0 && moveDir.y > 0)
            animationDirection = AnimationDirection.tiltUp;
        else if (moveDir == Vector2.right || moveDir == Vector2.left)
            animationDirection = AnimationDirection.straight;
        else if (moveDir.x != 0 && moveDir.y < 0)
            animationDirection = AnimationDirection.tiltDown;
        else if (moveDir == Vector2.down)
            animationDirection = AnimationDirection.down;
    }

    public AnimationDirection GetAnimationDirection()
    {
        return animationDirection;
    }

    public BasicAnimations GetCurrentAnimation()
    {
        return currentAnimation;
    }
}
