using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.TextCore.Text;

public enum GameState { FreeRoam, Battle, Dialog, Pause}

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float OriginalMoveSpeed = 5f;
    public float moveSpeed;
    [SerializeField] public float runSpeed = 9f;
    public PlayerController player;
    [SerializeField] public float jumpSpeed = 5f;
    [SerializeField] public float swimSpeed = 3f;
    [SerializeField] public float runSwimSpeed = 6f;
    public bool isMoving;
    public bool isSwimming = false;
    public PlayerControls controls;
    Vector2 moveDirection;
    private Animator animator;
    [SerializeField] public LayerMask solidObjectsLayer;
    [SerializeField] public LayerMask grassLayer;
    [SerializeField] public LayerMask interactableLayer;
    [SerializeField] public LayerMask ledgeLayer;
    [SerializeField] public LayerMask portalLayer;
    [SerializeField] public LayerMask waterLayer;
    public LayerMask TriggerableLayers {
        get => grassLayer | portalLayer;
    }
    bool UpisPressed;
    bool DownisPressed;
    bool LeftisPressed;
    bool RightisPressed;
    bool XisPressed;
    public GameState state;
    public GameState stateBeforePause;
    public bool inDialog;
    public bool inJump = false;
    public bool inEncounter = false;
    public bool canSwim = false;
    private void Awake(){
        controls = new PlayerControls();
    }

    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }

    void Start(){
        animator = GetComponent<Animator>();
        state = GameState.FreeRoam;
        SetPositionAndSnapToTile(transform.position);

        DialogManager.Instance.OnShowDialog += () => {
            inDialog = true;
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () => {
            inDialog = false;
            state = GameState.FreeRoam;
        };


        //Movement Held
        controls.Main.Up.performed += ctx => UpisPressed = true;
        controls.Main.Up.canceled += ctx => UpisPressed = false;
        controls.Main.Down.performed += ctx => DownisPressed = true;
        controls.Main.Down.canceled += ctx => DownisPressed = false;
        controls.Main.Left.performed += ctx => LeftisPressed = true;
        controls.Main.Left.canceled += ctx => LeftisPressed = false;
        controls.Main.Right.performed += ctx => RightisPressed = true;
        controls.Main.Right.canceled += ctx => RightisPressed = false;
        
        //Run
        controls.Main.Run.performed += ctx => XisPressed = true;
        controls.Main.Run.canceled += ctx => XisPressed = false;
        moveSpeed = OriginalMoveSpeed;

        //Interact
        controls.Main.Interact.performed += ctx => Interact();
    }

    void Update(){
        if(state == GameState.FreeRoam){
            if(!isMoving && UpisPressed && !inDialog && !inJump && !inEncounter){
                StartCoroutine(DoMove(Vector2.up));
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 1);
            } 
            if(!isMoving && DownisPressed && !inDialog && !inJump && !inEncounter){
                StartCoroutine(DoMove(Vector2.down));
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", -1);
            }
            if(!isMoving && LeftisPressed && !inDialog && !inJump && !inEncounter){
                StartCoroutine(DoMove(Vector2.left));
                animator.SetFloat("moveX", -1);
                animator.SetFloat("moveY", 0);
            } 
            if(!isMoving && RightisPressed && !inDialog && !inJump && !inEncounter){
                StartCoroutine(DoMove(Vector2.right));
                animator.SetFloat("moveX", 1);
                animator.SetFloat("moveY", 0);
            } 
        }
        if(XisPressed && !isSwimming){
            moveSpeed = runSpeed;
            animator.speed = 1.5f;
        }
        else {
            moveSpeed = OriginalMoveSpeed;
            animator.speed = 1f;
        }
    }

    private bool IsWalkable(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.3f, solidObjectsLayer | interactableLayer | ledgeLayer) != null){
            animator.SetBool("isMoving", false);
            return false;
        }
        if(Physics2D.OverlapCircle(targetPos, 0.3f, waterLayer) != null){
            if(canSwim == true){
                isSwimming = true;
                return true; //return true if you unlocked swim
            }
            else return false; //return false if you cannot swim
        }
        return true; //return true if it doesn't collide with anything
    }

    IEnumerator DoMove(Vector2 direction){
        isMoving = true;        
        animator.SetBool("isMoving", isMoving);
        
        var targetPos = transform.position;
        targetPos += (Vector3)direction;

        var ledge = CheckForLedge(targetPos);
        if (ledge != null){
            if (TryToJump(ledge, direction)){
                isMoving = false;
                animator.SetBool("isMoving", false);
                yield break;
            }
        }

        if(IsWalkable(targetPos)){
            if(isSwimming){
                animator.SetBool("isMoving", false);
                moveSpeed = swimSpeed;
                if(XisPressed){
                    moveSpeed = runSwimSpeed;
                }
            }
            float speed = moveSpeed;
            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos;
        }
        
        OnMoveOver();

        isMoving = false;
        isSwimming = false;
        if (!UpisPressed && !DownisPressed && !LeftisPressed && !RightisPressed) animator.SetBool("isMoving", false);
        moveDirection = Vector2.zero;
    }

    private void OnMoveOver(){
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, TriggerableLayers);
        
        foreach (var collider in colliders){
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null){
                animator.SetBool("isMoving", false);
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    void Interact(){
        if(inDialog){ //if in dialog, next line
            if(DialogManager.Instance.isTyping == false){
                DialogManager.Instance.NextDialog();
            }
        }
        else{ // if not in dialog, check for interaction
            var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
            var interactPos = transform.position + facingDir;
            
            var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
            if (collider != null){
                collider.GetComponent<Interactable>()?.Interact();
            }
        }
    }

// LEDGE JUMP Mechanics
    Ledge CheckForLedge(Vector3 targetPos){
        var collider = Physics2D.OverlapCircle(targetPos, 0.3f, ledgeLayer);
        return collider?.GetComponent<Ledge>();
    }

    public bool TryToJump(Ledge ledge, Vector2 moveDir){
        if (moveDir.x == ledge.xDir && moveDir.y == ledge.yDir){
            StartCoroutine(Jump(ledge));
            return true;
        }
        return false;
    }

    IEnumerator Jump(Ledge ledge){
        inJump = true;
        
        var jumpDest = transform.position + new Vector3(ledge.xDir, ledge.yDir) * 2;
        while ((jumpDest - transform.position).sqrMagnitude > Mathf.Epsilon){
                transform.position = Vector3.MoveTowards(transform.position, jumpDest, jumpSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = jumpDest;

        inJump = false;
    }

    public void SetPositionAndSnapToTile(Vector2 pos){
        // Example: 2.3 -> Floor -> 2 -> 2.5
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;

        transform.position = pos;
    }

    public void PauseGame(bool pause){
        if (pause){
            stateBeforePause = state;
            state = GameState.Pause;
        }
        else{
            state = stateBeforePause;
        }
    }
}
