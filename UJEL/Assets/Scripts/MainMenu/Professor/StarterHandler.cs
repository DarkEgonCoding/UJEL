using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarterHandler : MonoBehaviour
{
    public static StarterHandler instance;

    [SerializeField] GameObject StarterChoiceUI;
    [SerializeField] TextMeshProUGUI StarterName;
    [SerializeField] TextMeshProUGUI Type1Txt;
    [SerializeField] TextMeshProUGUI Type2Txt;
    [SerializeField] Image StarterImage;

    StarterChoice starterChoice;
    private bool justOpenedStarter = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        justOpenedStarter = false;
    }

    public void SetStarterChoice(StarterChoice starterChoice)
    {
        this.starterChoice = starterChoice;
    }

    public void OpenStarterChoice()
    {
        GameController.instance.state = GameState.ChooseStarter;

        InitUI();

        GameController.instance.SetUICanvas(true);
        StarterChoiceUI.SetActive(true);
        justOpenedStarter = true;
    }

    private void CloseStarterChoice()
    {
        GameController.instance.SetUICanvas(false);
        StarterChoiceUI.SetActive(false);

        GameController.instance.state = GameState.FreeRoam;
        justOpenedStarter = false;
    }

    private void InitUI()
    {
        if (starterChoice == null) return;

        var pokemon = starterChoice.Starter;
        StarterName.text = pokemon.Base.PokemonName;
        StarterImage.sprite = pokemon.Base.FrontSprite;
        Type1Txt.text = pokemon.Base.type1.ToString();
        Type2Txt.text = pokemon.Base.type2.ToString();
    }

    public void HandleUpdate()
    {
        if (justOpenedStarter)
        {
            justOpenedStarter = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            CloseStarterChoice();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ChooseStarter();
        }
    }

    private void ChooseStarter()
    {
        GameFlags.Instance.SetFlag("HasStarter");

        var playerParty = PokemonParty.GetPlayerParty();
        playerParty.AddPokemon(starterChoice.Starter);

        Debug.Log($"The player now has: {starterChoice.Starter.Base.PokemonName}");
        starterChoice.Starter.DebugMoves();

        PokedexManager.instance.SetCaughtStatus(starterChoice.Starter.Base, true);
    }
}
