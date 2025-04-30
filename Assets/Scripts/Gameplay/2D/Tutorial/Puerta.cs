using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Color beingHackedColor;
    public Color succesHackColor;
    private float initPosY;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initPosY = transform.position.y;
    }

    void Update()
    {
        
    }

    public void InitHack()
    {
        spriteRenderer.color = beingHackedColor;
    }

    public void OpenDoor()
    {
        spriteRenderer.color = succesHackColor;
        transform.DOLocalMoveY(initPosY + transform.localScale.y, 1.5f).OnComplete(()=> { gameObject.SetActive(false); });
    }
}
