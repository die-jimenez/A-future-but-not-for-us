using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    public string textoID;
    public enum InteractionType { Zoom, SendMessage, Sleep}
    public InteractionType interactionType;
    public Vector3 newCameraPos;
    public Vector3 newCameraRot;

    public float moveSpeed;


    public UnityEvent EventToExe; 


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
