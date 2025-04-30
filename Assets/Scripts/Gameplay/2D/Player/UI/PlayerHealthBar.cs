using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textoVida;
    [SerializeField] List<GameObject> healthPoints = new List<GameObject>();
    List<RawImage> healthPointsImage = new List<RawImage>();
    RawImage barRawImage;


    void Start()
    {
        if (healthPoints.Count == 0)
        {
            Debug.LogWarning(transform.name + " no tiene referenciado ningun punto de vida");
            return;
        }

        foreach (GameObject healthPoint in healthPoints)
        {
            if (healthPoint.TryGetComponent(out RawImage image))
            {
                healthPointsImage.Add(image);
            }
        }


        if (TryGetComponent(out RawImage _image))
        {
            barRawImage = _image;
        }
    }

    public void UpdateSprite(int currentHealth)
    {
        for (int i = 0; i < healthPoints.Count; i++)
        {
            if (currentHealth - 1 >= i)
            {
                HealthPoint(i);
            }
            else
            {
                LosePoint(i);
            }
        }

    }

    void LosePoint(int index)
    {
        if (healthPoints[index].activeSelf)
        {
            healthPoints[index].SetActive(false);
        }
    }

    public void RestorePoint(int index)
    {
        healthPoints[index].SetActive(true);
    }

    void HealthPoint(int index)
    {
        if (!healthPoints[index].activeSelf)
        {
            healthPoints[index].SetActive(true);
        }
    }

    public void ChangeColorHealthPoints(Color _color)
    {
        barRawImage.color = _color;
        if (textoVida != null) textoVida.color = _color;
        foreach (RawImage point in healthPointsImage)
        {
            point.color = _color;
        }
    }
}
