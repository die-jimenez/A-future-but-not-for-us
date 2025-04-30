using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpSlider : MonoBehaviour
{
    [SerializeField] PlayerController2D controller;

    void Start()
    {
        if (controller == null)
        {
            controller = FindAnyObjectByType<PlayerController2D>();
            Debug.LogWarning(transform.name + " no tiene asignado el controller del player, asi que buscó uno. " +
                "ESTO NO ES RECOMENDABLE SI HAY MAS DE UN PLAYER");
        }
        if (controller != null)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
