using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycleShader : MonoBehaviour
{
    [SerializeField] Material mat;

    [SerializeField] float dawnStartTime;
    [SerializeField] float dawnBlendTime;
    [SerializeField] Color dawnColor;
    [SerializeField] float dawnIntensity;

    [SerializeField] float dayStartTime;
    [SerializeField] float dayBlendTime;
    [SerializeField] Color dayColor;
    [SerializeField] float dayIntensity;

    [SerializeField] float eveningStartTime;
    [SerializeField] float eveningBlendTime;
    [SerializeField] Color eveningColor;
    [SerializeField] float eveningIntensity;

    [SerializeField] float duskStartTime;
    [SerializeField] float duskBlendTime;
    [SerializeField] Color duskColor;
    [SerializeField] float duskIntensity;

    [SerializeField] float nightStartTime;
    [SerializeField] float nightBlendTime;
    [SerializeField] Color nightColor;
    [SerializeField] float nightIntensity;

    float time = 0.0f;
    [SerializeField] float enricoPucci;
    [SerializeField] float dayCycleTime;

    // Run the post processing shader.
    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        time += Time.deltaTime * enricoPucci;
        time = time > dayCycleTime ? time - dayCycleTime : time;

        Color lightColor;
        float intensity;
        float blendProgress;

        if (time > dawnStartTime && time < dayStartTime) {
            blendProgress = (time - dawnStartTime) / dawnBlendTime;
            blendProgress = blendProgress > 1.0f ? 1.0f : blendProgress;
            lightColor = Color.Lerp(nightColor, dawnColor, blendProgress);
            intensity = dawnIntensity;
        } else if (time > dayStartTime && time < eveningStartTime) {
            blendProgress = (time - dayStartTime) / dayBlendTime;
            blendProgress = blendProgress > 1.0f ? 1.0f : blendProgress;
            lightColor = Color.Lerp(dawnColor, dayColor, blendProgress);
            intensity = dayIntensity;
        } else if (time > eveningStartTime && time < duskStartTime) {
            blendProgress = (time - eveningStartTime) / eveningBlendTime;
            blendProgress = blendProgress > 1.0f ? 1.0f : blendProgress;
            lightColor = Color.Lerp(dayColor, eveningColor, blendProgress);
            intensity = eveningIntensity;
        } else if (time > duskStartTime && time < nightStartTime) {
            blendProgress = (time - duskStartTime) / duskBlendTime;
            blendProgress = blendProgress > 1.0f ? 1.0f : blendProgress;
            lightColor = Color.Lerp(eveningColor, duskColor, blendProgress);
            intensity = duskIntensity;
        } else { // All other times are night.
            // IMPORTANT: Assumes that DAYCYCLETIME - nightBlendTime > 0.
            blendProgress = (time - nightStartTime) / nightBlendTime;
            blendProgress = blendProgress > 1.0f || blendProgress < 0 ? 1.0f : blendProgress;
            lightColor = Color.Lerp(duskColor, nightColor, blendProgress);
            intensity = nightIntensity;
        }

        intensity = 1.0f;
        Debug.Log(time);
        Shader.SetGlobalFloat("_Intensity", intensity);
        Shader.SetGlobalFloat("_Time", time);
        Shader.SetGlobalVector("_BlendColor", lightColor);
        Graphics.Blit(src, dest, mat);
    }

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
