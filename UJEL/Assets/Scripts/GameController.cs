using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public enum GameState { FreeRoam, Battle, Dialog, Pause}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] Camera battleCamera;
    public GameState state;
    GameState stateBeforePause;

    public static GameController instance;

    private void Start(){
        instance = this;
        playerController.OnEncountered.AddListener(() => StartCoroutine(StartBattle()));
        battleSystem.OnBattleOver.AddListener(EndBattle);
    }

    void EndBattle(bool won){
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update(){
        if (state == GameState.FreeRoam){
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle){
            battleSystem.HandleUpdate();
        }
    }

    IEnumerator StartBattle(){
        state = GameState.Battle;

        ScreenTransition transition = worldCamera.GetComponent<ScreenTransition>();
        transition.Reversed = false;

        yield return StartCoroutine(transition.TransitionAnimation());

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        
        battleSystem.StartBattle();

        transition = battleCamera.GetComponent<ScreenTransition>();
        transition.Reversed = true;
        yield return StartCoroutine(transition.TransitionAnimation());

        yield return StartCoroutine(battleSystem.EnterPokemon());
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
