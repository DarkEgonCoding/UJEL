using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using JetBrains.Annotations;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy, PartyScreen}

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
    public PlayerControls controls;
    public UnityEvent<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    PokemonParty playerParty;
    Pokemon wildPokemon;

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
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        SetupBattle();
    }

    public void SetupBattle(){
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
    }

    public IEnumerator EnterPokemon(){
            enemyUnit.PlayEnterAnimation();
            playerUnit.PlayEnterAnimation();
            
            yield return dialogBox.StartDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(PlayerAction());
    }

    private IEnumerator PlayerAction(bool overrideActive = false){
        if(!dialogBox.actionSelector.activeInHierarchy || overrideActive){
            Debug.Log("playeraction");
            state = BattleState.PlayerAction;
            yield return dialogBox.StartDialog($"What should {playerUnit.Pokemon.Base.Name} do?");
            yield return new WaitForSeconds(0.5f);
            dialogBox.EnableActionSelector(true);
        }
    }

    private void OpenPartyScreen(){
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void PlayerMove(){
        if(dialogBox.actionSelector.activeSelf){
            state = BattleState.PlayerMove;
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            dialogBox.EnableMoveSelector(true);
        }
    }

    IEnumerator PerformPlayerMove(){
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
            OnBattleOver.Invoke(true);
        }
        else{
            yield return enemyHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            yield return StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove(){
        if(dialogBox.isTyping) yield break;
        state = BattleState.EnemyMove;

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
            yield return StartCoroutine(PlayerAction());
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
        if (state == BattleState.PlayerAction){
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove){
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
                PlayerMove();
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
            StartCoroutine(PlayerAction());
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (controls.Main.Interact.WasPerformedThisFrame()){
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
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
            StartCoroutine(PlayerAction(overrideActive: true));
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
}
