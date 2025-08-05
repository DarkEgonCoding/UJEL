using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPokemonSlotUI
{
    void SetData(Pokemon pokemon);
    void ClearData();
    Pokemon StoredPokemon { get; }
}
