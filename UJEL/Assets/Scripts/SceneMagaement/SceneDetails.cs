using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] AudioClip sceneMusic;

    private void Start(){
        if (sceneMusic != null){
            AudioManager.instance.PlayMusic(sceneMusic, fade: true);
        }
    }
}
