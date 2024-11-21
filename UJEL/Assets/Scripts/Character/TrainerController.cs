using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] string name;
    [SerializeField] GameObject exclamation;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject fov;
    [SerializeField] AudioClip spottedMusic;
    [SerializeField] public AudioClip trainerBattleMusic;
    

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

    public string Name {
        get => name;
    }
    public Sprite Sprite {
        get => sprite;
    }
}
