
// import * as Sim from 'pokemon-showdown'; 

const Sim = require('pokemon-showdown');
const readline = require('readline');
stream = new Sim.BattleStream();

(async () => {
    for await (const output of stream) {
        console.log(output);
    }
})();

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
