using System.Collections;
using System.Collections.Generic;
using PsLib;
using UnityEngine;

public class PokemonLoader : MonoBehaviour
{
    PokemonDataServer server;
    string json = null;

    // Private dictionaries to store before moving to the database
    private Dictionary<string, PokemonBase> tempPokemonsByName;
    private Dictionary<int, PokemonBase> tempPokemonsByDexNum;

    private const int NUM_OF_POKEMON = 1000;
    private float startTime;

    public bool isLoaded = false;
    public bool GrowthRatesLoaded = false;
    public bool CatchRatesLoaded = false;
    public bool LearnsetsLoaded = false;
    public bool DexNumbersLoaded = false;
    public bool EvolutionsLoaded = false;
    public bool SpritesLoaded = false;
    public bool BackSpritesLoaded = false;
    public bool FoundLocationsLoaded = false;
    public bool ExpYieldLoaded = false;
    public bool FlavorTextsLoaded = false;
    public static PokemonLoader instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        isLoaded = false;
        GrowthRatesLoaded = false;
        CatchRatesLoaded = false;
        LearnsetsLoaded = false;
        DexNumbersLoaded = false;
        EvolutionsLoaded = false;
        SpritesLoaded = false;
        BackSpritesLoaded = false;
        FoundLocationsLoaded = false;
        ExpYieldLoaded = false;
        FlavorTextsLoaded = false;
        StartCoroutine(LoadPokemon());
    }

    IEnumerator LoadPokemon()
    {
        Debug.Log("Start loading growth rates");
        GrowthRateLoader.LoadGrowthRates();

        yield return new WaitUntil(() => GrowthRatesLoaded == true);

        Debug.Log("Start loading catch rates");
        CatchRatesLoader.LoadCatchRates();

        yield return new WaitUntil(() => CatchRatesLoaded == true);

        Debug.Log("Start Loading Movesets");
        LearnsetLoader.LoadLearnsets();

        yield return new WaitUntil(() => LearnsetsLoaded == true);

        Debug.Log("Start Loading Dex Numbers");
        PokedexNumberLoader.LoadDexNumbers();

        yield return new WaitUntil(() => DexNumbersLoaded == true);

        Debug.Log("Start Loading Evolutions");
        EvolutionsLoader.LoadEvolutions();

        yield return new WaitUntil(() => EvolutionsLoaded == true);

        Debug.Log("Start Loading Sprites");
        PSpriteLoader.LoadAllFrontSprites();

        yield return new WaitUntil(() => SpritesLoaded == true);

        Debug.Log("Start Loading Back Sprites");
        PSpriteLoader.LoadAllBackSprites();

        yield return new WaitUntil(() => BackSpritesLoaded == true);

        Debug.Log("Start Loading Found Locations");
        PokemonFoundLocationsLoader.LoadFoundLocations();

        yield return new WaitUntil(() => FoundLocationsLoaded == true);

        Debug.Log("Start Loading Exp Yields");
        ExpYieldLoader.LoadXPYields();

        yield return new WaitUntil(() => ExpYieldLoaded == true);

        Debug.Log("Start Loading Flavor Texts");
        PokemonFlavorTextLoader.LoadFlavorTexts();

        yield return new WaitUntil(() => FlavorTextsLoaded == true);

        Debug.Log("Pokemon Loader start");
        tempPokemonsByName = new Dictionary<string, PokemonBase>();
        tempPokemonsByDexNum = new Dictionary<int, PokemonBase>();

        server = new PokemonDataServer();
        server.Start();

        startTime = Time.time;

        for (int i = 1; i < NUM_OF_POKEMON; i++)
        {
            server.WriteLine($"getByNum {i}");
        }
        server.WriteLine($"getByName vulpixalola");
        server.WriteLine($"getByName ninetalesalola");
    }

    void Update()
    {
        if (server == null || !server.TryGetOutput(out string output)) return;

        json = output;

        //Debug.Log($"Raw JSON received: {json}");

        PsLib.Dex.Pokemon p = PsLib.Dex.Pokemon.deserialize(json);

        if (p == null)
        {
            Debug.LogError("Failed to deserialize Pokemon JSON");
            return;
        }

        PokemonBase pb = PokemonDB.ConvertToPokemonBase(p);
        tempPokemonsByName[PokemonDB.FixWeirdPokemonNames(pb.PokemonName.ToLower())] = pb;
        tempPokemonsByDexNum[pb.UniversalDexNumber] = pb;
        //Debug.Log($"Loaded Pokemon: {pb.PokemonName}, Num: {pb.UniversalDexNumber}");

        if (tempPokemonsByName.Count == NUM_OF_POKEMON + 1) // Accounts for alolan variants
        {
            float LoadTime = Time.time - startTime;

            Debug.Log($"All pokemon loaded in {LoadTime} seconds!");
            PokemonDB.Init(tempPokemonsByName, tempPokemonsByDexNum);

            server.WriteLine("exit");

            PokedexManager.instance.Init();
            PokemonParty.GetPlayerParty().InitializeParty();

            isLoaded = true;
        }
    }
}
