using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMusicController : MonoBehaviour
{
    public static GameplayMusicController instance;
    [SerializeField] AudioSource[] musicSource = new AudioSource[5];


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);

    }

    void Start()
    {

        musicSource[0]?.Play();
        musicSource[1]?.Play();
        musicSource[2]?.Play();
        musicSource[3]?.Play();
        musicSource[4]?.Play();
    }

    public void CrossFadeToSecond()
    {
        if (musicSource[1] != null)
            musicSource[1].volume = 0f;

        MusicFunction.FadeOut(musicSource[0], 0.5f);
        MusicFunction.FadeIn(musicSource[1], 1, 0.5f);
    }
    public void CrossFadeToThird()
    {
        if (musicSource[2] != null)
            musicSource[2].volume = 0f;

        MusicFunction.FadeOut(musicSource[1], 0.5f);
        MusicFunction.FadeIn(musicSource[2], 1, 0.5f);
    }
    public void CrossFadeToFourth()
    {
        if (musicSource[3] != null)
            musicSource[3].volume = 0f;

        MusicFunction.FadeOut(musicSource[2], 0.5f);
        MusicFunction.FadeIn(musicSource[3], 1, 0.5f);
    }
    public void CrossFadeToFifth()
    {
        if (musicSource[4] != null)
            musicSource[4].volume = 0f;

        MusicFunction.FadeOut(musicSource[3], 4f);
        MusicFunction.FadeIn(musicSource[4], 1, 4);
    }
}
