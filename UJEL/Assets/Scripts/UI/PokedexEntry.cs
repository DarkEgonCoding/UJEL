using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class PokedexEntry
{
    public PokemonBase pokemon;
    private Image pokemonImage;
    public bool haveCaught;
    public string description;
    public int entryNumber;
}
