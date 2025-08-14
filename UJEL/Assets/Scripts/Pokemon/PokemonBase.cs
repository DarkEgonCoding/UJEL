using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string pokemonName;
    [SerializeField] int pokedexNumber;
    [SerializeField] int universalDexNumber;
    [TextArea]
    [SerializeField] string description;
    [Tooltip("The locations where you can find this pokemon. Ex) Route 1, 4...")]
    [SerializeField] string foundLocations;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] public PokemonType type1;
    [SerializeField] public PokemonType type2;
    Dictionary<string, string> abilityDictionary;
    List<string> abilities;
    double weightkg;
    double heightm;
    string color;

    // Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    [SerializeField] int catchRate; // A value from 3-255 -> the higher it is the easier it is to catch

    [SerializeField] List<MoveLearn> levelUpMoves;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    // OLD LEARNING MOVES
    // [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;
    [SerializeField] List<Evolution> evolutions;
    public static int MaxNumOfMoves { get; set; } = 4;

    /// <summary>
    /// Init function called to set the values of a pokemonBase
    /// </summary>
    public void Init(int num, string name, List<string> types, int hp, int atk, int def, int spa, int spd, int spe, Dictionary<string, string> abilities, double heightm, double weightkg, string color)
    {
        this.universalDexNumber = num;
        SetInGamePokedexNumber(name);
        this.pokemonName = name;
        SetTypes(types);
        this.maxHp = hp;
        this.attack = atk;
        this.defense = def;
        this.spAttack = spa;
        this.spDefense = spd;
        this.speed = spe;

        // Abilities
        this.abilityDictionary = abilities;
        this.abilities = GetAbilityList(abilities);

        growthRate = GrowthRateLoader.GetGrowthRate(this.universalDexNumber);
        this.catchRate = CatchRatesLoader.GetCatchRate(this.universalDexNumber);
        this.heightm = heightm;
        this.weightkg = weightkg;
        this.color = color;

        this.frontSprite = PSpriteLoader.GetFrontSprite(name);
        this.backSprite = PSpriteLoader.GetBackSprite(name);
        SetLevelUpMoves();
        SetEvolutions();
    }

    private void SetEvolutions()
    {
        List<Evolution> evo = EvolutionsLoader.GetEvolution(pokemonName);
        if (evo != null)
        {
            evolutions = evo;
        }
        else
        {
            evolutions = new List<Evolution>();
        }
    }

    public string GetEvolutionString()
    {
        string evo = $"Evolutions: ";
        string originalEvo = evo;
        foreach (Evolution evolution in evolutions)
        {
            if (evolution.NeedsStone)
            {
                string stoneName = evolution.RequiredStone.ToString();
                evo += $"{stoneName} stone, ";
            }
            else
            {
                evo += $"Level: {evolution.RequiredLevel} to {evolution.EvolvesInto}, ";
            }
        }

        if (evo == originalEvo) return "Max evolution";

        evo = evo.TrimEnd(',', ' ');

        return evo;
    }

    private void SetInGamePokedexNumber(string name)
    {
        string normalizedName = FixWeirdPokemonNames(name.ToLower().Replace(" ", "").Replace("-", "").Replace(".", "").Replace("é", "e"));

        this.pokedexNumber = PokedexNumberLoader.GetDexNumber(normalizedName, DebugError: false);
    }

    public void SetLevelUpMoves()
    {
        string normalizedName = FixWeirdPokemonNames(pokemonName.ToLower().Replace(" ", "").Replace("-", "").Replace(".", "").Replace("é", "e"));

        if (LearnsetLoader.LevelUpMovesByPokemonDict != null && LearnsetLoader.LevelUpMovesByPokemonDict.TryGetValue(normalizedName, out var learned))
        {
            this.levelUpMoves = new List<MoveLearn>(learned);
        }
        else
        {
            this.levelUpMoves = new List<MoveLearn>();
            Debug.LogWarning($"No level-up moves found for '{name}' (lookup key: '{normalizedName}')");
        }
    }

    private string FixWeirdPokemonNames(string pokemonName)
    {
        pokemonName = pokemonName
        .Replace("’", "'")   // U+2019 right single quote
        .Replace("‘", "'")   // U+2018 left single quote
        .Replace("‛", "'")   // U+201B single high-reversed-9 quote
        .Replace("′", "'");  // U+2032 prime (just in case)

        if (pokemonName == "kommo-o") return "kommoo";
        else if (pokemonName == "hakamo-o") return "hakamoo";
        else if (pokemonName == "farfetch'd") return "farfetchd";
        else if (pokemonName == "ho-oh") return "hooh";
        else if (pokemonName == "porygon-z") return "porygonz";
        else if (pokemonName == "jangmo-o") return "jangmoo";
        else if (pokemonName == "sirfetch'd") return "sirfetchd";

        return pokemonName;
    }

    public void SetTypes(List<string> types)
    {
        type1 = types.Count > 0 ? StringToPokemonType(types[0]) : PokemonType.None;
        type2 = types.Count > 1 ? StringToPokemonType(types[1]) : PokemonType.None;
    }

    private List<string> GetAbilityList(Dictionary<string, string> abilities)
    {
        if (abilities == null) return new List<string>();
        return new List<string>(abilities.Values);
    }

    public PokemonType StringToPokemonType(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return PokemonType.None;

        // Normalize casing
        text = text.Trim();

        // Special cases where input doesn't match enum exactly
        switch (text.ToLower())
        {
            case "dark matter":
            case "darkmatter":
                return PokemonType.DarkMatter;
        }

        // Try parse ignoring case
        if (Enum.TryParse<PokemonType>(text, true, out var type))
            return type;

        // If parsing fails, default to None
        Debug.LogWarning($"Unknown Pokemon type string: '{text}'");
        return PokemonType.None;
    }

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }
        if (growthRate == GrowthRate.Erratic)
        {
            if (level < 50)
            {
                return (((level * level * level) * (100 - level)) / 50);
            }
            else if (level < 68)
            {
                return (((level * level * level) * (150 - level)) / 100);
            }
            else if (level < 100)
            {
                return (((level * level * level) * ((1911 - (10 * level)) / 3)) / 500);
            }
        }
        if (growthRate == GrowthRate.MediumSlow)
        {
            return (6 / 5) * (level * level * level) - (15 * (level * level)) + (100 * level) - 140;
        }
        if (growthRate == GrowthRate.Slow)
        {
            return (5 * level * level * level) / 4;
        }
        if (growthRate == GrowthRate.Fluctuating)
        {
            if (level < 15)
            {
                return ((level * level * level) * (((level + 1) / 3) + 24)) / 50;
            }
            else if (level < 36)
            {
                return ((level * level * level) * (level + 14)) / 50;
            }
            else if (level < 100)
            {
                return ((level * level * level) * ((level / 2) + 32)) / 50;
            }
        }

        return -1; // THIS IS AN ERROR, THE GROWTH RATE DOES NOT EXIST
    }

    public void DebugMoves()
    {
        if (levelUpMoves == null || levelUpMoves.Count == 0)
        {
            Debug.LogWarning($"{pokemonName} has no level-up moves.");
            return;
        }

        foreach (var ml in levelUpMoves)
        {
            Debug.Log($"{pokemonName} learns {ml}.");
        }
    }

    public void DebugEvolutions()
    {
        if (evolutions == null || evolutions.Count == 0)
        {
            Debug.LogError($"{pokemonName} has no evolutions.");
            return;
        }

        foreach (var evolution in evolutions)
        {
            if (!evolution.NeedsStone) Debug.Log($"{pokemonName} evolves into {evolution.EvolvesInto} at level {evolution.RequiredLevel}.");
            else Debug.Log($"{pokemonName} needs a stone to evolve.");
        }
    }

    public string PokemonName
    {
        get { return pokemonName; }
    }
    public string Description
    {
        get { return description; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int SpAttack
    {
        get { return spAttack; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int SpDefense
    {
        get { return spDefense; }
    }
    public int Speed
    {
        get { return speed; }
    }

    /*
    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
    */

    public int CatchRate => catchRate;
    public int PokedexNumber => pokedexNumber;
    public string FoundLocations => foundLocations;
    public List<MoveBase> LearnableByItems => learnableByItems;
    public List<Evolution> Evolutions => evolutions;

    public int ExpYield => expYield;
    public GrowthRate GrowthRate => growthRate;
    public int UniversalDexNumber => universalDexNumber;
    public List<string> Abilities => abilities;
    public List<MoveLearn> LevelUpMoves => levelUpMoves;
    public double Heightm => heightm;
    public double Weightkg => weightkg;
}

[System.Serializable]
public class Evolution
{
    [SerializeField] string evolvesInto;
    [SerializeField] int requiredLevel;

    [SerializeField] bool needsStone = false;

    private EvolutionStone requiredStone;

    public string EvolvesInto => evolvesInto;
    public int RequiredLevel => requiredLevel;
    public bool NeedsStone => needsStone;
    public EvolutionStone RequiredStone => requiredStone;

    public Evolution(string evolvesInto, int requiredLevel, bool needsStone = false, EvolutionStone evolutionStone = EvolutionStone.None)
    {
        this.evolvesInto = evolvesInto;
        this.requiredLevel = requiredLevel;
        this.needsStone = needsStone;
        if (needsStone)
        {
            requiredStone = evolutionStone;
        }
        else requiredStone = EvolutionStone.None;
    }
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy,
    DarkMatter
}

public enum GrowthRate
{
    MediumFast,
    Erratic,
    Fluctuating,
    MediumSlow,
    Fast,
    Slow,
}

public class TypeChart{
    static float[][] chart =
    {
        //                    NOR FIR WAT ELE GRA ICE FIG POI GRO FLY PSY BUG ROC GHO DRA DAR STE FAI DAM
        /*NOR*/
    new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 0.5f, 1f, 0f },
        /*FIR*/ new float[] { 1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 2f, 1f, 1f },
        /*WAT*/ new float[] { 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f, 1f },
        /*ELE*/ new float[] { 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f },
        /*GRA*/ new float[] { 1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 0.5f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f, 1f},
        /*ICE*/ new float[] { 1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f },
        /*FIG*/ new float[] { 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f, 0.5f },
        /*POI*/ new float[] { 1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f, 1f },
        /*GRO*/ new float[] { 1f, 2f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 0f, 1f, 0.5f, 2f, 1f, 1f, 1f, 2f, 1f, 0.5f },
        /*FLY*/ new float[] { 1f, 1f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f, 1f },
        /*PSY*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f, 2f },
        /*BUG*/ new float[] { 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 0.5f, 0.5f },
        /*ROC*/ new float[] { 1f, 2f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 0.5f },
        /*GHO*/ new float[] { 0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 0f },
        /*DRA*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 0f, 1f },
        /*DAR*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f },
        /*STE*/ new float[] { 1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 2f, 0.5f },
        /*FAI*/ new float[] { 1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 0.5f, 1f, 0.5f },
        /*DAM*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType){
        if (attackType == PokemonType.None || defenseType == PokemonType.None){
            return 1;
        }

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
