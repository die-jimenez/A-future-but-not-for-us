using UnityEngine;



public class ForceArmPosition : MonoBehaviour
{

    [SerializeField] Move2D moveController;
    [SerializeField] BodyPartAnimations bodyPartAnimations;
    [SerializeField] GameObject body;
    private bool isFacingRight;


    [SerializeField] private Vector3 forcedPosition;
    [SerializeField] private float forcedAngle;
    [SerializeField] private float normalAngle;

    [Header("Idle")]
    [SerializeField] private float idleAngle;
    [SerializeField] private Vector3 idlePosition;

    [Header("Up")]
    [SerializeField] private float upAngle;
    [SerializeField] private Vector3 upPosition;

    [Header("TiltUp")]
    [SerializeField] private float tiltUpAngle;
    [SerializeField] private Vector3 tiltUpPosition;

    [Header("StarightUp")]
    [SerializeField] private float straightAngle;
    [SerializeField] private Vector3 straightPosition;

    [Header("TiltDown")]
    [SerializeField] private float tiltDownAngle;
    [SerializeField] private Vector3 tiltDownPosition;

    [Header("Down")]
    [SerializeField] private float downAngle;
    [SerializeField] private Vector3 downPosition;






    void Start()
    {
        isFacingRight = moveController.GetIsFacingRight();
        if (body == null) Debug.LogError("No se asigno un -body- al brazo");
    }

    private void FixedUpdate()
    {
        RotateAroundBody();
    }

    void RotateAroundBody()
    {
        SetForcedAngleAndPosition();
        //Rotación del Brazo
        forcedAngle = isFacingRight ? forcedAngle : forcedAngle * -1;
        transform.rotation = Quaternion.Euler(0, 0, forcedAngle);

        //Correción de dirección
        FlipOnX(moveController.GetMoveDirection());
        forcedPosition.x = isFacingRight ? forcedPosition.x : forcedPosition.x * -1;
        transform.localPosition = body.transform.localPosition + forcedPosition;
    }

    void FlipOnX(Vector2 bodyMoveDirection)
    {
        if (bodyMoveDirection.x > 0 && !isFacingRight || bodyMoveDirection.x < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 currentScale = transform.localScale;
            currentScale.x = currentScale.x * -1;
            transform.localScale = currentScale;
        }
    }


    void SetForcedAngleAndPosition()
    {
        int direccion = bodyPartAnimations.transform.localScale.x > 0 ? 1 : -1;
        if (bodyPartAnimations != null)
        {
            if (bodyPartAnimations.GetCurrentAnimation() == BodyPartAnimations.BasicAnimations.Idle)
            {
                forcedAngle = idleAngle;
                forcedPosition = idlePosition;
                return;
            }

            switch (bodyPartAnimations.GetAnimationDirection())
            {
                case BodyPartAnimations.AnimationDirection.up:
                    forcedAngle = upAngle;
                    forcedPosition = upPosition;
                    break;
                case BodyPartAnimations.AnimationDirection.tiltUp:
                    forcedAngle = tiltUpAngle;
                    forcedPosition = tiltUpPosition;
                    break;
                case BodyPartAnimations.AnimationDirection.straight:
                    forcedAngle = straightAngle;
                    forcedPosition = straightPosition;
                    break;
                case BodyPartAnimations.AnimationDirection.tiltDown:
                    forcedAngle = tiltDownAngle;
                    forcedPosition = tiltDownPosition;
                    break;
                case BodyPartAnimations.AnimationDirection.down:
                    forcedAngle = downAngle;
                    forcedPosition = downPosition;
                    break;
            }
        }
    }
}
