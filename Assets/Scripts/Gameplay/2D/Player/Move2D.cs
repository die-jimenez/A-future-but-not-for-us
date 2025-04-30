using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Move2D : MonoBehaviour
{

    [SerializeField, Range(50f, 500f)] private float movementSpeed;
    public float speedMultiplier = 1;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField, Space(3)] private bool isFacingRight;
    private Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        speedMultiplier = speedMultiplier == 0? 1 : speedMultiplier;
    }


    public void Move(bool _tieneOrientacion)
    {
        rb.velocity = moveDirection * (movementSpeed * speedMultiplier * Time.fixedDeltaTime);
        if (_tieneOrientacion) Flip();
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }
    public void SetMoveDirection(Vector2 newDirection)
    {
        moveDirection = newDirection.normalized;
    }
    public float GetMovementSpeed()
    {
        return movementSpeed;
    }
    public void SetMovementSpeed(float newSpeed)
    {
        movementSpeed = newSpeed;
    }

    void Flip()
    {
        if (moveDirection.x > 0 && !isFacingRight || moveDirection.x < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 currentScale = transform.localScale;
            currentScale.x = currentScale.x * -1;
            transform.localScale = currentScale;
        }
    }

    public void StopMoving()
    {
        speedMultiplier = 0;
    }

    public void ResumeMoving()
    {
        speedMultiplier = 1;
    }

    public bool GetIsFacingRight()
    {
        return isFacingRight;
    }

}
