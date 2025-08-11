using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;
    [SerializeField] Canvas UICanvas;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;

    public static EvolutionManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        UICanvas.gameObject.SetActive(true);
        evolutionUI.SetActive(true);

        pokemonImage.sprite = pokemon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} is evolving!");

        var oldPokemon = pokemon.Base;
        pokemon.Evolve(evolution);

        pokemonImage.sprite = pokemon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{oldPokemon.PokemonName} evolved into {pokemon.Base.PokemonName}!");

        UICanvas.gameObject.SetActive(false);
        evolutionUI.SetActive(false);
        OnCompleteEvolution?.Invoke();
    }
}
