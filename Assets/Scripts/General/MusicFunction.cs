using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public static class MusicFunction 
{
    public static void FadeIn(AudioSource _audioSource, float maxVolume, float duracion)
    {
        //Dot Tween ya tiene la funcion DoFade que modifica el volumen
        _audioSource.DOFade(maxVolume, duracion);
    }

    public static void FadeOut(AudioSource _audioSource, float duracion)
    {
        //Dot Tween ya tiene la funcion DoFade que modifica el volumen
        _audioSource.DOFade(0, duracion);
    }

    public static void CrossFade(AudioSource _audioSourceOut, AudioSource _audioSourceIn, float duration)
    {
        FadeOut(_audioSourceOut, duration);
        FadeIn(_audioSourceIn, 1, duration);
    }

}
