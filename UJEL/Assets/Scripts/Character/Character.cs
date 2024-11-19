using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;
    [SerializeField] private float moveSpeed = 5f;


    private void Awake(){
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator DoCharacterMove(Vector2 direction){
        animator.MoveX = Mathf.Clamp(direction.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(direction.y, -1f, 1f);
        
        var targetPos = transform.position;
        targetPos += (Vector3)direction;

        if(IsPathClear(targetPos)){
            animator.IsMoving = true;
            float speed = moveSpeed;
            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos;
        }
        
        // Resets movement
        animator.IsMoving = false;
        animator.MoveX = 0;
        animator.MoveY = 0;
    }

    private bool IsPathClear(Vector3 targetPos){
        var difference = targetPos - transform.position;
        var dir = difference.normalized;
        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, difference.magnitude - 1, GameLayers.i.solidObjectsLayer | GameLayers.i.interactableLayer | GameLayers.i.ledgeLayer | GameLayers.i.playerLayer) == true){
            animator.IsMoving = false;
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos){
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0){
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else{
            Debug.LogError("Error in Look Towards: You can't ask the chracter to look diagonally");
        }

    }
}
