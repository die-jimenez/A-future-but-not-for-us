using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CreditosMovimiento : MonoBehaviour
{
    [SerializeField] RectTransform creditos;

    // Start is called before the first frame update
    void Start()
    {
        creditos.DOLocalMoveY(2100, 15).SetEase(Ease.Linear).OnComplete(()=> GameManager.instance?.GoToMainMenuScene());
    }

 
}
