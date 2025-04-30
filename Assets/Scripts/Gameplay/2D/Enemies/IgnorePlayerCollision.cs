using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IgnorePlayerCollision : MonoBehaviour
{
    private void OnEnable()
    {
        if (Level2DManager.instance.player2D != null)
        {
            Physics2D.IgnoreCollision(Level2DManager.instance.player2D.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    void Start()
    {
        //Hace que este collider no colisione con el player, como si fuera por pero mas ordenado.
        //Hay otro collider como hijo que es un trigger que si reacciona con el player. La idea era no crear dos layers.
        //Pues había el probelma de que para lso otros robots debia ser rigido, y para el player debia ser un trigger.
        Physics2D.IgnoreCollision(Level2DManager.instance.player2D.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    
}
