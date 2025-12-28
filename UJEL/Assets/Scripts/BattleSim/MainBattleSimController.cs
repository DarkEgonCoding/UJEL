using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainBattleSimController : MonoBehaviour
{
    public static MainBattleSimController instance;

    [SerializeField] UILog ParsedLog;
    [SerializeField] UILog RawLog;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    PsLib.Battle battle;

    private string p1spec = @"[
        {
            ""name"": """",
            ""species"": ""Articuno"",
            ""gender"": """",
            ""item"": ""Leftovers"",
            ""ability"": ""Pressure"",
            ""evs"": {""hp"": 252, ""atk"": 0, ""def"": 0, ""spa"": 252, ""spd"": 4, ""spe"": 0},
            ""nature"": ""Modest"",
            ""ivs"": {""hp"": 31, ""atk"": 31, ""def"": 31, ""spa"": 30, ""spd"": 30, ""spe"": 31},
            ""moves"": [""Ice Beam"", ""Hurricane"", ""Substitute"", ""Roost""]
        },
        {
            ""name"": """",
            ""species"": ""Ludicolo"",
            ""gender"": """",
            ""item"": ""Life Orb"",
            ""ability"": ""Swift Swim"",
            ""evs"": {""hp"": 4, ""atk"": 0, ""def"": 0, ""spa"": 252, ""spd"": 0, ""spe"": 252},
            ""nature"": ""Modest"",
            ""moves"": [""Surf"", ""Giga Drain"", ""Ice Beam"", ""Rain Dance""]
        },
        {
            ""name"": """",
            ""species"": ""Volbeat"",
            ""gender"": ""M"",
            ""item"": ""Damp Rock"",
            ""ability"": ""Prankster"",
            ""evs"": {""hp"": 248, ""atk"": 0, ""def"": 252, ""spa"": 0, ""spd"": 8, ""spe"": 0},
            ""nature"": ""Bold"",
            ""moves"": [""Tail Glow"", ""Baton Pass"", ""Encore"", ""Rain Dance""]
        },
        {
            ""name"": """",
            ""species"": ""Seismitoad"",
            ""gender"": """",
            ""item"": ""Life Orb"",
            ""ability"": ""Swift Swim"",
            ""evs"": {""hp"": 0, ""atk"": 0, ""def"": 0, ""spa"": 252, ""spd"": 4, ""spe"": 252},
            ""nature"": ""Modest"",
            ""moves"": [""Hydro Pump"", ""Earth Power"", ""Stealth Rock"", ""Rain Dance""]
        },
        {
            ""name"": """",
            ""species"": ""Alomomola"",
            ""gender"": """",
            ""item"": ""Damp Rock"",
            ""ability"": ""Regenerator"",
            ""evs"": {""hp"": 252, ""atk"": 0, ""def"": 252, ""spa"": 0, ""spd"": 4, ""spe"": 0},
            ""nature"": ""Bold"",
            ""moves"": [""Wish"", ""Protect"", ""Toxic"", ""Rain Dance""]
        },
        {
            ""name"": """",
            ""species"": ""Armaldo"",
            ""gender"": """",
            ""item"": ""Leftovers"",
            ""ability"": ""Swift Swim"",
            ""evs"": {""hp"": 128, ""atk"": 252, ""def"": 4, ""spa"": 0, ""spd"": 0, ""spe"": 124},
            ""nature"": ""Adamant"",
            ""moves"": [""X-Scissor"", ""Stone Edge"", ""Aqua Tail"", ""Rapid Spin""]
        }
    ]";

    private string p2spec = @"[
        {
            ""name"": """",
            ""species"": ""Articuno"",
            ""gender"": """",
            ""item"": ""Leftovers"",
            ""ability"": ""Pressure"",
            ""evs"": {""hp"": 252, ""atk"": 0, ""def"": 0, ""spa"": 252, ""spd"": 4, ""spe"": 0},
            ""nature"": ""Modest"",
            ""ivs"": {""hp"": 31, ""atk"": 31, ""def"": 31, ""spa"": 30, ""spd"": 30, ""spe"": 31},
            ""moves"": [""Ice Beam"", ""Hurricane"", ""Substitute"", ""Roost""]
        },
        {
            ""name"": """",
            ""species"": ""Ludicolo"",
            ""gender"": """",
            ""item"": ""Life Orb"",
            ""ability"": ""Swift Swim"",
            ""evs"": {""hp"": 4, ""atk"": 0, ""def"": 0, ""spa"": 252, ""spd"": 0, ""spe"": 252},
            ""nature"": ""Modest"",
            ""moves"": [""Surf"", ""Giga Drain"", ""Ice Beam"", ""Rain Dance""]
        },
        {
            ""name"": """",
            ""species"": ""Volbeat"",
            ""gender"": ""M"",
            ""item"": ""Damp Rock"",
            ""ability"": ""Prankster"",
            ""evs"": {""hp"": 248, ""atk"": 0, ""def"": 252, ""spa"": 0, ""spd"": 8, ""spe"": 0},
            ""nature"": ""Bold"",
            ""moves"": [""Tail Glow"", ""Baton Pass"", ""Encore"", ""Rain Dance""]
        },
        {
            ""name"": """",
            ""species"": ""Seismitoad"",
            ""gender"": """",
            ""item"": ""Life Orb"",
            ""ability"": ""Swift Swim"",
            ""evs"": {""hp"": 0, ""atk"": 0, ""def"": 0, ""spa"": 252, ""spd"": 4, ""spe"": 252},
            ""nature"": ""Modest"",
            ""moves"": [""Hydro Pump"", ""Earth Power"", ""Stealth Rock"", ""Rain Dance""]
        },
        {
            ""name"": """",
            ""species"": ""Alomomola"",
            ""gender"": """",
            ""item"": ""Damp Rock"",
            ""ability"": ""Regenerator"",
            ""evs"": {""hp"": 252, ""atk"": 0, ""def"": 252, ""spa"": 0, ""spd"": 4, ""spe"": 0},
            ""nature"": ""Bold"",
            ""moves"": [""Wish"", ""Protect"", ""Toxic"", ""Rain Dance""]
        },
        {
            ""name"": """",
            ""species"": ""Armaldo"",
            ""gender"": """",
            ""item"": ""Leftovers"",
            ""ability"": ""Swift Swim"",
            ""evs"": {""hp"": 128, ""atk"": 252, ""def"": 4, ""spa"": 0, ""spd"": 0, ""spe"": 124},
            ""nature"": ""Adamant"",
            ""moves"": [""X-Scissor"", ""Stone Edge"", ""Aqua Tail"", ""Rapid Spin""]
        }
    ]";

    string PackedP1 =
    "Articuno||leftovers|pressure|icebeam,hurricane,substitute,roost|Modest|252,,,252,4,||,,,30,30,|||]" +
    "Ludicolo ||lifeorb|swiftswim|surf,gigadrain,icebeam,raindance|Modest|4,,,252,,252|||||]" +
    "Volbeat||damprock|prankster|tailglow,batonpass,encore,raindance|Bold|248,,252,,8,|M||||]" +
    "Seismitoad||lifeorb|swiftswim|hydropump,earthpower,stealthrock,raindance|Modest|,,,252,4,252|||||]" +
    "Alomomola ||damprock|regenerator|wish,protect,toxic,raindance|Bold|252,,252,,4,|||||]" +
    "Armaldo ||leftovers|swiftswim|xscissor,stoneedge,aquatail,rapidspin|Adamant|128,252,4,,,124|||||";

    string PackedP2 =
    "Articuno||leftovers|pressure|icebeam,hurricane,substitute,roost|Modest|252,,,252,4,||,,,30,30,|||]" +
    "Ludicolo ||lifeorb|swiftswim|surf,gigadrain,icebeam,raindance|Modest|4,,,252,,252|||||]" +
    "Volbeat||damprock|prankster|tailglow,batonpass,encore,raindance|Bold|248,,252,,8,|M||||]" +
    "Seismitoad||lifeorb|swiftswim|hydropump,earthpower,stealthrock,raindance|Modest|,,,252,4,252|||||]" +
    "Alomomola ||damprock|regenerator|wish,protect,toxic,raindance|Bold|252,,252,,4,|||||]" +
    "Armaldo ||leftovers|swiftswim|xscissor,stoneedge,aquatail,rapidspin|Adamant|128,252,4,,,124|||||";

    public void BattleStart()
    {
        battle = new PsLib.Battle();
        battle.Start(PackedP1, PackedP2, "gen7randombattle");
    }

    public void DoMove(int move, int player)
    {
        Debug.Log($"Move chosen by player {player} with index {move}.");

        string message = $">{(player == 1 ? "p1" : "p2")} move {move}";
        Debug.Log("Message sent: " + message);
        battle.WriteLine(message);
    }

    public void DoSwitch(int switchIndex, int player)
    {
        Debug.Log($"Switch chosen by player {player} with index {switchIndex}.");

        string message = $">{(player == 1 ? "p1" : "p2")} switch {switchIndex}";
        Debug.Log("Message sent: " + message);
        battle.WriteLine(message);
    }

    private void Update()
    {
        if (battle == null) return;

        string raw;
        while (battle.TryGetRaw(out raw))
        {
            RawLog.AddMessage(raw);
        }
    }
}
