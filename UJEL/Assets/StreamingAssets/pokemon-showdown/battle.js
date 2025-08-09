
// import * as Sim from 'pokemon-showdown'; 

const Sim = require('pokemon-showdown');
const readline = require('readline');
stream = new Sim.BattleStream();

(async () => {
    for await (const output of stream) {
        console.log(output);
    }
})();

stream.write(`>start {"formatid":"gen7randombattle"}`);
stream.write(`>player p1 {"name":"Alice"}`);
stream.write(`>player p2 {"name":"Bob"}`);

var rl = readline.createInterface({
	input: process.stdin,
	output: process.stdout,
	terminal: false
});

(async () => {
	for await (const line of rl) {
		stream.write(line);
	}
})();

console.log("Exiting");
