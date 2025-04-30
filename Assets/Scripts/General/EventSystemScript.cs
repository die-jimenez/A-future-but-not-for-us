using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemScript : MonoBehaviour
{
    //Este script es necesario para evitar errores al tener dos event system cal trabajar multi scenes
    public static EventSystemScript instance { get; set; }

    void Start()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }
}
