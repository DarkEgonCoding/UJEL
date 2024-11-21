using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using JetBrains.Annotations;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip battleVictoryMusic;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    public PlayerControls controls;
    public UnityEvent<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;
    PlayerController player;
    TrainerController trainer;

    bool isTrainerBattle = false;

    private void Awake(){
        controls = new PlayerControls();
    }

    private void OnEnable(){
        controls.Enable();
        //controls.Main.Interact.performed += ctx => dialogBox.TrySkipDialog();
    }

    private void OnDisable(){
        controls.Disable();
        //controls.Main.Interact.performed -= ctx => dialogBox.TrySkipDialog();
    }

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon){
        dialogBox.SetDialog("");
        isTrainerBattle = false;
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty){
        dialogBox.SetDialog("");
        isTrainerBattle = true;
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle(){
        if (!isTrainerBattle){ // Wild Pokemon Battle
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);
            playerHud.SetData(playerUnit.Pokemon);
            enemyHud.SetData(enemyUnit.Pokemon);

            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }
        else { // Trainer Battle
            yield return StartCoroutine(EnterTrainers());

            // Send out first pokemon of trainer
                trainerImage.gameObject.SetActive(false);
                enemyUnit.gameObject.SetActive(true);
                enemyHud.gameObject.SetActive(true);
                var enemyPokemon = trainerParty.GetHealthyPokemon();
                enemyUnit.Setup(enemyPokemon);
                enemyHud.SetData(enemyUnit.Pokemon);
                enemyUnit.PlayEnterAnimation();
                yield return dialogBox.StartDialog($"{trainer.Name} sent out {enemyPokemon.Base.Name}!");
                yield return new WaitForSeconds(1.3f);
            // Send out first pokemon of the player
                playerImage.gameObject.SetActive(false);
                playerUnit.gameObject.SetActive(true);
                playerHud.gameObject.SetActive(true);
                var playerPokemon = playerParty.GetHealthyPokemon();
                playerUnit.Setup(playerPokemon);
                playerHud.SetData(playerUnit.Pokemon);
                playerUnit.PlayEnterAnimation();
                yield return dialogBox.StartDialog($"Go {playerPokemon.Base.Name}!");
                dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                yield return new WaitForSeconds(1.3f);
                yield return StartCoroutine(ActionSelection());
        }
        partyScreen.Init();
    }

    public IEnumerator EnterTrainers(){
        playerHud.gameObject.SetActive(false);
        enemyHud.gameObject.SetActive(false);

        playerUnit.gameObject.SetActive(false);
        enemyUnit.gameObject.SetActive(false);

        playerImage.gameObject.SetActive(true);
        trainerImage.gameObject.SetActive(true);
        playerImage.sprite = player.Sprite;
        trainerImage.sprite = trainer.Sprite;

        yield return new WaitForSeconds(1f);
        yield return dialogBox.StartDialog($"{trainer.Name} wants to battle!");
        yield return new WaitForSeconds(1.3f);
    }

    public IEnumerator EnterPokemon(){
            enemyUnit.PlayEnterAnimation();
            playerUnit.PlayEnterAnimation();
            
            yield return dialogBox.StartDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(ActionSelection());
    }

    private IEnumerator ActionSelection(bool overrideActive = false){
        if(!dialogBox.actionSelector.activeInHierarchy || overrideActive){
            state = BattleState.ActionSelection;
            yield return dialogBox.StartDialog($"What should {playerUnit.Pokemon.Base.Name} do?");
            yield return new WaitForSeconds(0.5f);
            dialogBox.EnableActionSelector(true);
        }
    }

    private void OpenPartyScreen(){
        dialogBox.EnableActionSelector(false);
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection(){
        if(dialogBox.actionSelector.activeSelf){
            state = BattleState.MoveSelection;
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            dialogBox.EnableMoveSelector(true);
        }
    }

    IEnumerator PlayerMove(){
        if(dialogBox.isTyping) yield break;
        state = BattleState.Busy;

        var move = playerUnit.Pokemon.Moves[currentMove];
        dialogBox.StartDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.name}!");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();

        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        move.PP--;

        if(damageDetails.Fainted){
            yield return enemyHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            dialogBox.StartDialog($"{enemyUnit.Pokemon.Base.Name} fainted!");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            if (!isTrainerBattle){
                OnBattleOver.Invoke(true);
            }
            else {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null){
                    StartCoroutine(SendNextTrainerPokemon(nextPokemon));
                }
                else{
                    OnBattleOver.Invoke(true);
                }
            }
        }
        else{
            yield return enemyHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            yield return StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove(){
        if(dialogBox.isTyping) yield break;
        state = BattleState.PerformMove;

        //Select Move
        var move = enemyUnit.Pokemon.GetRandomMove();

        dialogBox.StartDialog($"Enemy {enemyUnit.Pokemon.Base.Name} used {move.Base.name}!");
        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();

        var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        move.PP--;

        if(damageDetails.Fainted){
            yield return playerHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            dialogBox.StartDialog($"{playerUnit.Pokemon.Base.Name} fainted!");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            var nextPokemon = playerParty.GetHealthyPokemon();
            if(nextPokemon != null){
                OpenPartyScreen();
            }
            else{
                OnBattleOver.Invoke(false);
            }
        }
        else if(damageDetails.Fainted == false){
            yield return playerHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            yield return StartCoroutine(ActionSelection());
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails){
        if (damageDetails.Critical > 1f){
            yield return dialogBox.StartDialog("A critical hit!");
            yield return new WaitForSeconds(0.5f);
        }

        if (damageDetails.TypeEffectiveness > 1){
            yield return dialogBox.StartDialog("It's super effective!");
            yield return new WaitForSeconds(0.5f);
        }
        else if (damageDetails.TypeEffectiveness < 1){
            yield return dialogBox.StartDialog("It's not very effective...");
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void HandleUpdate(){
        if (state == BattleState.ActionSelection){
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection){
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen){
            HandlePartySelection();
        }
    }

    void HandleActionSelection(){
        if(controls.Main.Down.WasPerformedThisFrame()){
            currentAction += 2;
        }
        else if (controls.Main.Up.WasPerformedThisFrame()){
            currentAction -= 2;
        }
        else if (controls.Main.Right.WasPerformedThisFrame()){
            ++currentAction;
        }
        else if (controls.Main.Left.WasPerformedThisFrame()){
            --currentAction;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (controls.Main.Interact.WasPerformedThisFrame()){
            if (currentAction == 0){
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1){
                // Bag
            }
            else if (currentAction == 2){
                // Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 3){
                // Run
            }
        }
    }

    void HandleMoveSelection(){
        if(controls.Main.Right.WasPerformedThisFrame()){
                ++currentMove;
        }
        else if (controls.Main.Left.WasPerformedThisFrame()){
                --currentMove;
            }
        else if (controls.Main.Down.WasPerformedThisFrame()){
                currentMove += 2;
        }
        else if (controls.Main.Up.WasPerformedThisFrame()){
                currentMove -= 2;
        }
        else if (controls.Main.Run.WasPerformedThisFrame()){
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(ActionSelection());
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (controls.Main.Interact.WasPerformedThisFrame()){
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
    }

    int currentMember;
    void HandlePartySelection(){
        if(controls.Main.Right.WasPerformedThisFrame()){
                ++currentMember;
        }
        else if (controls.Main.Left.WasPerformedThisFrame()){
                --currentMember;
            }
        else if (controls.Main.Down.WasPerformedThisFrame()){
                currentMember += 2;
        }
        else if (controls.Main.Up.WasPerformedThisFrame()){
                currentMember -= 2;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (controls.Main.Interact.WasPerformedThisFrame()){
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0){
                return;
            }
            if (selectedMember == playerUnit.Pokemon){
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            dialogBox.actionSelector.SetActive(false);
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (controls.Main.Run.WasPerformedThisFrame()){
            if (playerUnit.Pokemon.HP <= 0) return;
            
            partyScreen.gameObject.SetActive(false);
            StartCoroutine(ActionSelection(overrideActive: true));
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon){
        if (playerUnit.Pokemon.HP > 0){
        yield return dialogBox.StartDialog($"Come back {playerUnit.Pokemon.Base.Name}!");
        playerUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        playerHud.SetData(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.StartDialog($"Go {newPokemon.Base.Name}!");
        playerUnit.PlayEnterAnimation();
        yield return new WaitForSeconds(1f);

        StartCoroutine(EnemyMove());
    }

    IEnumerator SendNextTrainerPokemon(Pokemon nextPokemon){
        state = BattleState.Busy;
        enemyUnit.Setup(nextPokemon);
        enemyHud.SetData(nextPokemon);
        yield return dialogBox.StartDialog($"{trainer.Name} sent out {nextPokemon.Base.Name}!");
        enemyUnit.PlayEnterAnimation();
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ActionSelection());
    }
}
