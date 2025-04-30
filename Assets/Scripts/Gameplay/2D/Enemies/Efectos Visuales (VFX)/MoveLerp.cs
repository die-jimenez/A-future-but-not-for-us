using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLerp : MonoBehaviour
{
    public enum MoveType { linear, ciruclar, instant };
    [SerializeField] MoveType moveType;
    [SerializeField] Transform target;
    [SerializeField] float followSpeed;



    void Start()
    {

    }



    private void LateUpdate()
    {
        if (target != null)
        {
            switch (moveType)
            {
                case MoveType.linear:
                    Vector3 newPosition = Vector3.Lerp(transform.position, target.position, followSpeed * Time.fixedDeltaTime);
                    transform.position = newPosition;
                    break;

                case MoveType.ciruclar:
                    //Codigo sacado de la documentacion

                    // The center of the arc
                    Vector3 center = (transform.position + target.position) * 0.5F;

                    // move the center a bit downwards to make the arc vertical
                    center -= new Vector3(0, 1.5f, 0);

                    // Interpolate over the arc relative to center
                    Vector3 relativeSelfPosition = transform.position - center;
                    Vector3 relativeTargetPosition = target.position - center;

                    // The fraction of the animation that has happened so far is
                    // equal to the elapsed time divided by the desired time for
                    // the total journey.
                    //float fracComplete = (Time.time - startTime) / journeyTime;

                    transform.position = Vector3.Slerp(relativeSelfPosition, relativeTargetPosition, followSpeed * Time.deltaTime);
                    transform.position += center;
                    break;

                case MoveType.instant:
                    transform.position = target.position;
                    break;
            }
        }

    }

    

}
