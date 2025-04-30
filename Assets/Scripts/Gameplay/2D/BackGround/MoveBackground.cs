using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveBackground : MonoBehaviour
{
    [SerializeField] bool automatic;
    [SerializeField, Range(-1f, 1f)] float speedX;
    [SerializeField, Range(-1f, 1f)] float speedY;

    PlayerController2D player;
    RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (automatic)
        {
            rawImage.uvRect = new Rect(rawImage.uvRect.position + new Vector2(speedX, speedY) * Time.deltaTime, rawImage.uvRect.size);
        }
        else
        {
            Vector2 playerPos = player.transform.position * 0.025f;
            rawImage.uvRect = new Rect(playerPos, rawImage.uvRect.size);
        }

    }
}
