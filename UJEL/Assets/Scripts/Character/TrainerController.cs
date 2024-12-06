using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Sprite sprite;
    [SerializeField] string name;
    [SerializeField] GameObject exclamation;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog PostBattleDialog;
    [SerializeField] Dialog SecondPostBattleDialog;
    [SerializeField] bool DisableDialogWhenFinished = false;
    [SerializeField] GameObject fov;
    [SerializeField] AudioClip spottedMusic;
    [SerializeField] public AudioClip trainerBattleMusic;

    bool AlreadyBattled = false;
    int DialogSelection = 1;
    

    Character character;

    private void Awake(){
        character = GetComponent<Character>();
    }

    private void Start(){
        SetFovRotation(character.animator.DefaultDirection);
    }

    public void OnSeePlayer(Collider2D playerCollider){
        AudioManager.instance.PlayMusic(spottedMusic, startSeconds: 0.65f);
        GameController.instance.state = GameState.Trainer;
        StartCoroutine(TriggerTrainerBattle(playerCollider));
    }

    public void Interact(Transform initiator){

        character.LookTowards(initiator.position);
        
        if (AlreadyBattled == true){
            if(PostBattleDialog != null && DialogSelection == 1){
                character.LookTowards(initiator.position);
                DialogManager.Instance.ShowDialog(PostBattleDialog);
                DialogSelection = 2;
                return;
            }
            if(SecondPostBattleDialog != null && DialogSelection == 2){
                character.LookTowards(initiator.position);
                DialogManager.Instance.ShowDialog(SecondPostBattleDialog);
                DialogSelection = 1;
                if(DisableDialogWhenFinished) DialogSelection = 0;
                return;
            }
            return;
        }

        DialogManager.Instance.ShowDialog(dialog, () => {
            StartCoroutine(GameController.instance.StartTrainerBattle(this));
            fov.SetActive(false);
            AlreadyBattled = true;
        });
    }

    public IEnumerator TriggerTrainerBattle(Collider2D playerCollider){
        yield return ShowExclamation(); // Show Exclamation

        // Walk towards the player
        var diff = playerCollider.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        yield return character.DoCharacterMove(moveVec);

        // Show dialog
        DialogManager.Instance.ShowDialog(dialog, () => {
            StartCoroutine(GameController.instance.StartTrainerBattle(this));
            fov.SetActive(false);
            AlreadyBattled = true;
        });
    }

    public IEnumerator ShowExclamation(){
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        exclamation.SetActive(false);
    }

    public void SetFovRotation(FacingDirection dir){
        float angle = 0f;
        if (dir == FacingDirection.Right){
            angle = 90f;
        }
        else if (dir == FacingDirection.Up){
            angle = 180f;
        }
        else if (dir == FacingDirection.Left){
            angle = 270f;
        }

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return AlreadyBattled;
    }

    public void RestoreState(object state)
    {
        AlreadyBattled = (bool)state;
        
        character.animator.SetFacingDirection(character.animator.DefaultDirection);

        if (AlreadyBattled){
            fov.SetActive(false);
        }
        else{
            fov.SetActive(true);
        }
    }

    public string Name {
        get => name;
    }
    public Sprite Sprite {
        get => sprite;
    }
}
