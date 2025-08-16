using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterChoice : MonoBehaviour, Interactable
{
    [SerializeField] PokemonTextEntry starterPokemon;

    Pokemon starter;
    public Pokemon Starter => starter;

    public bool UseRandomStarter = false;
    private int randomIndex;
    [SerializeField] List<PokemonTextEntry> randomStarterPokemon;

    void Start()
    {
        StartCoroutine(InitStarterChoice());
    }

    public IEnumerator InitStarterChoice()
    {
        yield return new WaitUntil(() => PokemonLoader.instance.isLoaded == true);

        randomIndex = UnityEngine.Random.Range(0, randomStarterPokemon.Count);
        SetStarter();
    }

    private void SetStarter()
    {
        if (!UseRandomStarter)
        {
            starter = PokemonTextEntryExtensions.TextEntryToPokemon(starterPokemon);
        }
        else
        {
            starter = PokemonTextEntryExtensions.TextEntryToPokemon(randomStarterPokemon[randomIndex]);
        }
    }

    public void Interact(Transform initiator)
    {
        if (starter != null)
        {
            StarterHandler.instance.SetStarterChoice(this);
            StarterHandler.instance.OpenStarterChoice();
        }
        else Debug.LogError("Starter is null or empty.");
    }
}
