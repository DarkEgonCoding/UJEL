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

    public bool GrowthRatesLoaded = false;
    public bool CatchRatesLoaded = false;
    public bool LearnsetsLoaded = false;
    public static PokemonLoader instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        GrowthRatesLoaded = false;
        CatchRatesLoaded = false;
        LearnsetsLoaded = false;
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
        tempPokemonsByName[pb.PokemonName.ToLower()] = pb;
        tempPokemonsByDexNum[pb.UniversalDexNumber] = pb;
        //Debug.Log($"Loaded Pokemon: {pb.PokemonName}, Num: {pb.UniversalDexNumber}");

        if (tempPokemonsByName.Count == NUM_OF_POKEMON - 1)
        {
            float LoadTime = Time.time - startTime;

            Debug.Log($"All pokemon loaded in {LoadTime} seconds!");
            PokemonDB.Init(tempPokemonsByName, tempPokemonsByDexNum);

            server.WriteLine("exit");

            Debug.Log(PokemonDB.GetPokemonByDexNum(1).PokemonName + " " + PokemonDB.GetPokemonByDexNum(1).GrowthRate + " " + PokemonDB.GetPokemonByDexNum(1).CatchRate);
            Debug.Log(PokemonDB.GetPokemonByDexNum(2).PokemonName + " " + PokemonDB.GetPokemonByDexNum(2).GrowthRate + " " + PokemonDB.GetPokemonByDexNum(2).CatchRate);
            Debug.Log(PokemonDB.GetPokemonByDexNum(3).PokemonName + " " + PokemonDB.GetPokemonByDexNum(3).GrowthRate + " " + PokemonDB.GetPokemonByDexNum(3).CatchRate);
            Debug.Log(PokemonDB.GetPokemonByName("pikachu").PokemonName + " " + PokemonDB.GetPokemonByName("pikachu").GrowthRate + " " + PokemonDB.GetPokemonByName("pikachu").CatchRate);
        }
    }
}
