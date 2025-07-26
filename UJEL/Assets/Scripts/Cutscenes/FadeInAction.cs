using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FadeInAction : CutsceneAction
{
    [SerializeField] float duration;

    public override IEnumerator Play()
    {
        yield return Fader.instance.FadeIn(duration);
    }
}
