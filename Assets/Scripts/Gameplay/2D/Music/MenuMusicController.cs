using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    [SerializeField] AudioSource[] musicSource = new AudioSource[2];
    public AudioClip menuClip1;
    public AudioClip menuClip2;


    void Start()
    {
        musicSource[0]?.Play();
        musicSource[1]?.Play();
    }

    public void CrossFadeClips()
    {
        if (musicSource[1] != null)
        {
            musicSource[1].volume = 0f;
        }
        MusicFunction.FadeOut(musicSource[0], 1.5f);
        MusicFunction.FadeIn(musicSource[1], 1, 2);
    }

    public void ReverseCrossFadeClips()
    {
        if (musicSource[0] != null)
        {
            musicSource[0].volume = 0f;
        }
        MusicFunction.FadeOut(musicSource[1], 1.5f);
        MusicFunction.FadeIn(musicSource[0], 1, 2);
    }


}
