using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using JetBrains.Annotations;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.Analytics;

public class BattleSystemUI : MonoBehaviour
{
    public static BattleSystemUI instance;

    [Header("UI Elements")]
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;

    [Header("Boxes and Screens")]
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    
    [Header("Audio")]
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip battleVictoryMusic;

    [Header("Visuals")]
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;

    public PlayerControls controls;
    public UnityEvent<bool> OnBattleOver;

    BattleQueue battleQueue = new BattleQueue();
    
    int currentAction;
    int currentMove;
    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;
    PlayerController player;
    TrainerController trainer;
    bool isTrainerBattle = false;
    int escapeAttempts;
    MoveBase moveToLearn;
    public bool didLearnMove;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// Runs the init functions in order of the wild pokemon encounter
    /// </summary>
    /// <param name="playerParty"></param>
    /// <param name="wildPokemon"></param>
    /// <returns></returns>
    public IEnumerator BattleInit(PokemonParty playerParty, Pokemon wildPokemon)
    {
        StartBattle(playerParty, wildPokemon);
        yield return StartCoroutine(SetupBattle());

        PokemonParty enemyParty = new PokemonParty();
        enemyParty.AddPokemon(wildPokemon);
        ServerInit(playerParty, enemyParty);
    }

    /// <summary>
    /// Runs the init functions in order of the Trainer battle encounter
    /// </summary>
    /// <param name="playerParty"></param>
    /// <param name="trainerParty"></param>
    /// <returns></returns>
    public IEnumerator BattleInit(PokemonParty playerParty, PokemonParty trainerParty)
    {
        isTrainerBattle = true;
        StartTrainerBattle(playerParty, trainerParty);
        yield return StartCoroutine(SetupBattle());
        ServerInit(playerParty, trainerParty);
    }

    /// <summary>
    /// Sets up the player and wild pokemon and dialog box
    /// </summary>
    /// <param name="playerParty"></param>
    /// <param name="wildPokemon"></param>
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        dialogBox.SetDialog("");
        isTrainerBattle = false;
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
    }

    /// <summary>
    /// Sets up the player and trainer and the dialog box
    /// </summary>
    /// <param name="playerParty"></param>
    /// <param name="trainerParty"></param>
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        dialogBox.SetDialog("");
        isTrainerBattle = true;
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
    }

    public IEnumerator SetupBattle()
    {
        if (!isTrainerBattle)
        { // Wild Pokemon Battle
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);
            playerHud.SetData(playerUnit.Pokemon);
            enemyHud.SetData(enemyUnit.Pokemon);

            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }
        else
        { // Trainer Battle
            yield return StartCoroutine(EnterTrainers());

            // Send out first pokemon of trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            enemyHud.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            enemyHud.SetData(enemyUnit.Pokemon);
            enemyUnit.PlayEnterAnimation();
            yield return DisplayDialog($"{trainer.Name} sent out {enemyPokemon.Base.PokemonName}!", 1.3f);
            
            // Send out first pokemon of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            playerHud.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            playerHud.SetData(playerUnit.Pokemon);
            playerUnit.PlayEnterAnimation();
            yield return dialogBox.StartDialog($"Go {playerPokemon.Base.PokemonName}!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return new WaitForSeconds(1.3f);
        }
        partyScreen.Init();
        escapeAttempts = 0;
    }

    public IEnumerator EnterTrainers()
    {
        playerHud.gameObject.SetActive(false);
        enemyHud.gameObject.SetActive(false);

        playerUnit.gameObject.SetActive(false);
        enemyUnit.gameObject.SetActive(false);

        playerImage.gameObject.SetActive(true);
        trainerImage.gameObject.SetActive(true);
        playerImage.sprite = player.Sprite;
        trainerImage.sprite = trainer.Sprite;

        yield return new WaitForSeconds(1f);
        yield return DisplayDialog($"{trainer.Name} wants to battle!", 1.3f);
    }

    /// <summary>
    /// To be called by GameController when the transition graphic is finished
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnterPokemon()
    {
        enemyUnit.PlayEnterAnimation();
        playerUnit.PlayEnterAnimation();
        yield return DisplayDialog($"A wild {enemyUnit.Pokemon.Base.PokemonName} appeared!", 1f);
    }

    private void ServerInit(PokemonParty playerParty, PokemonParty enemyParty)
    {
        string packedOne = PokemonPacker.Pack(playerParty);
        string packedTwo = PokemonPacker.Pack(enemyParty);
        battleQueue.Init(packedOne, packedTwo);
    }

    public IEnumerator DisplayDialog(string dialog, float waitTime = 0)
    {
        yield return dialogBox.StartDialog(dialog);
        yield return new WaitForSeconds(waitTime);
    }




}
