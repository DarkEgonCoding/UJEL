using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern = 2f;
    int currentPattern = 0;

    Character character;

    float idleTimer = 0;
    NPCState state;


    private void Awake(){
        character = GetComponent<Character>();
    }

    private void Update(){
        if (state == NPCState.Idle){
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern){
                idleTimer = 0f;
                if (movementPattern.Count > 0){
                    StartCoroutine(Walk());
                }
            }
        }
    }

    private IEnumerator Walk(){
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.DoCharacterMove(movementPattern[currentPattern]);
        if (transform.position != oldPos){
            currentPattern = (currentPattern + 1) % movementPattern.Count;
        }

        state = NPCState.Idle;
    }

    public void Interact(Transform initiator){
        if (state == NPCState.Idle){
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            DialogManager.Instance.ShowDialog(dialog, () => {
                state = NPCState.Idle;
                idleTimer = 0;
            });
        }
    }
}

public enum NPCState { Idle, Walking, Dialog }
