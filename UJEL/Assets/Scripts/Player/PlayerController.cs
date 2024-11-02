using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;

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
    bool UpisPressed;
    bool DownisPressed;
    bool LeftisPressed;
    bool RightisPressed;
    bool XisPressed;
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
    }

    void Update(){
        if(!isMoving && UpisPressed){
            StartCoroutine(DoMove(Vector2.up));
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 1);
        } 
        if(!isMoving && DownisPressed){
            StartCoroutine(DoMove(Vector2.down));
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", -1);
        }
        if(!isMoving && LeftisPressed){
            StartCoroutine(DoMove(Vector2.left));
            animator.SetFloat("moveX", -1);
            animator.SetFloat("moveY", 0);
        } 
        if(!isMoving && RightisPressed){
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
        if(Physics2D.OverlapCircle(targetPos, 0.3f, solidObjectsLayer) != null){
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
        
        isMoving = false;
        if (!UpisPressed && !DownisPressed && !LeftisPressed && !RightisPressed) animator.SetBool("isMoving", false);
        moveDirection = Vector2.zero;
    }
}
