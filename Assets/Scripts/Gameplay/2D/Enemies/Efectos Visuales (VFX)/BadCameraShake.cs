using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BadCameraShake : MonoBehaviour
{
    //Codigo basado en: https://www.youtube.com/watch?v=luSVUPU6bK0

    //[SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float shakeTime;
    [SerializeField] private KeyCode keyToDebug;

    private float timer;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin channelPerlin;


    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        channelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StopShake();
    }

    public void PlayShake()
    {
        if (channelPerlin != null)
        {
            channelPerlin.m_AmplitudeGain = shakeAmplitude;
            timer = shakeTime;
        }
    }

    void StopShake()
    {
        if (channelPerlin != null)
        {
            channelPerlin.m_AmplitudeGain = 0;
            timer = 0;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(keyToDebug))
        {
            PlayShake();
        }
#endif
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) StopShake();
        }
    }
}
