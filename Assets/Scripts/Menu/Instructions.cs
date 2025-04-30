using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AudioSource))]
public class Instructions : MonoBehaviour
{

    [SerializeField] private RawImage instrucciones;
    [SerializeField] AudioClip sonido;





    void Start()
    {
        RawImage rawImage = GetComponent<RawImage>();

        GetComponent<AudioSource>().PlayOneShot(sonido);
        instrucciones.gameObject.SetActive(true);
        DOVirtual.DelayedCall(5f, () => instrucciones.DOColor(Color.clear, 2f));
        DOVirtual.DelayedCall(7.5f, () => GameManager.instance.GoToMainMenuScene());
    }

    void Update()
    {

    }
}
