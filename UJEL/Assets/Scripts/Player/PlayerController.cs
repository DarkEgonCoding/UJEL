using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5;
    [SerializeField] public float runSpeed = 9;
    public bool isMoving;
    public PlayerControls controls;
    Vector2 moveDirection;
    private Animator animator;
    [SerializeField] public LayerMask solidObjectsLayer;

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
        controls.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>()); 
    }

    private void Move(Vector2 direction){
        moveDirection = direction;
        animator.SetFloat("moveX", moveDirection.x);
        animator.SetFloat("moveY", moveDirection.y);
    }

    void Update(){
        if(!isMoving && moveDirection!=Vector2.zero) StartCoroutine(DoMove(moveDirection));
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
        animator.SetBool("isMoving", isMoving);
    }
}
