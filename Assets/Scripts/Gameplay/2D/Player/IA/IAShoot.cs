using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(IAManager))]
public class IAShoot : MonoBehaviour
{
    [SerializeField] IAManager controller;
    [SerializeField] private Transform arm;
    [SerializeField] private Weapon weapon;
    [SerializeField] private LineRenderer laser;
    [SerializeField, Range(1f, 10f)] float maxDistance;
    [SerializeField] LayerMask enemiesLayer;

    private Vector2 weaponDirection;
    private RaycastHit2D[] weaponRayCast = new RaycastHit2D[1];

    void Start()
    {
        if (weapon == null) Debug.LogWarning("La IA de disparo la referencia de un arma");
        controller = controller == null ? GetComponent<IAManager>() : controller;
    }

    private void Update()
    {
        weaponDirection = Matematicas.DireccionEntre(arm.position, weapon.transform.position);
        Debug.DrawRay(weapon.transform.position, weaponDirection * maxDistance);

        if (laser.gameObject.activeSelf)
        {
            SetPositionToLaser();
        }

    }


    public bool isAnyEnemyInFront()
    {
        //weaponDirection = Matematicas.DireccionEntre(transform.position, weapon.transform.position);
        //Debug.DrawRay(weapon.transform.position, weaponDirection * maxDistance);
        if (Physics2D.RaycastNonAlloc(weapon.transform.position, weaponDirection, weaponRayCast, maxDistance, enemiesLayer) != 0)
        {
            return true;
        }
        else return false;
    }

    public bool isTargetEnemyInFront(Transform target)
    {
        if (target == null) return false;

        weaponDirection = Matematicas.DireccionEntre(transform.position, weapon.transform.position);
        Debug.DrawRay(weapon.transform.position, weaponDirection * maxDistance);

        if (Physics2D.RaycastNonAlloc(weapon.transform.position, weaponDirection, weaponRayCast, maxDistance, enemiesLayer) != 0)
        {
            if (weaponRayCast[0].collider.transform == target) return true;
            else return false;
        }
        else return false;
    }

    public void SetPositionToLaser()
    {
        Vector3 start = weapon.transform.position;
        Vector3 end = weapon.transform.position + new Vector3(weaponDirection.x, weaponDirection.y, weapon.transform.localPosition.z) * maxDistance;

        //Los puntos de un LineRenderer se cambian en arreglos
        Vector3[] vertexs = new Vector3[] { start,end };
        SetLineRendererPoints(laser, vertexs);
    }

    public void SetLineRendererPoints(LineRenderer lineRenderer, Vector3[] points)
    {
        lineRenderer.positionCount = points.Length; // Definir el número de puntos
        lineRenderer.SetPositions(points);
    }

    public void TurnOnLaser()
    {
        laser.gameObject.SetActive(true);
    }
}
