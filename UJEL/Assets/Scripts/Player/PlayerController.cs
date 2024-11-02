using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;

public enum GameState { FreeRoam, Battle, Dialog}

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float OriginalMoveSpeed = 5f;
    public float moveSpeed;
    [SerializeField] public float runSpeed = 9f;
    public bool isMoving;
    public PlayerControls controls;
    Vector2 moveDirection;
    private Animator animator;
    [SerializeField] public LayerMask solidObjectsLayer;
    [SerializeField] public LayerMask grassLayer;
    [SerializeField] public LayerMask interactableLayer;
    [SerializeField] public float EncounterPercentage = 10f;
    bool UpisPressed;
    bool DownisPressed;
    bool LeftisPressed;
    bool RightisPressed;
    bool XisPressed;
    public GameState state;
    public bool inDialog;
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
        if(!isMoving && UpisPressed && !inDialog){
            StartCoroutine(DoMove(Vector2.up));
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 1);
        } 
        if(!isMoving && DownisPressed && !inDialog){
            StartCoroutine(DoMove(Vector2.down));
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", -1);
        }
        if(!isMoving && LeftisPressed && !inDialog){
            StartCoroutine(DoMove(Vector2.left));
            animator.SetFloat("moveX", -1);
            animator.SetFloat("moveY", 0);
        } 
        if(!isMoving && RightisPressed && !inDialog){
            StartCoroutine(DoMove(Vector2.right));
            animator.SetFloat("moveX", 1);
            animator.SetFloat("moveY", 0);
        } 
        if(XisPressed){
            moveSpeed = runSpeed;
            animator.speed = 1.5f;
        }
        else {
            moveSpeed = OriginalMoveSpeed;
            animator.speed = 1f;
        }
    }

    private bool IsWalkable(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.3f, solidObjectsLayer | interactableLayer) != null){
            animator.SetBool("isMoving", false);
            return false;
        }
        return true;
    }

    IEnumerator DoMove(Vector2 direction){
        isMoving = true;        
        animator.SetBool("isMoving", isMoving);
        float speed = moveSpeed;
        
        var targetPos = transform.position;
        targetPos += (Vector3)direction;
        if(IsWalkable(targetPos)){
            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos;
        }
        
        CheckForEncounters();

        isMoving = false;
        if (!UpisPressed && !DownisPressed && !LeftisPressed && !RightisPressed) animator.SetBool("isMoving", false);
        moveDirection = Vector2.zero;
    }

    private void CheckForEncounters(){
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null){
            if (Random.Range(1, 101) <= EncounterPercentage){
                Debug.Log("Encounter");
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
}
