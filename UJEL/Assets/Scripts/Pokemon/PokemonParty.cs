using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    List<Pokemon> pokemons;

    [SerializeField] List<PokemonTextEntry> pokemonTextEntries;

    public event Action OnUpdated;

    const int MAX_POKEMON = 6;

    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;
        }
        set
        {
            pokemons = value;
            OnUpdated?.Invoke();
        }
    }

    public void InitializeParty()
    {
        pokemons = new List<Pokemon>();

        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }

        foreach (var pokemonText in pokemonTextEntries)
        {
            var createdPokemon = PokemonTextEntryExtensions.TextEntryToPokemon(pokemonText);
            AddPokemon(createdPokemon);
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < MAX_POKEMON)
        {
            pokemons.Add(newPokemon);

            OnUpdated?.Invoke();
        }
        else
        {
            PCBox.instance.AddPokemonToPC(newPokemon);
        }
    }

    public void RemovePokemon(int index)
    {
        if (pokemons.Count > 1 && index >= 0 && index < pokemons.Count)
        {
            pokemons.RemoveAt(index);
        }
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach (var pokemon in pokemons)
        {
            var evolution = pokemon.CheckForEvolution();
            if (evolution != null)
            {
                yield return EvolutionManager.instance.Evolve(pokemon, evolution);
            }
        }

        OnUpdated?.Invoke();
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }

    public Pokemon GetExpShare()
    {
        foreach (var pokemon in pokemons)
        {
            if (pokemon.HeldItem.Name == "exp_share")
            {
                return pokemon;
            }
        }

        return null;
    }
}
