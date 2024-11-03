using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Fader : MonoBehaviour
{
    SpriteRenderer image;
    public static Fader instance;

    private void Awake(){
        image = GetComponent<SpriteRenderer>();
        instance = this;
    }

    public IEnumerator FadeIn(float time){
        yield return image.DOFade(1f, time).WaitForCompletion();
    }
    public IEnumerator FadeOut(float time){
        yield return image.DOFade(0f, time).WaitForCompletion();
    }
}
