using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CronometroUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cronometroText;
    private float tiempoActual;
    private bool cronometroActivo;

    void Start()
    {
        tiempoActual = 0;
        cronometroActivo = true;
    }

    void Update()
    {
        if (cronometroActivo)
        {
            tiempoActual += Time.deltaTime;
            int minutos = Mathf.FloorToInt(tiempoActual / 60F);
            int segundos = Mathf.FloorToInt(tiempoActual % 60F);

            // Actualiza el texto con formato "00:00"
            cronometroText.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    public void ReiniciarCronometro()
    {
        tiempoActual = 0;
        cronometroText.text = "00:00";
    }

    public void DetenerCronometro()
    {
        cronometroActivo = false;
    }

    public void IniciarCronometro()
    {
        cronometroActivo = true;
    }
}
