using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PSpriteLoader
{
    public static Dictionary<string, Sprite> FrontSprites { get; private set; } = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> BackSprites { get; private set; } = new Dictionary<string, Sprite>();

    public static void LoadAllFrontSprites()
    {
        FrontSprites.Clear();

        Sprite[] sprites = Resources.LoadAll<Sprite>("PokeSprites/front");
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("No Pokémon front sprites found in Resources/PokeSprites/front!");
            return;
        }

        foreach (var sprite in sprites)
        {
            if (sprite == null) continue;

            // Just set filter mode; no Apply() so no read/write needed
            sprite.texture.filterMode = FilterMode.Point;

            string nameKey = sprite.name.ToLowerInvariant();
            nameKey = PokemonDB.FixWeirdPokemonNames(nameKey);

            if (!FrontSprites.ContainsKey(nameKey))
            {
                FrontSprites.Add(nameKey, sprite);
            }
        }

        Debug.Log($"Loaded {FrontSprites.Count} Pokémon front sprites into PSpriteLoader.");
        PokemonLoader.instance.SpritesLoaded = true;
    }

    public static void LoadAllBackSprites()
    {
        BackSprites.Clear();

        Sprite[] sprites = Resources.LoadAll<Sprite>("PokeSprites/back");
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("No Pokémon back sprites found in Resources/PokeSprites/back!");
            return;
        }

        foreach (var sprite in sprites)
        {
            if (sprite == null) continue;

            // Just set filter mode; no Apply() so no read/write needed
            sprite.texture.filterMode = FilterMode.Point;

            string nameKey = sprite.name.ToLowerInvariant();
            nameKey = PokemonDB.FixWeirdPokemonNames(nameKey);

            if (!BackSprites.ContainsKey(nameKey))
            {
                BackSprites.Add(nameKey, sprite);
            }
        }

        Debug.Log($"Loaded {BackSprites.Count} Pokémon front sprites into PSpriteLoader.");
        PokemonLoader.instance.BackSpritesLoaded = true;
    }

    public static Sprite GetFrontSprite(string pokemonName)
    {
        if (string.IsNullOrEmpty(pokemonName)) return null;

        pokemonName = PokemonDB.FixWeirdPokemonNames(pokemonName.ToLowerInvariant());

        if (FrontSprites.TryGetValue(pokemonName, out Sprite sprite))
            return sprite;

        Debug.LogWarning($"Front sprite not found for Pokémon '{pokemonName}'");
        return null;
    }

    public static Sprite GetBackSprite(string pokemonName)
    {
        if (string.IsNullOrEmpty(pokemonName)) return null;

        pokemonName = PokemonDB.FixWeirdPokemonNames(pokemonName.ToLowerInvariant());

        if (BackSprites.TryGetValue(pokemonName, out Sprite sprite))
            return sprite;

        Debug.LogWarning($"Back sprite not found for Pokémon '{pokemonName}'");
        return null;
    }
}
