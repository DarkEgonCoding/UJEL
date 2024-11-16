using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] public Material TransitionMaterial;
    [SerializeField] public float AnimationSpeed = 1f;
    [SerializeField] public Texture2D TransitionTex;
    [SerializeField] public bool Reversed;
    [SerializeField] public float waitTime;

    void OnRenderImage(RenderTexture src, RenderTexture dst){
        TransitionMaterial.SetTexture("_TransitionTex", TransitionTex);
        Graphics.Blit(src, dst, TransitionMaterial);
    }

    public IEnumerator TransitionAnimation(){
        Shader.SetGlobalFloat("_Cutoff", 0f);
        this.enabled = true;
        for (float t = -waitTime; t < 1.0f + waitTime; t += Time.deltaTime * AnimationSpeed){
            if (Reversed) {
                Shader.SetGlobalFloat("_Cutoff", 1.0f + waitTime - t);
            }
            else {
                Shader.SetGlobalFloat("_Cutoff", t);
            }
            yield return new WaitForEndOfFrame();
        }
        this.enabled = false;
    }
}
