using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    // Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    //States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteRenderer spriteRenderer;
    SpriteAnimator currentAnim;

    private void Start(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update(){
        var prevAnim = currentAnim;

        if (MoveX != 0 && MoveY != 0) Debug.LogError("Error in CharacterAnimator Update: MoveX and MoveY are != 0");

        // Moving Animations
        if (MoveX == 1){
            currentAnim = walkRightAnim;
        }
        else if (MoveX == -1){
            currentAnim = walkLeftAnim;
        }
        else if (MoveY == 1){
            currentAnim = walkUpAnim;
        }
        else if (MoveY == -1){
            currentAnim = walkDownAnim;
        }

        if (currentAnim != prevAnim){ // if animation has changed
            currentAnim.Start();
        }

        // Idle Animations
        if (IsMoving){
            currentAnim.HandleUpdate();
        }
        else{
            spriteRenderer.sprite = currentAnim.Frames[0];
        }
    }

    public void SetFacingDirection(FacingDirection dir){
        if (dir == FacingDirection.Right){
            MoveX = 1;
            MoveY = 0;
        }
        else if (dir == FacingDirection.Left){
            MoveX = -1;
            MoveY = 0;
        }
        else if (dir == FacingDirection.Down){
            MoveY = -1;
            MoveX = 0;
        }
        else if (dir == FacingDirection.Up){
            MoveY = 1;
            MoveX = 0;
        }
    }

    public FacingDirection DefaultDirection {
        get => defaultDirection;
    }
}

public enum FacingDirection { Up, Down, Left, Right }
