using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDialog : MonoBehaviour
{
    [SerializeField] GameObject objectToCollision;
    [Space(5)]
    [Header("Eventos de colision")]
    public UnityEvent triggerEnter;
    public UnityEvent triggerExit;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == objectToCollision)
        {
            triggerEnter.Invoke();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == objectToCollision)
        {
            triggerExit.Invoke();
        }
    }
}
