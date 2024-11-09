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

    void OnRenderImage(RenderTexture src, RenderTexture dst){
        TransitionMaterial.SetTexture("_TransitionTex", TransitionTex);
        Graphics.Blit(src, dst, TransitionMaterial);
    }

    public IEnumerator TransitionAnimation(){
        Shader.SetGlobalFloat("_Cutoff", 0f);
        this.enabled = true;
        yield return new WaitForSeconds(0.5f);
        for (float t = 0f; t<1.0f; t += Time.deltaTime * AnimationSpeed){
            Shader.SetGlobalFloat("_Cutoff", t);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        this.enabled = false;
    }
}
