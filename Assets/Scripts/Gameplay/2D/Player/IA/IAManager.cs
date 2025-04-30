using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public class IAManager : MonoBehaviour
{
    #region Class, Enum, Getters & Setters
    public Move2D moveController { get { return _moveController; } private set { _moveController = value; } }
    #endregion


    #region Editor ----------------------------------------------
    [Header("Movimiento")]
    [SerializeField ] private Move2D _moveController; 
    public Transform target;

    [Header("Referencias")]
    public Transform body;
    [SerializeField] private Rigidbody2D bodyRB;
    public Weapon weapon;
    #endregion

    //Eventos 
    [HideInInspector] public UnityEvent Attack = new UnityEvent();

    //Automatismos
    [HideInInspector] public IAHacker autoHack;
    [HideInInspector] public IAShoot autoShoot;
    [HideInInspector] public IAMove autoMove;

    private void Start()
    {
        autoHack = TryGetComponent(out IAHacker hackScript) ? hackScript : null;
        autoShoot = TryGetComponent(out IAShoot shootScript) ? shootScript : null;
        autoMove = TryGetComponent(out IAMove moveScript) ? moveScript : null;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------
    #region Metodos públicos para otros IA scripts
    public Vector2 GetMoveDirection()
    {
        if (moveController == null)
        {
            Debug.Log(transform.name + " no tiene referencia de MoveController");
            return Vector2.zero;
        }
        else return moveController.GetMoveDirection();
    }

    public void SetMoveDirection(Vector2 newDirection)
    {
        if (moveController == null)
        {
            Debug.Log(transform.name + " no tiene referencia de MoveController");
            return;
        }
        moveController.SetMoveDirection(newDirection);
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------------------------------------------------


    //----------------------------------------------------------------------------------------------------------------------------------------------------
    #region Metodos publicos para el controller del objeto
    public void SetBody(Transform _body)
    {
        if (_body != null) body = _body;
        else Debug.LogWarning(transform.name + " recibio una referencia nula");
    }

    public void SetBodyRigidBody(Rigidbody2D rigidbody2D)
    {
        if (rigidbody2D != null) bodyRB = rigidbody2D;
        else Debug.LogWarning(transform.name + " recibio una referencia nula");
    }

    public void SetWeaponReference(Weapon script)
    {
        if (script != null) weapon = script;
        else Debug.LogWarning(transform.name + " recibio una referencia nula");
    }
    #endregion
    //----------------------------------------------------------------------------------------------------------------------------------------------------


}
