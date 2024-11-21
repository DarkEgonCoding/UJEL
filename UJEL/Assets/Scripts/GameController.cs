using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Video;

public enum GameState { FreeRoam, Battle, Dialog, Pause, Trainer}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] Camera battleCamera;
    [SerializeField] AudioClip wildBattleMusic;
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

        AudioManager.instance.PlayMusic(wildBattleMusic, startSeconds: .5f);

        ScreenTransition transition = worldCamera.GetComponent<ScreenTransition>();
        transition.Reversed = false;

        yield return StartCoroutine(transition.TransitionAnimation());

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        
        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
        battleSystem.StartBattle(playerParty, wildPokemon);

        transition = battleCamera.GetComponent<ScreenTransition>();
        transition.Reversed = true;
        yield return StartCoroutine(transition.TransitionAnimation());

        yield return battleSystem.EnterPokemon();
    }

    public IEnumerator StartTrainerBattle(TrainerController trainer){
        state = GameState.Battle;

        AudioManager.instance.PlayMusic(trainer.trainerBattleMusic, startSeconds: 0.5f);

        ScreenTransition transition = worldCamera.GetComponent<ScreenTransition>();
        transition.Reversed = false;

        yield return StartCoroutine(transition.TransitionAnimation());

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        
        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);

        transition = battleCamera.GetComponent<ScreenTransition>();
        transition.Reversed = true;
        yield return StartCoroutine(transition.TransitionAnimation());
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
