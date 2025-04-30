using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextBubble : MonoBehaviour
{
    [SerializeField] Transform mainCamera;
    [SerializeField] Transform canvas;

    Vector3 relativeCameraOffset;
    [SerializeField] Vector3 angleOffset;

    void Start()
    {
        GetRelativeRotation(mainCamera);
    }

    void Update()
    {
        StartCoroutine(RotateTo(mainCamera));
        //SimpleRotation(mainCamera);
    }



    void GetRelativeRotation(Transform target)
    {
        transform.rotation = Quaternion.LookRotation(target.position - canvas.position);
        relativeCameraOffset = transform.rotation.eulerAngles;
    }

    IEnumerator RotateTo(Transform target)
    {
        yield return new WaitForSeconds(0.1f);
        transform.rotation = Quaternion.LookRotation(target.position - canvas.position);
        Vector3 angle = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(angle.x - relativeCameraOffset.x, angle.y + angleOffset.y, angle.z + angleOffset.z);
    }

    void SimpleRotation(Transform target)
    {
        Vector3 targetDir = transform.position - target.position;
        float angle = Vector3.Angle(targetDir, target.forward);


        Vector3 currentAngle = transform.rotation.eulerAngles;
        //float dotAngle = Matematicas.Map(dot, -1, 1, 0, 360);
        //transform.rotation = Quaternion.Euler(currentAngle.x, dotAngle+90, currentAngle.z);
        Debug.Log(angle);
    }

}
