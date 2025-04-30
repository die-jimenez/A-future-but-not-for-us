using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class CanvasShake : MonoBehaviour
{
    //Queria montar un sistema de onda para el shake, con ataque y caida pero tengo que avanzar con otras cosas
    //EL ATAQUE ESTA FUNCIONANDO, la caida no.

    [SerializeField] private float shakeStrong;
    [SerializeField, Range(0f, 0.5f)] private float attack;
    [SerializeField, Range(0f, 0.5f)] private float release;
    [SerializeField] private float shakeTime;
    [SerializeField] private KeyCode keyToDebug;

    private RectTransform canvasCamera;

    private float timer;
    private float attackTime;
    private float releaseTime;

    private bool attackEnded;
    private bool releaseStarted;



    private void Awake()
    {
        canvasCamera = GetComponent<RectTransform>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(keyToDebug))
        {
            PlayShake();
        }
#endif

        if (timer > 0)
        {
            Shake();
            timer -= Time.deltaTime;
        }
        else StopShake();
    }


    public void PlayShake()
    {
        timer = shakeTime;
    }

    void Shake()
    {
        if (attack != 0 && !attackEnded)
        {
            if (attackTime >= attack) attackEnded = true;
            attackTime += Time.deltaTime;
        }

        float range = (shakeStrong * 10) * strongModifier();
        float randomPositionX = Random.Range(-range, range);
        float randomPositionY = Random.Range(-range, range);
        canvasCamera.localPosition = new Vector3(randomPositionX, randomPositionY, 0);
    }

    void StopShake()
    {
        timer = 0;
        attackTime = 0;
        releaseTime = 0;
        attackEnded = false;
        releaseStarted = false;
        canvasCamera.localPosition = Vector3.zero;
    }

    float strongModifier()
    {
        if (!attackEnded && !releaseStarted)
        {
            return Matematicas.Map(attackTime, 0f, attack, 0f, 1f);
        }
        else if (attackEnded && !releaseStarted)
        {
            return 1;
        }
        else if (attackEnded && releaseStarted)
        {
            return 1;
        }
        else return 1;
    }

}
