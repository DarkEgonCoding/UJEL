using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] AudioClip sceneMusic;
    [SerializeField] private float startSeconds;
    [SerializeField] private bool doFade;

    private void Start(){
        if (sceneMusic != null){
            AudioManager.instance.PlayMusic(sceneMusic, fade: doFade, startSeconds: startSeconds);
        }
    }
}
