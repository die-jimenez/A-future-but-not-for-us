using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class HackableObject : MonoBehaviour
{
    public enum Type { any, enemy };

    public Type type;
    public bool isBeingHacked;
    public UnityEvent InitHack = new UnityEvent();
    public UnityEvent SuccesHack = new UnityEvent();
    public UnityEvent FailedHack = new UnityEvent();

    [HideInInspector] public Enemy enemy;


    void Start()
    {
        if (type == Type.enemy)
        {
            if (TryGetComponent(out Enemy _enemy)) enemy = _enemy;
            else Debug.LogWarning(transform.name + "... tine marcado en HackableObject que es un enemy, pero no tiene el script --Enemy--");
        }
    }

    void Update()
    {

    }

}
