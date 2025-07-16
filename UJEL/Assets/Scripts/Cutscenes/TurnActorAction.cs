using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActorAction : CutsceneAction
{
    [SerializeField] CutsceneActor actor;
    [SerializeField] FacingDirection direction;

    public override IEnumerator Play()
    {
        // Note the player is currently NOT a character, set facing direction will not work.
        actor.GetCharacter().animator.SetFacingDirection(direction);
        yield break;
    }

}
