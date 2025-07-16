using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum GameState { FreeRoam, Battle, Dialog, Pause, Trainer, Menu, Cutscene}

public enum MenuState { Main, Pokemon, Bag}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] Camera battleCamera;
    [SerializeField] AudioClip wildBattleMusic;
    public GameState state;
    public GameState previousState;
    GameState stateBeforePause;
    MenuState menuState;
    public PlayerControls controls;
    public static GameController instance;
    [SerializeField] GameObject menu;
    [SerializeField] List<TextMeshProUGUI> menuItems;
    [SerializeField] PartyScreen settingsPartyScreen;
    PokemonParty playerParty;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] Canvas UICanvas;
    int selectedMenuItem = 0;
    int currentPartyMember;

    private void Awake()
    {
        controls = new PlayerControls();
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        PokemonDB.Init();
        MoveDB.Init();
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
        instance = this;
        playerController.OnEncountered.AddListener(() => StartCoroutine(StartBattle()));
        battleSystem.OnBattleOver.AddListener(EndBattle);
        UICanvas.gameObject.SetActive(false);

        //Bag
        controls.Main.C.performed += ctx => OpenMenu();

        //Save and Load --- TEMPORARY
        //controls.Main.Save.performed += ctx => Save();
        //controls.Main.Load.performed += ctx => Load();
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
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

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePause = state;
            state = GameState.Pause;
        }
        else
        {
            state = stateBeforePause;
        }
    }
    public void OpenMenu()
    {
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
                // Switch around pokemon and stuff
                Debug.Log("you haven't done this yet...");
            }
            if (controls.Main.Run.WasPerformedThisFrame())
            {
                settingsPartyScreen.gameObject.SetActive(false);
                menuState = MenuState.Main;
                menu.SetActive(true);
                UpdateItemSelection();
            }
        }

        // Bag Update
        if (menuState == MenuState.Bag)
        {
            inventoryUI.HandleBagUpdate();
            if (controls.Main.Interact.WasPerformedThisFrame())
            {
                Debug.Log("you haven't done this yet...");
            }
            if (controls.Main.Run.WasPerformedThisFrame())
            {
                UICanvas.gameObject.SetActive(true);
                inventoryUI.gameObject.SetActive(false);
                menuState = MenuState.Main;
                menu.SetActive(true);
                UpdateItemSelection();
            }
        }
    }

    private IEnumerator OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            // Party
            playerParty = playerController.GetComponent<PokemonParty>();
            settingsPartyScreen.Init();
            settingsPartyScreen.SetPartyData(playerParty.Pokemons);
            settingsPartyScreen.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            menuState = MenuState.Pokemon;
        }
        else if (selectedItem == 1)
        {
            // Bag
            UICanvas.gameObject.SetActive(true);
            inventoryUI.gameObject.SetActive(true);
            menuState = MenuState.Bag;
        }
        else if (selectedItem == 2)
        {
            // Save
            Save();
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            // Load
            Load();
            state = GameState.FreeRoam;
        }
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
