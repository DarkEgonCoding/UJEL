using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] public float fadeDuration = 0.75f;

    float originalMusicVol;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null){
            instance = this;
            originalMusicVol = musicPlayer.volume;
        }
        else {
            instance.originalMusicVol = musicPlayer.volume;
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false, float startSeconds = 0){
        if (clip == null) return;
        
        StartCoroutine(PlayMusicAsync(clip, loop, fade, startSeconds));
    }

    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade, float startSeconds){
        if (fade){
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }

        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.time = startSeconds;
        musicPlayer.Play();

        yield return musicPlayer.DOFade(originalMusicVol, fadeDuration).WaitForCompletion();
    }
}
