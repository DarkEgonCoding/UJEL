// PokemonDataServer.js
const { Dex } = require('pokemon-showdown');
const readline = require('readline');

// Create stdin interface
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
    terminal: false
});

// Function to process a command
function processCommand(line) {
    line = line.trim();
    if (!line) return;

    // Expected format: "getByName pikachu" or "getByNum 25"
    const parts = line.split(/\s+/);
    const command = parts[0];
    const arg = parts.slice(1).join(' ');

    let result;

    if (command === 'getByName') {
        const pokemon = Dex.species.get(arg);
        result = formatPokemon(pokemon);
    } 
    else if (command === 'getByNum') {
        const num = parseInt(arg, 10);
        const pokemon = Dex.species.all().find(p => p.num === num);
        result = formatPokemon(pokemon);
    }
    else if (command === 'exit') {
        process.exit(0);
    }
    else if (line.trim() === "getAll") 
    {
        const allPokemon = Dex.species.all(); // from pokemon-showdown
        console.log(JSON.stringify(allPokemon));
    }
    else {
        result = { error: 'Unknown command' };
    }

    console.log(JSON.stringify(result));
}

// Function to format a Pokémon object
function formatPokemon(pokemon) {
    if (!pokemon || !pokemon.exists) {
        return { error: 'Pokemon not found' };
    }
    return {
        num: pokemon.num,
        name: pokemon.name,
        types: pokemon.types,
        baseStats: pokemon.baseStats,
        abilities: pokemon.abilities,
        heightm: pokemon.heightm,
        weightkg: pokemon.weightkg,
        color: pokemon.color
    };
}

// Read each line from stdin
(async () => {
    for await (const line of rl) {
        processCommand(line);
    }
})();
