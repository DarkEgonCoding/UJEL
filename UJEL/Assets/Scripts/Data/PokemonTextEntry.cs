using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PokemonTextEntry
{
    [SerializeField] PokemonInGame pokemonEnum;
    [SerializeField][Range(0, 100)] int level;
    [SerializeField] List<string> moves;

    public PokemonInGame PokemonEnum => pokemonEnum;
    public int Level => level;
    public List<string> Moves => moves;
}

public static class PokemonTextEntryExtensions
{
    public static PokemonBase PokemonEnumToPokemonBase(PokemonInGame pokemonInGame)
    {
        string pName = pokemonInGame.ToString();
        PokemonBase pb = PokemonDB.GetPokemonByName(pName);
        return pb;
    }

    public static Pokemon TextEntryToPokemon(PokemonTextEntry textEntry)
    {
        string pName = textEntry.PokemonEnum.ToString();
        PokemonBase pb = PokemonDB.GetPokemonByName(pName);

        var pokemon = new Pokemon(pb, textEntry.Level, textEntry.Moves);

        return pokemon;
    }

    public static string ALL_MOVES = "absorb, accelerock, acid, acidarmor, acidspray, acrobatics, acupressure, aerialace, aeroblast, afteryou, agility, aircutter, airslash, alluringvoice, allyswitch, amnesia, anchorshot, ancientpower, appleacid, aquacutter, aquajet, aquaring, aquastep, aquatail, armorcannon, armthrust, aromatherapy, aromaticmist, assist, assurance, astonish, astralbarrage, attackorder, attract, aurasphere, aurawheel, aurorabeam, auroraveil, autotomize, avalanche, axekick, babydolleyes, baddybad, banefulbunker, barbbarrage, barrage, barrier, batonpass, beakblast, beatup, behemothbash, behemothblade, belch, bellydrum, bestow, bide, bind, bite, bitterblade, bittermalice, blastburn, blazekick, bleakwindstorm, blizzard, block, bloodmoon, blueflare, bodypress, bodyslam, boltbeak, boltstrike, boneclub, bonemerang, bonerush, boomburst, bounce, bouncybubble, branchpoke, bravebird, breakingswipe, brickbreak, brine, brutalswing, bubble, bubblebeam, bugbite, bugbuzz, bulkup, bulldoze, bulletpunch, bulletseed, burningbulwark, burningjealousy, burnup, buzzybuzz, calmmind, camouflage, captivate, ceaselessedge, celebrate, charge, chargebeam, charm, chatter, chillingwater, chillyreception, chipaway, chloroblast, circlethrow, clamp, clangingscales, clangoroussoul, clearsmog, closecombat, coaching, coil, collisioncourse, cometpunch, comeuppance, confide, confuseray, confusion, constrict, conversion, conversion2, copycat, coreenforcer, corrosivegas, cosmicpower, cottonguard, cottonspore, counter, courtchange, covet, crabhammer, craftyshield, crosschop, crosspoison, crunch, crushclaw, crushgrip, curse, cut, darkestlariat, darkpulse, darkvoid, dazzlinggleam, decorate, defendorder, defensecurl, defog, destinybond, detect, diamondstorm, dig, direclaw, disable, disarmingvoice, discharge, dive, dizzypunch, doodle, doomdesire, doubleedge, doublehit, doubleironbash, doublekick, doubleshock, doubleslap, doubleteam, dracometeor, dragonascent, dragonbreath, dragoncheer, dragonclaw, dragondance, dragondarts, dragonenergy, dragonhammer, dragonpulse, dragonrage, dragonrush, dragontail, drainingkiss, drainpunch, dreameater, drillpeck, drillrun, drumbeating, dualchop, dualwingbeat, dynamaxcannon, dynamicpunch, earthpower, earthquake, echoedvoice, eerieimpulse, eeriespell, eggbomb, electricterrain, electrify, electroball, electrodrift, electroshot, electroweb, embargo, ember, encore, endeavor, endure, energyball, entrainment, eruption, esperwing, eternabeam, expandingforce, explosion, extrasensory, extremespeed, facade, fairylock, fairywind, fakeout, faketears, falsesurrender, falseswipe, featherdance, feint, feintattack, fellstinger, ficklebeam, fierydance, fierywrath, filletaway, finalgambit, fireblast, firefang, firelash, firepledge, firepunch, firespin, firstimpression, fishiousrend, fissure, flail, flameburst, flamecharge, flamethrower, flamewheel, flareblitz, flash, flashcannon, flatter, fleurcannon, fling, flipturn, floatyfall, floralhealing, flowershield, flowertrick, fly, flyingpress, focusblast, focusenergy, focuspunch, followme, forcepalm, foresight, forestscurse, foulplay, freezedry, freezeshock, freezingglare, freezyfrost, frenzyplant, frostbreath, frustration, furyattack, furycutter, furyswipes, fusionbolt, fusionflare, futuresight, gastroacid, geargrind, gearup, geomancy, gigadrain, gigaimpact, gigatonhammer, glaciallance, glaciate, glaiverush, glare, glitzyglow, grassknot, grasspledge, grasswhistle, grassyglide, grassyterrain, gravapple, gravity, growl, growth, grudge, guardsplit, guardswap, guillotine, gunkshot, gust, gyroball, hail, hammerarm, happyhour, harden, hardpress, haze, headbutt, headcharge, headlongrush, headsmash, healbell, healblock, healingwish, healorder, healpulse, heartstamp, heartswap, heatcrash, heatwave, heavyslam, helpinghand, hex, hiddenpower, highhorsepower, highjumpkick, holdback, holdhands, honeclaws, hornattack, horndrill, hornleech, howl, hurricane, hydrocannon, hydropump, hydrosteam, hyperbeam, hyperdrill, hyperfang, hyperspacefury, hyperspacehole, hypervoice, hypnosis, iceball, icebeam, iceburn, icefang, icehammer, icepunch, iceshard, icespinner, iciclecrash, iciclespear, icywind, imprison, incinerate, infernalparade, inferno, infestation, ingrain, instruct, iondeluge, irondefense, ironhead, irontail, ivycudgel, jawlock, jetpunch, judgment, jumpkick, junglehealing, karatechop, kinesis, kingsshield, knockoff, kowtowcleave, landswrath, laserfocus, lashout, lastresort, lastrespects, lavaplume, leafage, leafblade, leafstorm, leaftornado, leechlife, leechseed, leer, lick, lifedew, lightofruin, lightscreen, liquidation, lockon, lovelykiss, lowkick, lowsweep, luckychant, luminacrash, lunarblessing, lunardance, lunge, lusterpurge, machpunch, magicalleaf, magiccoat, magicpowder, magicroom, magmastorm, magnetbomb, magneticflux, magnetrise, magnitude, makeitrain, malignantchain, matblock, matchagotcha, meanlook, meditate, mefirst, megadrain, megahorn, megakick, megapunch, memento, metalburst, metalclaw, metalsound, meteorassault, meteorbeam, meteormash, metronome, mightycleave, milkdrink, mimic, mindblown, mindreader, minimize, miracleeye, mirrorcoat, mirrormove, mirrorshot, mist, mistball, mistyexplosion, mistyterrain, moonblast, moongeistbeam, moonlight, morningsun, mortalspin, mountaingale, mudbomb, muddywater, mudshot, mudslap, mudsport, multiattack, mysticalfire, mysticalpower, nastyplot, naturalgift, naturepower, naturesmadness, needlearm, nightdaze, nightmare, nightshade, nightslash, nobleroar, noretreat, nuzzle, oblivionwing, obstruct, octazooka, octolock, odorsleuth, ominouswind, orderup, originpulse, outrage, overdrive, overheat, painsplit, paleowave, paraboliccharge, partingshot, payback, payday, peck, perishsong, petalblizzard, petaldance, phantomforce, photongeyser, pikapapow, pinmissile, plasmafists, playnice, playrough, pluck, poisonfang, poisongas, poisonjab, poisonpowder, poisonsting, poisontail, polarflare, pollenpuff, poltergeist, populationbomb, pounce, pound, powder, powdersnow, powergem, powersplit, powerswap, powertrick, powertrip, poweruppunch, powerwhip, precipiceblades, present, prismaticlaser, protect, psybeam, psyblade, psychic, psychicfangs, psychicnoise, psychicterrain, psychoboost, psychocut, psychoshift, psychup, psyshieldbash, psyshock, psystrike, psywave, punishment, purify, pursuit, pyroball, quash, quickattack, quickguard, quiverdance, rage, ragefist, ragepowder, ragingbull, ragingfury, raindance, rapidspin, razorleaf, razorshell, razorwind, recover, recycle, reflect, reflecttype, refresh, relicsong, rest, retaliate, return, revelationdance, revenge, reversal, revivalblessing, risingvoltage, roar, roaroftime, rockblast, rockclimb, rockpolish, rockslide, rocksmash, rockthrow, rocktomb, rockwrecker, roleplay, rollingkick, rollout, roost, rototiller, round, ruination, sacredfire, sacredsword, safeguard, saltcure, sandattack, sandsearstorm, sandstorm, sandtomb, sappyseed, scald, scaleshot, scaryface, scorchingsands, scratch, screech, searingshot, secretpower, secretsword, seedbomb, seedflare, seismictoss, selfdestruct, shadowball, shadowbone, shadowclaw, shadowforce, shadowpunch, shadowsneak, shadowstrike, sharpen, shedtail, sheercold, shellsidearm, shellsmash, shelltrap, shelter, shiftgear, shockwave, shoreup, signalbeam, silktrap, silverwind, simplebeam, sing, sizzlyslide, sketch, skillswap, skittersmack, skullbash, skyattack, skydrop, skyuppercut, slackoff, slam, slash, sleeppowder, sleeptalk, sludge, sludgebomb, sludgewave, smackdown, smartstrike, smellingsalts, smog, smokescreen, snaptrap, snarl, snatch, snipeshot, snore, snowscape, soak, softboiled, solarbeam, solarblade, sonicboom, spacialrend, spark, sparklingaria, sparklyswirl, spectralthief, speedswap, spicyextract, spiderweb, spikecannon, spikes, spikyshield, spinout, spiritbreak, spiritshackle, spite, spitup, splash, splishysplash, spore, spotlight, springtidestorm, stealthrock, steameruption, steamroller, steelbeam, steelroller, steelwing, stickyweb, stockpile, stomp, stompingtantrum, stoneaxe, stoneedge, storedpower, stormthrow, strangesteam, strength, strengthsap, stringshot, strugglebug, stuffcheeks, stunspore, submission, substitute, suckerpunch, sunnyday, sunsteelstrike, supercellslam, superfang, superpower, supersonic, surf, surgingstrikes, swagger, swallow, sweetkiss, sweetscent, swift, switcheroo, swordsdance, synchronoise, synthesis, syrupbomb, tachyoncutter, tackle, tailglow, tailslap, tailwhip, tailwind, takedown, takeheart, tarshot, taunt, tearfullook, teatime, technoblast, teeterdance, telekinesis, teleport, temperflare, terablast, terastarstorm, terrainpulse, thief, thousandarrows, thousandwaves, thrash, throatchop, thunder, thunderbolt, thundercage, thunderclap, thunderfang, thunderouskick, thunderpunch, thundershock, thunderwave, tickle, tidyup, topsyturvy, torchsong, torment, toxic, toxicspikes, toxicthread, trailblaze, transform, triattack, trick, trickortreat, trickroom, triplearrows, tripleaxel, tripledive, triplekick, tropkick, trumpcard, twinbeam, twineedle, twister, upperhand, uproar, uturn, vacuumwave, vcreate, veeveevolley, venomdrench, venoshock, victorydance, vinewhip, visegrip, vitalthrow, voltswitch, volttackle, wakeupslap, waterfall, watergun, waterpledge, waterpulse, watershuriken, watersport, waterspout, wavecrash, weatherball, whirlpool, whirlwind, wickedblow, wideguard, wildboltstorm, wildcharge, willowisp, wingattack, wish, withdraw, wonderroom, woodhammer, workup, worryseed, wrap, wringout, xscissor, yawn, zapcannon, zenheadbutt, zingzap, zippyzap";
}

public enum PokemonInGame
{
    Bulbasaur, Ivysaur, Venusaur, Charmander, Charmeleon, Charizard, Squirtle, Wartortle, Blastoise,
    Chikorita, Bayleef, Meganium, Cyndaquil, Quilava, Typhlosion, Totodile, Croconaw, Feraligatr,
    Treecko, Grovyle, Sceptile, Torchic, Combusken, Blaziken, Mudkip, Marshtomp, Swampert, Turtwig,
    Grotle, Torterra, Chimchar, Monferno, Infernape, Piplup, Prinplup, Empoleon, Snivy, Servine, Serperior,
    Tepig, Pignite, Emboar, Oshawott, Dewott, Samurott, Chespin, Quilladin, Chesnaught, Fennekin, Braixen,
    Delphox, Froakie, Frogadier, Greninja, Rowlet, Dartrix, Decidueye, Litten, Torracat, Incineroar,
    Popplio, Brionne, Primarina, Grookey, Thwackey, Rillaboom, Scorbunny, Raboot, Cinderace, Sobble,
    Drizzile, Inteleon, Sentret, Furret, Mareep, Flaaffy, Eevee, Vaporeon, Flareon, Jolteon, Espeon,
    Umbreon, Leafeon, Glaceon, Sylveon, Vulpix, Ninetales, AlolanVulpix, AlolanNinetales, Cubchoo,
    Beartic, Onix, Steelix, Drilbur, Excadrill, Nincada, Ninjask, Shedinja, Tympole, Palpitoad, Seismitoad,
    Timburr, Gurdurr, Conkeldurr, Sandygast, Palossand, Mudbray, Mudsdale, Passimian, Makuhita, Hariyama,
    Clobbopus, Grapploct, Meditite, Medicham, Houndour, Houndoom, Poochyena, Mightyena, Pawniard, Bisharp,
    Kingambit, Absol, Vullaby, Mandibuzz, Ralts, Kirlia, Gardevoir, Gallade, Duskull, Dusclops, Dusknoir,
    Venipede, Whirlipede, Scolipede, Dewpider, Araquanid, Grubbin, Charjabug, Vikavolt, Cutiefly, Ribombee,
    Venonat, Venomoth, Ledyba, Ledian, Heracross, Pinsir, Joltik, Galvantula, Dwebble, Crustle, Zubat, Golbat,
    Crobat, Pidgey, Pidgeotto, Pidgeot, Rufflet, Braviary, Aerodactyl, Fletchling, Fletchinder, Talonflame,
    Porygon, Porygon2, PorygonZ, Buneary, Lopunny, Tauros, Helioptile, Heliolisk, Doduo, Dodrio, Deerling,
    Sawsbuck, Darumaka, Darmanitan, Growlithe, Arcanine, Litwick, Lampent, Chandelure, Turtonator, Fomantis,
    Lurantis, Seedot, Nuzleaf, Shiftry, Farfetchd, Sirfetchd, Snover, Abomasnow, Phantump,
    Trevenant, Mareanie, Toxapex, Spheal, Sealeo, Walrein, Mantyke, Mantine, Shellos, Gastrodon, Frillish,
    Jellicent, Tynamo, Eelektrik, Eelektross, Elekid, Electabuzz, Electivire, Rotom, Arctozolt, Chinchou,
    Lanturn, Croagunk, Toxicroak, Roggenrola, Boldore, Gigalith, Trapinch, Vibrava, Flygon, Abra, Kadabra,
    Alakazam, Noibat, Noivern, Clauncher, Clawitzer, Aron, Lairon, Aggron, Slakoth, Vigoroth, Slaking, Wishiwashi,
    Archen, Archeops, Larvesta, Volcarona, Golett, Golurk, Weedle, Kakuna, Beedrill, Pichu, Pikachu, Raichu,
    Magnemite, Magneton, Magnezone, Shellder, Cloyster, Cubone, Marowak, Tangela, Tangrowth, Ditto, Ampharos
    , Sudowoodo, Shuckle, Smeargle, Lotad, Lombre, Ludicolo, Torkoal, Swablu, Altaria, Baltoy, Claydol, Feebas,
    Milotic, Bagon, Shelgon, Salamence, Beldum, Metang, Metagross, Bidoof, Bibarel, Drifloon, Drifblim, Bronzor,
    Bronzong, Gible, Gabite, Garchomp, Riolu, Lucario, Sandile, Krokorok, Krookodile, Zorua, Zoroark, Ferroseed,
    Ferrothorn, Klink, Klang, Klinklang, Axew, Fraxure, Haxorus, Deino, Zweilous, Hydreigon, Espurr, Meowstic,
    Honedge, Doublade, Aegislash, Tyrunt, Tyrantrum, Hawlucha, Goomy, Sliggoo, Goodra, Klefki, Salandit, Salazzle,
    Dhelmise, Impidimp, Morgrem, Grimmsnarl, Arctovish, Sneasel, Weavile, Snorunt, Glalie, Froslass, Lapras,
    Jangmoo, Hakamoo, Kommoo
}