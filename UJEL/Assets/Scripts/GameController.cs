using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Video;

public enum GameState { FreeRoam, Battle, Dialog, Pause, Trainer, Menu}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] Camera battleCamera;
    [SerializeField] AudioClip wildBattleMusic;
    public GameState state;
    GameState stateBeforePause;
    public PlayerControls controls;

    public static GameController instance;

    private void Awake(){
        controls = new PlayerControls();
        PokemonDB.Init();
        MoveDB.Init();
    }

    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }

    private void Start(){
        instance = this;
        playerController.OnEncountered.AddListener(() => StartCoroutine(StartBattle()));
        battleSystem.OnBattleOver.AddListener(EndBattle);

        //Bag
        controls.Main.C.performed += ctx => OpenMenu();

        //Save and Load --- TEMPORARY
        controls.Main.Save.performed += ctx => Save();
        controls.Main.Load.performed += ctx => Load();
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
        else if (state == GameState.Menu){
            MenuHandleUpdate();
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

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
        
        battleSystem.StartBattle(playerParty, wildPokemonCopy);

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

    public void OpenMenu(){
        if (state == GameState.FreeRoam){
            state = GameState.Menu;
        }
    }

    public void MenuHandleUpdate(){
        /*
        if (controls.Main.Down.WasPerformedThisFrame()){
            
        }
        else if (controls.Main.Up.WasPerformedThisFrame()){
            
        }

        if (controls.Main.Interact.WasPerformedThisFrame()){
            
        }
        if (controls.Main.Run.WasPerformedThisFrame()){
            state = GameState.FreeRoam;
            // Set Active false
        }
        */
    }

    public void Save(){
        if (state == GameState.FreeRoam){
            SavingSystem.i.Save("saveSlot1");
        }
    }

    public void Load(){
        if (state == GameState.FreeRoam){
            SavingSystem.i.Load("saveSlot1");
        }
    }
}
