using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class WorldSpaceResponsiveUI : MonoBehaviour
{
    public Camera mainCamera;
    public Vector2 referenceResolution = new Vector2(1920, 1080); // Resolución de diseño
    public float pixelsPerUnit = 100; // Ajusta este valor para controlar el tamaño

    private RectTransform _canvasRect;

    void Start()
    {
        _canvasRect = GetComponent<RectTransform>();
        UpdateCanvasSize();
    }

    void Update()
    {
        UpdateCanvasSize();
    }

    void UpdateCanvasSize()
    {
        if (mainCamera == null) return;

        // 1. Calcular el tamaño del viewport de la cámara en unidades del mundo
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // 2. Calcular la relación de aspecto de la pantalla actual vs. referencia
        float screenAspect = (float)Screen.width / Screen.height;
        float referenceAspect = referenceResolution.x / referenceResolution.y;

        // 3. Ajustar el tamaño del Canvas para mantener el aspect ratio
        if (screenAspect > referenceAspect)
        {
            // Pantalla más ancha que la referencia
            _canvasRect.sizeDelta = new Vector2(
                cameraHeight * referenceAspect * pixelsPerUnit,
                cameraHeight * pixelsPerUnit
            );
        }
        else
        {
            // Pantalla más estrecha que la referencia
            _canvasRect.sizeDelta = new Vector2(
                cameraWidth * pixelsPerUnit,
                cameraWidth / referenceAspect * pixelsPerUnit
            );
        }

        // 4. Posicionar el Canvas frente a la cámara
        _canvasRect.position = mainCamera.transform.position + mainCamera.transform.forward * 1f;
        _canvasRect.rotation = mainCamera.transform.rotation;
    }
}

