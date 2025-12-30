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

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, MoveForget, EndingBattle}

public class BattleSystemCopy : MonoBehaviour
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
    [SerializeField] MoveSelectionUI moveSelectionUI;

    PokeballItem templatePokeball;
    BattleQueue battleQueue = new BattleQueue();

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
    MoveBase moveToLearn;
    public bool didLearnMove;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void Start()
    {
        OnBattleOver.AddListener(BattleOver);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

// ---------------------------------------------- Battle Init -------------------------------------------------

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        dialogBox.SetDialog("");
        isTrainerBattle = false;
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        dialogBox.SetDialog("");
        isTrainerBattle = true;
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
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
            yield return dialogBox.StartDialog($"{trainer.Name} sent out {enemyPokemon.Base.PokemonName}!");
            yield return new WaitForSeconds(1.3f);
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
            yield return StartCoroutine(ActionSelection());
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
        yield return dialogBox.StartDialog($"{trainer.Name} wants to battle!");
        yield return new WaitForSeconds(1.3f);
    }

    public IEnumerator EnterPokemon()
    {
        enemyUnit.PlayEnterAnimation();
        playerUnit.PlayEnterAnimation();

        yield return dialogBox.StartDialog($"A wild {enemyUnit.Pokemon.Base.PokemonName} appeared!");
        yield return new WaitForSeconds(1f);

        // Note that the playerUnit and enemyUnit can be used to tell the server which pokemon are starting
        string packedOne = PokemonPacker.Pack(playerUnit.Pokemon); // TODO: Use the entire party packer if possible
        string packedTwo = PokemonPacker.Pack(enemyUnit.Pokemon);
        //battleQueue.Init();
    }

// ------------------------------------------------------------------------------------------------------

    private IEnumerator ActionSelection(bool overrideActive = false)
    {
        if (!dialogBox.actionSelector.activeInHierarchy || overrideActive)
        {
            state = BattleState.ActionSelection;
            yield return dialogBox.StartDialog($"What should {playerUnit.Pokemon.Base.PokemonName} do?");
            yield return new WaitForSeconds(0.5f);
            dialogBox.EnableActionSelector(true);
        }
    }

    private void OpenPartyScreen()
    {
        dialogBox.EnableActionSelector(false);
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData();
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        if (dialogBox.actionSelector.activeSelf)
        {
            state = BattleState.MoveSelection;
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            dialogBox.EnableMoveSelector(true);
        }
    }

    IEnumerator PlayerMove()
    {
        if (dialogBox.isTyping) yield break;
        state = BattleState.Busy;

        var move = playerUnit.Pokemon.Moves[currentMove];
        dialogBox.StartDialog($"{playerUnit.Pokemon.Base.PokemonName} used {move.Base.Name}!");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();

        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        move.PP--;

        if (damageDetails.Fainted)
        {
            yield return enemyHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            dialogBox.StartDialog($"{enemyUnit.Pokemon.Base.PokemonName} fainted!");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            if (!isTrainerBattle)
            {
                yield return GainExperience();
                yield return new WaitForSeconds(0.5f);
                OnBattleOver.Invoke(true);
            }
            else
            {
                yield return GainExperience();
                yield return new WaitForSeconds(0.5f);
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    StartCoroutine(SendNextTrainerPokemon(nextPokemon));
                }
                else
                {
                    OnBattleOver.Invoke(true);
                }
            }
        }
        else
        {
            yield return enemyHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
            yield return StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        yield return null;
        /*
                if (dialogBox.isTyping) yield break;
                state = BattleState.PerformMove;

                //Select Move
                var move = enemyUnit.Pokemon.GetRandomMove();

                dialogBox.StartDialog($"Enemy {enemyUnit.Pokemon.Base.PokemonName} used {move.Base.Name}!");
                enemyUnit.PlayAttackAnimation();
                yield return new WaitForSeconds(1f);

                playerUnit.PlayHitAnimation();

                var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
                move.PP--;

                if (damageDetails.Fainted)
                {
                    yield return playerHud.UpdateHP();
                    yield return ShowDamageDetails(damageDetails);
                    dialogBox.StartDialog($"{playerUnit.Pokemon.Base.PokemonName} fainted!");
                    playerUnit.PlayFaintAnimation();

                    yield return new WaitForSeconds(2f);
                    var nextPokemon = playerParty.GetHealthyPokemon();
                    if (nextPokemon != null)
                    {
                        OpenPartyScreen();
                    }
                    else
                    {
                        OnBattleOver.Invoke(false);
                    }
                }
                else if (damageDetails.Fainted == false)
                {
                    yield return playerHud.UpdateHP();
                    yield return ShowDamageDetails(damageDetails);
                    yield return StartCoroutine(ActionSelection());
                }
        */
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.StartDialog("A critical hit!");
            yield return new WaitForSeconds(0.5f);
        }

        if (damageDetails.TypeEffectiveness > 1)
        {
            yield return dialogBox.StartDialog("It's super effective!");
            yield return new WaitForSeconds(0.5f);
        }
        else if (damageDetails.TypeEffectiveness < 1)
        {
            yield return dialogBox.StartDialog("It's not very effective...");
            yield return new WaitForSeconds(0.5f);
        }
    }

    public MoveBase learnedMove;
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.MoveForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == PokemonBase.MaxNumOfMoves)
                {
                    // Don't learn the new move
                    didLearnMove = false;
                }
                else
                {
                    // Forget the selected move and learn the new move
                    learnedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
                    playerUnit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                    didLearnMove = true;
                }

                state = BattleState.EndingBattle;
            };

            HandleForgetMoveSelection(onMoveSelected);
        }
    }

    void HandleActionSelection()
    {
        if (controls.Main.Down.WasPerformedThisFrame())
        {
            currentAction += 2;
        }
        else if (controls.Main.Up.WasPerformedThisFrame())
        {
            currentAction -= 2;
        }
        else if (controls.Main.Right.WasPerformedThisFrame())
        {
            ++currentAction;
        }
        else if (controls.Main.Left.WasPerformedThisFrame())
        {
            --currentAction;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (controls.Main.Interact.WasPerformedThisFrame())
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
                dialogBox.EnableActionSelector(false);

                Debug.LogWarning("Pokeballs are currently disabled because the function requires an input of what kind of pokeball was thrown.");
                templatePokeball = new PokeballItem();
                StartCoroutine(ThrowPokeball(templatePokeball));
            }
            else if (currentAction == 2)
            {
                // Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
                dialogBox.EnableActionSelector(false);
                StartCoroutine(TryToEscape());
            }
        }
    }

    void HandleMoveSelection()
    {
        if (controls.Main.Right.WasPerformedThisFrame())
        {
            ++currentMove;
        }
        else if (controls.Main.Left.WasPerformedThisFrame())
        {
            --currentMove;
        }
        else if (controls.Main.Down.WasPerformedThisFrame())
        {
            currentMove += 2;
        }
        else if (controls.Main.Up.WasPerformedThisFrame())
        {
            currentMove -= 2;
        }
        else if (controls.Main.Run.WasPerformedThisFrame())
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(ActionSelection());
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (controls.Main.Interact.WasPerformedThisFrame())
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
    }

    int currentMember;
    void HandlePartySelection()
    {
        if (controls.Main.Right.WasPerformedThisFrame())
        {
            ++currentMember;
        }
        else if (controls.Main.Left.WasPerformedThisFrame())
        {
            --currentMember;
        }
        else if (controls.Main.Down.WasPerformedThisFrame())
        {
            currentMember += 2;
        }
        else if (controls.Main.Up.WasPerformedThisFrame())
        {
            currentMember -= 2;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (controls.Main.Interact.WasPerformedThisFrame())
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            dialogBox.actionSelector.SetActive(false);
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (controls.Main.Run.WasPerformedThisFrame())
        {
            if (playerUnit.Pokemon.HP <= 0) return;

            partyScreen.gameObject.SetActive(false);
            StartCoroutine(ActionSelection(overrideActive: true));
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.StartDialog($"Come back {playerUnit.Pokemon.Base.PokemonName}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        playerHud.SetData(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.StartDialog($"Go {newPokemon.Base.PokemonName}!");
        playerUnit.PlayEnterAnimation();
        yield return new WaitForSeconds(1f);

        StartCoroutine(EnemyMove());
    }

    IEnumerator SendNextTrainerPokemon(Pokemon nextPokemon)
    {
        state = BattleState.Busy;
        enemyUnit.Setup(nextPokemon);
        enemyHud.SetData(nextPokemon);
        yield return dialogBox.StartDialog($"{trainer.Name} sent out {nextPokemon.Base.PokemonName}!");
        enemyUnit.PlayEnterAnimation();
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ActionSelection());
    }

    IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
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
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCatpureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon, pokeballItem);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokemon is Caught
            yield return dialogBox.StartDialog($"You caught a {enemyUnit.Pokemon.Base.PokemonName}!");
            yield return new WaitForSeconds(0.75f);

            // Set caught to true in the pokedex
            PokedexManager.instance.SetCaughtStatus(enemyUnit.Pokemon.Base, true);

            yield return GainExperience();
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            Destroy(pokeball);
            yield return new WaitForSeconds(.75f);

            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.StartDialog($"{enemyUnit.Pokemon.Base.PokemonName} has been added to your party.");
            yield return new WaitForSeconds(.5f);

            OnBattleOver.Invoke(true);
        }
        else
        {
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            yield return dialogBox.StartDialog($"{enemyUnit.Pokemon.Base.PokemonName} broke free!");

            Destroy(pokeball);
            yield return new WaitForSeconds(0.75f);

            yield return EnemyMove();
        }
    }

    int TryToCatchPokemon(Pokemon pokemon, PokeballItem pokeballItem)
    {
        var statusBonus = 1;
        float a = ((3f * pokemon.MaxHp - 2f * pokemon.HP) / (3f * pokemon.MaxHp)) * pokemon.Base.CatchRate * pokeballItem.CateRateModifier * statusBonus;

        if (a >= 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
            {
                return shakeCount;
            }

            ++shakeCount;
        }

        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.StartDialog("You can't run from trainer battles!");
            yield return new WaitForSeconds(0.75f);
            StartCoroutine(EnemyMove());
            yield break;
        }

        ++escapeAttempts;

        float playerSpeed = playerUnit.Pokemon.Speed;
        float enemySpeed = enemyUnit.Pokemon.Speed;

        if (enemySpeed < playerSpeed)
        { // If you are faster than the enemy
            var randomVal = UnityEngine.Random.Range(0, 101);
            if (randomVal < 10)
            { // 10% change to fail
                yield return dialogBox.StartDialog("Can't escape!");
                yield return new WaitForSeconds(0.75f);
                StartCoroutine(EnemyMove());
            }
            else
            { // Run success
                yield return dialogBox.StartDialog("Ran away safely!");
                yield return new WaitForSeconds(0.75f);
                OnBattleOver.Invoke(true);
            }
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            Debug.Log(f);

            var SecondRandomVal = UnityEngine.Random.Range(0, 256);
            Debug.Log(SecondRandomVal);

            if (SecondRandomVal < f)
            {
                yield return dialogBox.StartDialog("Ran away safely!");
                yield return new WaitForSeconds(0.75f);
                OnBattleOver.Invoke(true);
            }
            else
            {
                yield return dialogBox.StartDialog("Can't escape!");
                yield return new WaitForSeconds(0.75f);
                StartCoroutine(EnemyMove());
            }
        }
    }

    IEnumerator GainExperience()
    {
        // Exp Gain
        int expYield = enemyUnit.Pokemon.Base.ExpYield;
        int enemyLevel = enemyUnit.Pokemon.Level;
        float trainerBonus = (isTrainerBattle) ? 1.5f : 1;

        int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);

        playerUnit.Pokemon.Exp += expGain;
        yield return dialogBox.StartDialog($"{playerUnit.Pokemon.Base.PokemonName} gained {expGain} exp.");
        yield return new WaitForSeconds(0.75f);
        yield return playerHud.SetExpSmooth();
        yield return new WaitForSeconds(0.25f);

        var expSharePokemon = playerParty.GetExpShare();
        if (expSharePokemon == playerUnit.Pokemon) expSharePokemon = null;
        if (expSharePokemon != null)
        {
            var expShareGain = expGain / 2;
            expSharePokemon.Exp += expShareGain;
            yield return dialogBox.StartDialog($"{expSharePokemon.Base.PokemonName} gained {expShareGain} exp.");
            yield return new WaitForSeconds(0.75f);
        }  

        // Check Level Up
        while (playerUnit.Pokemon.CheckForLevelUp())
        {
            playerHud.SetLevel();
            yield return LevelUpPokemon(playerUnit.Pokemon);
            yield return playerHud.SetExpSmooth(true);
        }

        if (expSharePokemon != null)
        {
            while (expSharePokemon.CheckForLevelUp())
            {
                yield return LevelUpPokemon(expSharePokemon);
            }
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerable LevelUpPokemon(Pokemon pokemon)
    {
        yield return dialogBox.StartDialog($"{pokemon.Base.PokemonName} grew to level {pokemon.Level}!");
        yield return new WaitForSeconds(0.85f);

        // Try to learn a new move
        var newMove = pokemon.GetLearnableMoveAtCurrLevel();
        if (newMove != null)
        {
            if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
            {
                //playerUnit.Pokemon.LearnMove(newMove.Base);
                //yield return dialogBox.StartDialog($"{playerUnit.Pokemon.Base.PokemonName} learned {newMove.Base.Name}!");
                yield return new WaitForSeconds(0.85f);
                dialogBox.SetMoveNames(pokemon.Moves);
            }
            else
            {
                //yield return dialogBox.StartDialog($"{playerUnit.Pokemon.Base.PokemonName} is trying to learn {newMove.Base.Name}.");
                yield return new WaitForSeconds(0.5f);
                yield return dialogBox.StartDialog($"But it cannot learn more than {PokemonBase.MaxNumOfMoves} moves.");
                yield return new WaitForSeconds(0.5f);
                //yield return ChooseMoveToForget(playerUnit.Pokemon, newMove.Base);
                yield return new WaitUntil(() => state != BattleState.MoveForget);
                if (!didLearnMove)
                {
                    yield return dialogBox.StartDialog($"{pokemon.Base.PokemonName} did not learn {moveToLearn.Name}.");
                    yield return new WaitForSeconds(0.75f);
                }
                else if (didLearnMove)
                {
                    yield return dialogBox.StartDialog("1");
                    yield return new WaitForSeconds(1f);
                    yield return dialogBox.StartDialog("2");
                    yield return new WaitForSeconds(1f);
                    yield return dialogBox.StartDialog("3");
                    yield return new WaitForSeconds(1f);
                    yield return dialogBox.StartDialog("Poof!");
                    yield return new WaitForSeconds(1f);
                    yield return dialogBox.StartDialog($"{pokemon.Base.PokemonName} forgot {learnedMove.Name} and learned {moveToLearn.Name}!");
                    yield return new WaitForSeconds(0.75f);
                }
                learnedMove = null;
                moveToLearn = null;
                yield return new WaitForSeconds(2f);
            }
        }
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.StartDialog($"Forget a move?");
        yield return new WaitForSeconds(0.75f);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveForget;
    }

    int currentForgetMoveSelection = 0;
    public void HandleForgetMoveSelection(Action<int> onSelected)
    {
        if (controls.Main.Down.WasPerformedThisFrame())
        {
            ++currentForgetMoveSelection;
        }
        else if (controls.Main.Up.WasPerformedThisFrame())
        {
            --currentForgetMoveSelection;
        }

        currentForgetMoveSelection = Mathf.Clamp(currentForgetMoveSelection, 0, PokemonBase.MaxNumOfMoves);

        moveSelectionUI.UpdateForgetMoveSelection(currentForgetMoveSelection);

        if (controls.Main.Interact.WasPerformedThisFrame())
        {
            onSelected?.Invoke(currentForgetMoveSelection);
        }
    }

    public void BattleOver(bool won)
    {
        playerHud.ClearData();
        enemyHud.ClearData();

        if (isTrainerBattle)
        {
            if (won) MoneyHandler.instance.AddMoney(MoneyHandler.instance.CurrentMoneyWager);
            else MoneyHandler.instance.RemoveMoney(MoneyHandler.instance.CurrentMoneyWager / 2);

            MoneyHandler.instance.ResetMoneyWager();
        }
    }
}
