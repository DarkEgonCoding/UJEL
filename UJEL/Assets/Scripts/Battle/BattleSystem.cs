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

    [SerializeField] GameObject pokeballSprite;

    BattleState state;
    int currentAction;
    int currentMove;
    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;
    PlayerController player;
    TrainerController trainer;

    bool isTrainerBattle = false;
    int escapeAttempts;

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
        player = playerParty.GetComponent<PlayerController>();
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
        escapeAttempts = 0;
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
                yield return GainExperience();
                yield return new WaitForSeconds(0.5f);
                OnBattleOver.Invoke(true);
            }
            else {
                yield return GainExperience();
                yield return new WaitForSeconds(0.5f);
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
                dialogBox.EnableActionSelector(false);
                StartCoroutine(ThrowPokeball());
            }
            else if (currentAction == 2){
                // Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 3){
                // Run
                dialogBox.EnableActionSelector(false);
                StartCoroutine(TryToEscape());
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

    IEnumerator ThrowPokeball(){
        state = BattleState.Busy;

        if (isTrainerBattle){
            yield return dialogBox.StartDialog($"You can't capture the trainer's pokemon!");
            yield return new WaitForSeconds(.5f);
            yield return EnemyMove();
            yield break;
        }

        yield return dialogBox.StartDialog($"{player.Name} used POKEBALL!");
        yield return new WaitForSeconds(1f);

        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        // Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0,2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCatpureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon);

        for (int i=0; i< Mathf.Min(shakeCount, 3); i++){
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4){
            // Pokemon is Caught
            yield return dialogBox.StartDialog($"You caught this loser -> {enemyUnit.Pokemon.Base.Name}");
            yield return GainExperience();
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            Destroy(pokeball);
            yield return new WaitForSeconds(.75f);
            
            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.StartDialog($"{enemyUnit.Pokemon.Base.Name} has been added to your party.");
            yield return new WaitForSeconds(.5f);

            OnBattleOver.Invoke(true);   
        }
        else{
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball. DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            yield return dialogBox.StartDialog($"{enemyUnit.Pokemon.Base.Name} broke free!");

            Destroy(pokeball);
            yield return new WaitForSeconds(0.75f);

            yield return EnemyMove();
        }
    }

    int TryToCatchPokemon(Pokemon pokemon){
        var statusBonus = 1;
        float a = ((3f * pokemon.MaxHp - 2f * pokemon.HP)/ (3f * pokemon.MaxHp)) * pokemon.Base.CatchRate * statusBonus;

        if (a >= 255){
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4){
            if (UnityEngine.Random.Range(0, 65535) >= b){
                return shakeCount;
            }

            ++shakeCount;
        }

        return shakeCount;
    }

    IEnumerator TryToEscape(){
        state = BattleState.Busy;

        if (isTrainerBattle){
            yield return dialogBox.StartDialog("You can't run from trainer battles!");
            yield return new WaitForSeconds(0.75f);
            StartCoroutine(EnemyMove());
            yield break;
        }

        ++escapeAttempts;

        float playerSpeed = playerUnit.Pokemon.Speed;
        float enemySpeed = enemyUnit.Pokemon.Speed;

        if (enemySpeed < playerSpeed){ // If you are faster than the enemy
            var randomVal = UnityEngine.Random.Range(0, 101);
            if (randomVal < 10){ // 10% change to fail
                yield return dialogBox.StartDialog("Can't escape!");
                yield return new WaitForSeconds(0.75f);
                StartCoroutine(EnemyMove());
            }
            else{ // Run success
                yield return dialogBox.StartDialog("Ran away safely!");
                yield return new WaitForSeconds(0.75f);
                OnBattleOver.Invoke(true);
            }
        }
        else{
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            Debug.Log(f);

            var SecondRandomVal = UnityEngine.Random.Range(0, 256);
            Debug.Log(SecondRandomVal);

            if (SecondRandomVal < f){
                yield return dialogBox.StartDialog("Ran away safely!");
                yield return new WaitForSeconds(0.75f);
                OnBattleOver.Invoke(true);
            }
            else{
                yield return dialogBox.StartDialog("Can't escape!");
                yield return new WaitForSeconds(0.75f);
                StartCoroutine(EnemyMove());
            }
        }
    }

    IEnumerator GainExperience(){
        // Exp Gain
        int expYield = enemyUnit.Pokemon.Base.ExpYield;
        int enemyLevel = enemyUnit.Pokemon.Level;
        float trainerBonus = (isTrainerBattle)? 1.5f : 1;

        int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
        playerUnit.Pokemon.Exp += expGain;
        yield return dialogBox.StartDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp.");
        yield return new WaitForSeconds(0.75f);
        yield return playerHud.SetExpSmooth();
        yield return new WaitForSeconds(0.25f);

        // Check Level Up
        while (playerUnit.Pokemon.CheckForLevelUp()){
            playerHud.SetLevel();
            yield return dialogBox.StartDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}!");

            yield return playerHud.SetExpSmooth(true);
        }

        yield return new WaitForSeconds(1f);
    }
}
