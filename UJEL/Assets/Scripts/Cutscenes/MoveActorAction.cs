using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveActorAction : CutsceneAction
{
    [SerializeField] CutsceneActor actor;
    [SerializeField] List<Vector2> movePatterns;

    public override IEnumerator Play()
    {
        var character = actor.GetCharacter();

        foreach (var moveVec in movePatterns)
        {
            if (character != PlayerController.Instance.character) yield return character.DoCharacterMove(moveVec);
            else yield return PlayerController.Instance.DoMove(moveVec);
        }
    }
}

[System.Serializable]
public class CutsceneActor
{
    [SerializeField] bool isPlayer;
    [SerializeField] Character character;

    public Character GetCharacter() => (isPlayer) ? PlayerController.Instance.character : character;
}
