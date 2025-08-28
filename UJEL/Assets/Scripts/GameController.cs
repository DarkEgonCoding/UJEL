using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum GameState { FreeRoam, Battle, Dialog, Pause, Trainer, Menu, Cutscene, Evolution, PC, Shop, ChooseStarter }

public enum MenuState { Main, Pokemon, Bag, PartyOption, Pokedex, Map }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] Camera battleCamera;
    [SerializeField] AudioClip wildBattleMusic;

    // Debug checks whenever state is changed
    private GameState _state;
    public GameState state
    {
        get => _state;
        set
        {
            Debug.Log($"GameState changed: {_state} → {value}");
            _state = value;
        }
    }

    GameState stateBeforePause;

    MenuState visualMenuState;
    MenuState menuState
    { get => visualMenuState;
        set
        {
            Debug.Log($"MenuState changed: {visualMenuState} → {value}");
            visualMenuState = value;
        }
    }

    public PlayerControls controls;
    public static GameController instance;
    [SerializeField] GameObject menu;
    [SerializeField] List<TextMeshProUGUI> menuItems;
    [SerializeField] PartyScreen settingsPartyScreen;
    PokemonParty playerParty;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] PokedexUIManager pokedexUIManager;
    [SerializeField] Canvas UICanvas;
    [SerializeField] GameObject MapUI;
    [SerializeField] public EncounterZone legendaryEncounter;
    int selectedMenuItem = 0;
    int currentPartyMember;
    private string pressed;

    private void Awake()
    {
        controls = new PlayerControls();
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        MoveDB.Init();
        ConditionsDB.Init();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        if (instance == null) instance = this;
        playerController.OnEncountered.AddListener(() => StartCoroutine(StartBattle()));
        battleSystem.OnBattleOver.AddListener(EndBattle);
        UICanvas.gameObject.SetActive(false);

        //Bag
        controls.Main.C.performed += ctx => OpenMenu();

        //Save and Load --- TEMPORARY
        //controls.Main.Save.performed += ctx => Save();
        //controls.Main.Load.performed += ctx => Load();

        EvolutionManager.instance.OnStartEvolution += () => state = GameState.Evolution;
        EvolutionManager.instance.OnCompleteEvolution += () => state = GameState.FreeRoam;
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        var playerParty = playerController.GetComponent<PokemonParty>();
        StartCoroutine(playerParty.CheckForEvolutions());
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            MenuHandleUpdate();
        }
        else if (state == GameState.PC)
        {
            PCBox.instance.HandlePCUpdate();
        }
        else if (state == GameState.Shop)
        {
            ShopController.instance.HandleUpdate();
        }
        else if (state == GameState.ChooseStarter)
        {
            StarterHandler.instance.HandleUpdate();
        }
    }

    public void StartCutsceneState()
    {
        state = GameState.Cutscene;
    }

    public void StartFreeRoamState()
    {
        state = GameState.FreeRoam;
    }

    IEnumerator StartBattle()
    {
        state = GameState.Battle;

        AudioManager.instance.PlayMusic(wildBattleMusic, startSeconds: .5f);

        ScreenTransition transition = worldCamera.GetComponent<ScreenTransition>();
        transition.Reversed = false;

        yield return StartCoroutine(transition.TransitionAnimation());

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();

        var wildPokemonCopy = currentEncounterZone.GetRandomWildPokemon();
        //var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        //var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartBattle(playerParty, wildPokemonCopy);

        transition = battleCamera.GetComponent<ScreenTransition>();
        transition.Reversed = true;
        yield return StartCoroutine(transition.TransitionAnimation());

        yield return battleSystem.EnterPokemon();
    }

    public IEnumerator StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;

        MoneyHandler.instance.SetMoneyWager(trainer.MoneyForWin);

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

    public void PauseGame(bool pause, GameState? resumeState = null)
    {
        if (pause) // If pausing the game, set to pause
        {
            if (state == GameState.Pause) return;

            stateBeforePause = state;
            state = GameState.Pause;
        }
        else
        {
            // If there is a resume state, do it. Else, go to the state before pausing.
            state = resumeState ?? stateBeforePause;
        }
    }
    
    public void OpenMenu()
    {
        // Don't open the menu if you haven't gotten a starter yet.
        // This can be disabled for testing.
        if (!GameFlags.Instance.HasFlag("HasStarter"))
        {
            Debug.LogWarning("You don't have the first starter yet.");
            return;
        }

        if (state == GameState.FreeRoam)
        {
            state = GameState.Menu;
            menuState = MenuState.Main;
            menu.SetActive(true);
            UpdateItemSelection();
        }
    }

    public void MenuHandleUpdate()
    {
        // Main Menu Update
        if (menuState == MenuState.Main)
        {
            int prevSelection = selectedMenuItem;

            if (controls.Main.Down.WasPerformedThisFrame())
            {
                ++selectedMenuItem;
            }
            else if (controls.Main.Up.WasPerformedThisFrame())
            {
                --selectedMenuItem;
            }

            selectedMenuItem = Mathf.Clamp(selectedMenuItem, 0, menuItems.Count - 1);
            if (prevSelection != selectedMenuItem) UpdateItemSelection();

            if (controls.Main.Interact.WasPerformedThisFrame())
            {
                menu.SetActive(false);
                StartCoroutine(OnMenuSelected(selectedMenuItem));
            }
            if (controls.Main.Run.WasPerformedThisFrame())
            {
                menu.SetActive(false);
                state = GameState.FreeRoam;
            }
        }

        // Pokemon Update
        if (menuState == MenuState.Pokemon)
        {
            if (controls.Main.Right.WasPerformedThisFrame())
            {
                ++currentPartyMember;
            }
            else if (controls.Main.Left.WasPerformedThisFrame())
            {
                --currentPartyMember;
            }
            else if (controls.Main.Down.WasPerformedThisFrame())
            {
                currentPartyMember += 2;
            }
            else if (controls.Main.Up.WasPerformedThisFrame())
            {
                currentPartyMember -= 2;
            }

            currentPartyMember = Mathf.Clamp(currentPartyMember, 0, playerParty.Pokemons.Count - 1);

            settingsPartyScreen.UpdateMemberSelection(currentPartyMember);

            if (controls.Main.Interact.WasPerformedThisFrame())
            {
                // Show the context menu
                SwitchPokemon();
                /*
                var slotPos = settingsPartyScreen.GetSlotPosition(currentPartyMember);
                settingsPartyScreen.contextMenu.Show(slotPos, OnPartyOptionSelected);
                menuState = MenuState.PartyOption;
                */
            }
            if (controls.Main.Run.WasPerformedThisFrame())
            {
                settingsPartyScreen.gameObject.SetActive(false);
                menuState = MenuState.Main;
                menu.SetActive(true);
                UpdateItemSelection();
            }
        }

        if (menuState == MenuState.PartyOption)
        {
            if (controls.Main.Interact.WasPerformedThisFrame())
            {
                pressed = "z";
            }
            if (controls.Main.Run.WasPerformedThisFrame())
            {
                pressed = "x";
            }
            else if (controls.Main.Down.WasPerformedThisFrame())
            {
                pressed = "down";
            }
            else if (controls.Main.Up.WasPerformedThisFrame())
            {
                pressed = "up";
            }

            settingsPartyScreen.contextMenu.HandleUpdate(pressed);
        }

        // Bag Update
        if (menuState == MenuState.Bag)
        {
            inventoryUI.HandleBagUpdate(BagReturn);
        }

        if (menuState == MenuState.Pokedex)
        {
            pokedexUIManager.HandleUpdate(PokedexReturn);
        }

        if (menuState == MenuState.Map)
        {
            MapController.instance.HandleUpdate(() =>
            {
                StartCoroutine(LeaveMap());
            });
        }
    }

    IEnumerator LeaveMap()
    {
        menuState = MenuState.Main;
        yield return new WaitForEndOfFrame();
        UICanvas.gameObject.SetActive(false);
        MapUI.SetActive(false);
        menu.SetActive(true);
    }

    public void DisableMap()
    {
        UICanvas.gameObject.SetActive(false);
        MapUI.SetActive(false);
    }

    void PokedexReturn()
    {
        UICanvas.gameObject.SetActive(false);
        pokedexUIManager.gameObject.SetActive(false);
        menuState = MenuState.Main;
        menu.SetActive(true);
        UpdateItemSelection();   
    }

    void BagReturn()
    {
        UICanvas.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
        menuState = MenuState.Main;
        menu.SetActive(true);
        UpdateItemSelection();
    }

    void OnPartyOptionSelected(int selectedIndex)
    {
        settingsPartyScreen.contextMenu.Hide();

        switch (selectedIndex)
        {
            case 0: // Move
                //isSwitchingPokemon = true;
                //pokemonToSwitch = currentPartyMember;
                break;

            case 1: // Give Item
                Debug.Log("TODO: Give item UI");
                break;

            case 2: // Cancel
                break;
        }

        menuState = MenuState.Pokemon;
    }

    private int switchSourceIndex = -1; // -1 means no source selected
    private void SwitchPokemon()
    {
        if (switchSourceIndex == -1)
        {
            // First selection
            switchSourceIndex = currentPartyMember;
            Debug.Log($"Selected {playerParty.Pokemons[switchSourceIndex].Base.PokemonName} for switching.");
        }
        else if (switchSourceIndex == currentPartyMember)
        {
            // Cancel if the same is selected again
            Debug.Log("Cancelled switch.");
            switchSourceIndex = -1;
        }
        else
        {
            // Perform switch
            var temp = playerParty.Pokemons[switchSourceIndex];
            playerParty.Pokemons[switchSourceIndex] = playerParty.Pokemons[currentPartyMember];
            playerParty.Pokemons[currentPartyMember] = temp;

            Debug.Log($"Switched {playerParty.Pokemons[currentPartyMember].Base.PokemonName} with {playerParty.Pokemons[switchSourceIndex].Base.PokemonName}");

            // Refresh UI
            settingsPartyScreen.SetPartyData();
            settingsPartyScreen.UpdateMemberSelection(currentPartyMember);

            switchSourceIndex = -1;
        }
    }

    private IEnumerator OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            // Party
            playerParty = playerController.GetComponent<PokemonParty>();
            settingsPartyScreen.Init();
            settingsPartyScreen.SetPartyData();
            settingsPartyScreen.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            menuState = MenuState.Pokemon;
        }
        else if (selectedItem == 1) // Pokedex
        {
            UICanvas.gameObject.SetActive(true);
            pokedexUIManager.gameObject.SetActive(true);
            pokedexUIManager.justOpenedPokedex = true;
            pokedexUIManager.RefreshPokedex();
            menuState = MenuState.Pokedex;
        }
        else if (selectedItem == 2)
        {
            // Bag
            UICanvas.gameObject.SetActive(true);
            inventoryUI.gameObject.SetActive(true);
            inventoryUI.justOpenedBag = true;
            menuState = MenuState.Bag;
        }
        else if (selectedItem == 3)
        {
            // Map
            menuState = MenuState.Map;
            UICanvas.gameObject.SetActive(true);
            MapUI.SetActive(true);
        }
        else if (selectedItem == 4)
        {
            // Save
            Save();
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 5)
        {
            // Load
            Load();
            state = GameState.FreeRoam;
        }
    }

    public void SetUICanvas(bool Active)
    {
        if (Active) UICanvas.gameObject.SetActive(true);
        else UICanvas.gameObject.SetActive(false);
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedMenuItem)
            {
                menuItems[i].faceColor = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                menuItems[i].faceColor = Color.black;
            }
        }
    }

    public void Save()
    {
        if (state == GameState.Menu)
        {
            SavingSystem.i.Save("saveSlot1");
        }
    }

    public void Load()
    {
        if (state == GameState.Menu)
        {
            SavingSystem.i.Load("saveSlot1");
        }
    }

    public EncounterZone currentEncounterZone;
    public void SetCurrentEncounterZone(EncounterZone zone)
    {
        currentEncounterZone = zone;
    }
}
