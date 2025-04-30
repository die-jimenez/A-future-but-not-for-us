using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageAnimation : MonoBehaviour
{
    [Tooltip("La textura debe tener tener configurado el permitir Read/Write sino produce un error")]
    public float frameRate = 0.1f; // Tiempo entre frames
    [SerializeField] bool isRunningAnimation;
    public bool isTimeFrozen;

    public List<Sprite> framesSprite = new List<Sprite>();
    List<Texture2D> framesTexture = new List<Texture2D>();
    
    RawImage rawImage;
    private int currentFrame;
    private float frameSpeed;




    private void OnEnable()
    {
        if (framesTexture.Count > 0 && !isTimeFrozen && !isRunningAnimation)
        {
            isRunningAnimation = true;
            StartCoroutine(PlayAnimation());
        }

        if (framesTexture.Count > 0 && isTimeFrozen && !isRunningAnimation)
        {
            isRunningAnimation = true;
            StartCoroutine(PlayAnimationFrozenTime());
        }
    }

    private void OnDisable()
    {
        isRunningAnimation = false;
    }

    private void Start()
    {
        frameSpeed = 1f / frameRate;
        rawImage = GetComponent<RawImage>();
        foreach (Sprite sprite in framesSprite)
        {
            framesTexture.Add(SpriteToTexture2D(sprite));
        }

        if (framesTexture.Count > 0 && !isTimeFrozen && !isRunningAnimation)
        {
            isRunningAnimation = true;
            StartCoroutine(PlayAnimation());
        }

        if (framesTexture.Count > 0 && isTimeFrozen && !isRunningAnimation)
        {
            isRunningAnimation = true;
            StartCoroutine(PlayAnimationFrozenTime());
        }
    }

    
    private IEnumerator PlayAnimation()
    {
        while (true)
        {
            rawImage.texture = framesTexture[currentFrame];
            currentFrame = (currentFrame + 1) % framesTexture.Count;
            yield return new WaitForSeconds(frameSpeed);
        }
    }

    private IEnumerator PlayAnimationFrozenTime()
    {
        while (true)
        {
            rawImage.texture = framesTexture[currentFrame];
            currentFrame = (currentFrame + 1) % framesTexture.Count;
            yield return new WaitForSecondsRealtime(frameSpeed);
        }
    }

    Texture2D SpriteToTexture2D(Sprite sprite)
    {
        // Crea una textura del mismo tamaño que el rectángulo del sprite
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);

        // Configura el filtro Point para mantener bordes nítidos y sin suavizado
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp; // Evita repetir bordes

        // Copia los píxeles del sprite a la textura
        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height
        );
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }
}
