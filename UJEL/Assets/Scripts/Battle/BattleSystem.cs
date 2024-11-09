using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    public PlayerControls controls;
    public UnityEvent<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    private void Awake(){
        controls = new PlayerControls();
    }

    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }

    public void StartBattle(){
        SetupBattle();
    }

    public void SetupBattle(){
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
    }

    public IEnumerator EnterPokemon(){
        enemyUnit.PlayEnterAnimation();
        playerUnit.PlayEnterAnimation();
        
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared!");

        yield return StartCoroutine(PlayerAction());
    }

    private IEnumerator PlayerAction(){
        state = BattleState.PlayerAction;
        yield return StartCoroutine(dialogBox.TypeDialog($"What should {playerUnit.Pokemon.Base.Name} do?"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove(){
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove(){
        state = BattleState.Busy;

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.name}!");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();

        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        move.PP--;

        if(damageDetails.Fainted){
            yield return enemyHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} fainted!");
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
        state = BattleState.EnemyMove;

        //Select Move
        var move = enemyUnit.Pokemon.GetRandomMove();

        dialogBox.TypeDialog($"Enemy {enemyUnit.Pokemon.Base.Name} used {move.Base.name}!");
        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();

        var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        move.PP--;

        if(damageDetails.Fainted){
            yield return playerHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} fainted!");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver.Invoke(false);
        }
        else if(damageDetails.Fainted == false){
            yield return playerHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            yield return StartCoroutine(PlayerAction());
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails){
        if (damageDetails.Critical > 1f){
            yield return dialogBox.TypeDialog("A critical hit!");
        }

        if (damageDetails.TypeEffectiveness > 1){
            yield return dialogBox.TypeDialog("It's super effective!");
        }
        else if (damageDetails.TypeEffectiveness < 1){
            yield return dialogBox.TypeDialog("It's not very effective...");
        }
    }

    public void HandleUpdate(){
        if (state == BattleState.PlayerAction){
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove){
            HandleMoveSelection();
        }
    }

    void HandleActionSelection(){
        if(controls.Main.Down.WasPerformedThisFrame()){
            if(currentAction < 1){
                ++currentAction;
            }
        }
        else if (controls.Main.Up.WasPerformedThisFrame()){
            if (currentAction > 0){
                --currentAction;
            }
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (controls.Main.Interact.WasPerformedThisFrame()){
            if (currentAction == 0){
                // Fight
                PlayerMove();
            }
            else if (currentAction == 1){
                // Run
            }
        }
    }

    void HandleMoveSelection(){
        if(controls.Main.Right.WasPerformedThisFrame()){
            if(currentMove < playerUnit.Pokemon.Moves.Count - 1){
                ++currentMove;
            }
        }
        else if (controls.Main.Left.WasPerformedThisFrame()){
            if (currentMove > 0){
                --currentMove;
            }
        }
        else if (controls.Main.Down.WasPerformedThisFrame()){
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2){
                currentMove += 2;
            }
        }
        else if (controls.Main.Up.WasPerformedThisFrame()){
            if (currentMove > 1){
                currentMove -= 2;
            }
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (controls.Main.Interact.WasPerformedThisFrame()){
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}
