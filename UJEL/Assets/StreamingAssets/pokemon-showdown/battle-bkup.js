
import * as Sim from 'pokemon-showdown';
//const BattleStreams = require('pokemon-showdown').BattleStreams
//const RandomPlayerAI = require('pokemon-showdown').RandomPlayerAI
//const Teams = require('pokemon-showdown').Teams

//import {BattleStreams, RandomPlayerAI, Teams} from './sim';
//import {TeamGenerators} from './randoms';
//import * as readline from 'readline';
//
//Teams.setGeneratorFactory(TeamGenerators);
//
//const p2 = new RandomPlayerAI(streams.p2);
//void p2.start();
//
//const streams = BattleStreams.getPlayerStreams(new BattleStreams.BattleStream());
//const spec = {formatid: 'gen7customgame'};
//
//const p1spec = {name: 'Bot 1', team: Teams.pack(Teams.generate('gen7randombattle'))};
//const p2spec = {name: 'Bot 2', team: Teams.pack(Teams.generate('gen7randombattle'))};
//
//
////void p1.start();
//
////void (async () => {
////	for await (const chunk of streams.omniscient) {
////		console.log(chunk);
////	}
////})();
//
//void (async () => {
//	for await (const chunk of streams.p1) {
//		console.log(chunk);
//	}
//})();
//
////void (async () => {
////	for await (const chunk of streams.p2) {
////		console.log(chunk);
////	}
////})();
//
////console.log(p1spec);
////console.log(p2spec);
////process.exit();
//
//void streams.omniscient.write(`>start ${JSON.stringify(spec)}
//>player p1 ${JSON.stringify(p1spec)}
//>player p2 ${JSON.stringify(p2spec)}`);
//
//var rl = readline.createInterface({
//	input: process.stdin,
//	output: process.stdout,
//});
//
//for await (const line of rl) {
//	streams.p1.write(line);
//}
//
//console.log("Exiting");
