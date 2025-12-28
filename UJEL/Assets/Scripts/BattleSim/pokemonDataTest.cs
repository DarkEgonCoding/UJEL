using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PsLib;

public class pokemonDataTest : MonoBehaviour
{
    PokemonDataServer server;

    void Start()
    {
        server = new PokemonDataServer();
        server.Start();

        server.WriteLine("getByNum 1");
    }

    void Update()
    {
        if (server.TryGetOutput(out string output))
        {
            Debug.Log($"Server Output: {output}");
        }
    }
}
