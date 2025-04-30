using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraGame2D : MonoBehaviour
{
    Camera thisCamera;
    [SerializeField] RenderTexture texturaPantalla3D;


    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadFinalScene.AddListener(() => StartCoroutine(SetCameraAsTexture()));
        }
    }

    void Update()
    {

    }

    IEnumerator SetCameraAsTexture()
    {
        if (texturaPantalla3D == null) Debug.LogWarning("A la main Camera del juego 2D le falta la textura que usara cuando se cargue la escena 3D");
        yield return new WaitForSeconds(0.5f);
        thisCamera.targetTexture = texturaPantalla3D;
    }
}
