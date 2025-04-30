using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEvents : MonoBehaviour
{
    [Space(10)]
    [Header("Eventos de cambios de estados")]
    [Space(5)] public UnityEvent IniciarCinematica;
    public UnityEvent IniciarTutorial;
    public UnityEvent TerminarTutorial;

}
