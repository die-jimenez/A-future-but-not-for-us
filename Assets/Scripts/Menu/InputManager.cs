using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static InputAction inputActions;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
        inputActions = GetComponent<InputAction>();
    }

    public InputAction GetInputPlayer()
    {
        if (inputActions == null)
        {
            inputActions = GetComponent<InputAction>();
            return inputActions;
        }
        else return inputActions;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
