using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField] private Color morningColor;
    [SerializeField] private float morningIntensity;
    [SerializeField] public float morningLength = 5;
    [SerializeField] private Color dayColor;
    [SerializeField] private float dayIntensity;
    [SerializeField] public float dayLength = 7;
    [SerializeField] private Color afternoonColor;
    [SerializeField] private float afternoonIntensity;
    [SerializeField] public float afternoonLength = 5;
    [SerializeField] private Color sunsetColor;
    [SerializeField] private float sunsetIntensity;
    [SerializeField] public float sunsetLength = 2;
    [SerializeField] private Color nightColor;
    [SerializeField] private float nightIntensity;
    [SerializeField] public float nightLength = 6;
    [SerializeField] private float time;
    [SerializeField] private float timeMin;
    [SerializeField] public float timeModifier = 1;
    private float transitionTime = 1;
    [SerializeField] Light light;

    private void Start(){
        time = 0;
        morningLength *= 60;
        dayLength *= 60;
        dayLength += morningLength;
        afternoonLength *= 60;
        afternoonLength += dayLength;
        sunsetLength *= 60;
        sunsetLength += afternoonLength;
        nightLength *= 60;
        nightLength += sunsetLength;
        light.intensity = morningIntensity;
        light.color = morningColor;

        transitionTime *= 60;
    }

    private void Update(){
        time += Time.deltaTime * timeModifier;
        timeMin = time/60;
        if(time > nightLength-transitionTime && time < nightLength){
            float t = (time-nightLength + transitionTime) / transitionTime;
            light.intensity = Mathf.Lerp(nightIntensity, morningIntensity, t);
            light.color = Color.Lerp(nightColor, morningColor, t);
        }
        else if(time > morningLength-transitionTime && time < morningLength){
            float t = (time-morningLength + transitionTime) / transitionTime;
            light.intensity = Mathf.Lerp(morningIntensity, dayIntensity, t);
            light.color = Color.Lerp(morningColor, dayColor, t);
        }
        else if(time > dayLength-transitionTime && time < dayLength){
            float t = (time-dayLength + transitionTime) / transitionTime;
            light.intensity = Mathf.Lerp(dayIntensity, afternoonIntensity, t);
            light.color = Color.Lerp(dayColor, afternoonColor, t);
        }
        else if(time > afternoonLength-transitionTime && time < afternoonLength){
            float t = (time-afternoonLength + transitionTime) / transitionTime;
            light.intensity = Mathf.Lerp(afternoonIntensity, sunsetIntensity, t);
            light.color = Color.Lerp(afternoonColor, sunsetColor, t);
        }
        else if(time > sunsetLength-transitionTime && time < sunsetLength){
            float t = (time-sunsetLength + transitionTime) / transitionTime;
            light.intensity = Mathf.Lerp(sunsetIntensity, nightIntensity, t);
            light.color = Color.Lerp(sunsetColor, nightColor, t);
        }
        else if(time > nightLength){
            time = 0;
        }
    }
}
