using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    public enum ObjectType { text, sprite, RawImage }

    [Header("Parameters")]
    public ObjectType objectType;
    public float durationOn;
    public float durationOff;
    public bool isVisible;
    public bool loop;

    RawImage rawImage;
    TextMeshProUGUI textMesh;
    SpriteRenderer spriteRenderer;
    int blinkCount;


    private void OnEnable()
    {
        rawImage = TryGetComponent(out RawImage raw) ? raw : null;
        textMesh = TryGetComponent(out TextMeshProUGUI text) ? text : null;
        spriteRenderer = TryGetComponent(out SpriteRenderer sprite) ? sprite : null;
        CheckReferences();

        if (loop)
        {
            StartCoroutine(BlinkOnLoop());
        }
    }

    void Start()
    {
        
    }


    IEnumerator BlinkOnLoop()
    {
        while (loop)
        {
            if (isVisible)
            {
                isVisible = false;
                SetVisibility(false);
                yield return new WaitForSeconds(durationOff);
            }
            else
            {
                isVisible = true;
                SetVisibility(true);
                yield return new WaitForSeconds(durationOn);
            }
        }
    }

    IEnumerator BlinkOnce()
    {
        isVisible = false;
        SetVisibility(false);
        yield return new WaitForSeconds(durationOff);

        isVisible = true;
        SetVisibility(true);
        yield return new WaitForSeconds(durationOn);
        blinkCount++;
    }

    IEnumerator BlinkMultipleTimes(int times)
    {
        blinkCount = 0;
        for (int i = 0; i < times; i++)
        {
            yield return StartCoroutine(BlinkOnce());
        }
    }

    public void BlinkThreeTimes()
    {
        StartCoroutine(BlinkMultipleTimes(3));
    }



    void SetVisibility(bool value)
    {
        if (objectType == ObjectType.text) textMesh.enabled = value;//Text
        if (objectType == ObjectType.RawImage) rawImage.enabled = value;//RawImage
        if (objectType == ObjectType.sprite) spriteRenderer.enabled = value;//Sprite
        isVisible = value;
    }

    public IEnumerator BlinkAsEvent(int blinks, float times, SpriteRenderer _spriteRenderer)
    {
        if (times <= 0)
        {
            Debug.LogWarning(transform.name + " no hizo blinks porque la duración dada a estos fue '< 0'");
            yield break;
        }
        spriteRenderer = _spriteRenderer;

        //Se multiplica x2 porque cada "blink" contiene el tiempo de apagado y prendido
        float intervals = (float)times / ((blinks * 2));
        durationOff = intervals;
        durationOn = intervals;
        enabled = true;
        SetVisibility(false);
        yield return new WaitForSeconds(times);
        enabled = false;
        SetVisibility(true);
    }

    public IEnumerator BlinkAsEvent(int blinks, float times, RawImage _rawImage)
    {
        if (times <= 0)
        {
            Debug.LogWarning(transform.name + " no hizo blinks porque la duración dada a estos fue '< 0'");
            yield break;
        }
        rawImage = _rawImage;

        //Se multiplica x2 porque cada "blink" contiene el tiempo de apagado y prendido
        float intervals = (float)times / ((blinks * 2));
        durationOff = intervals;
        durationOn = intervals;
        enabled = true;
        SetVisibility(false);
        yield return new WaitForSeconds(times);
        enabled = false;
        SetVisibility(true);
    }

    public IEnumerator BlinkAsEvent(int blinks, float times, TextMeshProUGUI _textMesh)
    {
        if (times <= 0)
        {
            Debug.LogWarning(transform.name + " no hizo blinks porque la duración dada a estos fue '< 0'");
            yield break;
        }
        textMesh = _textMesh;

        //Se multiplica x2 porque cada "blink" contiene el tiempo de apagado y prendido
        float intervals = (float)times / ((blinks * 2));
        durationOff = intervals;
        durationOn = intervals;
        enabled = true;
        SetVisibility(false);
        yield return new WaitForSeconds(times);
        enabled = false;
        SetVisibility(true);
    }

    void CheckReferences()
    {
        //Text
        if (objectType == ObjectType.text)
        {
            if (textMesh == null)
            {
                Debug.LogError(gameObject.name + " debería tener un TextMeshPro o seleccionaste mal el -objectType-");
                return;
            }
        }

        //Sprite
        if (objectType == ObjectType.sprite)
        {
            if (spriteRenderer == null)
            {
                Debug.LogError(gameObject.name + " debería tener un SpriteRenderer o seleccionaste mal el -objectType-");
                return;
            }
        }

        //RawImage
        if (objectType == ObjectType.RawImage)
        {
            if (rawImage == null)
            {
                Debug.LogError(gameObject.name + " debería tener un RawImage o seleccionaste mal el -objectType-");
                return;
            }
        }
    }

}



