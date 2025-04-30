using UnityEngine;
using UnityEngine.EventSystems;

public class Arm : MonoBehaviour
{
    [SerializeField] Move2D moveController;
    [SerializeField] private GameObject body;
    [SerializeField] private float angle;
    private Vector2 target;
    private bool isFacingRight;
    public Vector3 starterPos;
    public float forcedAngle;




    void Start()
    {
        isFacingRight = moveController.GetIsFacingRight();
        if (body == null) Debug.LogError("No se asigno un -body- al brazo");
        starterPos = transform.localPosition;
    }

    private void FixedUpdate()
    {
        RotateAroundBody();
    }

    void RotateAroundBody()
    {
        //Rotación del Brazo
        target = moveController.GetMoveDirection() + new Vector2(body.transform.position.x, body.transform.position.y);
        angle = Matematicas.RadianesEntre(transform.position, target) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + forcedAngle);
        //Correción de dirección
        FlipOnY(moveController.GetMoveDirection());
        transform.localPosition = body.transform.localPosition + starterPos;
    }

    void FlipOnY(Vector2 bodyMoveDirection)
    {
        if (bodyMoveDirection.x > 0 && !isFacingRight || bodyMoveDirection.x < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 currentScale = transform.localScale;
            currentScale.y = currentScale.y * -1;
            transform.localScale = currentScale;
        }
    }
}
