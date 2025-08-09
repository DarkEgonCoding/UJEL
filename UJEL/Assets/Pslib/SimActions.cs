
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using PsLib.Sim.Messages;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;

namespace PsLib.Sim.Messages
{
    public abstract class Message {}

    public class StreamText : Attribute
    {
        public StreamText(string str) {}
    }

    public class Flag : Attribute
    {
        public Flag(string str) {}
    }

    public class Optional : Attribute
    {
        public Optional() {}
    }
}

namespace PsLib.Sim.Messages.Parts
{
    public enum Status {
        brn,
        frz,
        hail,
        par,
        powder,
        prankster,
        sandstorm,
        tox,
        trapped
    };

    public enum Stat {
        atk,
        def,
        spd,
        spa,
        spe
    }

    public class Player {
        public enum PlayerPos
        {
            p1,
            p2,
            p3,
            p4
        }

        public PlayerPos pos;

        public Player Parse(string text)
        {
            Player p = new Player();
            p.pos = (PlayerPos) Enum.Parse(typeof(PlayerPos), text);
            return p;
        }
    }

    public class Pokemon
    {
        public enum PokePos
        {
            p1a, p1b,
            p2a, p2b,
            p3a, p3b,
            p4a, p4b
        }

        public PokePos pos;
        public string species;
        
        public static Pokemon Parse(string text)
        {
            Pokemon p = new Pokemon();

            string posStr = text.Split(": ")[0];
            string species = text.Split(": ")[1];

            return p;
        }
    }

    public class Details
    {
        public string species;
        public int level;
        public string gender;
        public bool isShiny;
        public string teratype;

        public const string LEVEL_REGEX = @"L\d\d";

        public static Details Parse(string text)
        {
            Details p = new Details();
            string[] details = text.Split(", ");
            p.species = details[0];

            foreach (string detail in details[1..])
            {
                if (Regex.IsMatch(detail, LEVEL_REGEX)) {
                    p.level = int.Parse(detail.Substring(1));
                } else if (detail == "F" || detail == "M") {
                    p.gender = detail;
                } else if (detail == "shiny") {
                    p.isShiny = true;
                } else if (detail.Contains("tera:")) {
                    p.teratype = detail.Split(":")[1];
                }
            }
            
            return p;
        }
    }

    public class HpStatus
    {
        public string hpCurrent;
        public string hpMax;
        public Status status;

        public static HpStatus Parse(string text)
        {
            HpStatus hps = new HpStatus();
            string[] details = text.Split(" ");
            hps.hpCurrent = details[0].Split("/")[0];
            hps.hpMax = details[0].Split("/")[0];
            hps.status = (Status) Enum.Parse(typeof(Status), details[1]);
            return hps;
        }
    }

    public class Stats
    {
        public Stat[] arr;

        public Stats Parse(string text)
        {
            Stats stats = new Stats();
            string[] statSplit = text.Split(", ");
            stats.arr = statSplit.Select(x => (Stat) Enum.Parse(typeof(Stat), x.Trim())).ToArray();
            return stats;
        }
    }
}

namespace PsLib.Sim.Messages.Init
{
    public class PLAYER : Message
    {
        public Parts.Player player;
        public string username;
        public string avatar;
        public string rating;
    }

    public class TEAMSIZE : Message
    {
        public Parts.Player player;
        public string number;
    }

    public class gametype : Message
    {
        public Parts.Player player;
        public string number;
    }

    public class GEN : Message
    {
        public int gennum;
    }

    public class TIER : Message
    {
        public string formatname;
    }

    public class RATED : Message
    {
        public string message;
    }

    public class RULE : Message
    {
        public string rule;
    }

    public class CLEARPOKE : Message
    {

    }

    public class POKE : Message
    {
        public Parts.Player player;
        public string details;
        public string item;
    }

    public class TEAMPREVIEW : Message
    {

    }

    public class START : Message
    {

    }
}

namespace PsLib.Sim.Messages.Progress
{
    [StreamText("")]
    public class NULLMSG : Message
    {

    }

    public class REQUEST : Message
    {
        public string request;
    }

    public class INACTIVE : Message
    {
        public string message;
    }

    public class UPKEEP : Message
    {

    }

    public class TURN : Message
    {
        public int number;
    }

    public class WIN : Message
    {
        public string user;
    }

    public class TIE : Message
    {

    }

    [StreamText("t:")]
    public class TIMESTAMP : Message
    {
        public int timestamp;
    }
}

namespace PsLib.Sim.Messages.Actions.Major
{
    public class MOVE : Message
    {
        public Parts.Pokemon pokemon;
        public string move;
        public Parts.Pokemon target;

        /* The move missed */
        [Flag("[miss]")]
        public bool isMiss;

        /* The move should not be animated */
        [Flag("[still]")]
        public bool isStill;

        /* The move should use the animation of MOVE2 */
        [Flag("[anim]")]
        public bool useAnim;

        public string GetDesc() {
            return pokemon.species + " used move " + move + " at " + target + ".";
        }
    }

    public class SWITCH : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Details details;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return pokemon.species + " was switched in.";
        }
    }

    public class DRAG : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Details details;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return pokemon.species + " was dragged in.";
        }
    }

    public class DETAILSCHANGE : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Details details;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return pokemon.species + " turned into " + details.species + ".";
        }
    }
    
    [StreamText("-formechange")]
    public class FORMECHANGE : Message
    {
        public Parts.Pokemon pokemon;
        public string species;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return pokemon.species + " turned into " + species + ".";
        }
    }

    public class REPLACE : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Details details;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return "Illusion ended for " + pokemon.species;
        }
    }

    public class SWAP : Message
    {
        public Parts.Pokemon pokemon;
        public int position;
    }

    public class CANT : Message
    {
        public Parts.Pokemon pokemon;
        public string reason;
        [Optional]
        public string move;

        public string GetDesc() {
            if (move == null) {
                return pokemon.species + " couldn't because of " + reason + ".";
            } else {
                return pokemon.species + " tried to use " + move +
                    " but it couldn't because of " + reason + ".";
            }
        }
    }

    public class FAINT : Message
    {
        public Parts.Pokemon pokemon;

        public string GetDesc() {
            return pokemon.species + " fainted.";
        }
    }
}

namespace PsLib.Sim.Messages.Actions.Minor
{

    public class FAIL : Message
    {
        public Parts.Pokemon pokemon;
        public string action;
    }

    public class BLOCK : Message
    {
        public Parts.Pokemon pokemon;
        public string effect;
        [Optional]
        public string move;
        [Optional]
        public string action;
    }

    public class NOTARGET : Message
    {
        [Optional]
        public Parts.Pokemon pokemon;
    }

    public class MISS : Message
    {
        public Parts.Pokemon source;
        public Parts.Pokemon target;
    }

    public class DAMAGE : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.HpStatus hpStatus;
    }

    public class HEAL : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.HpStatus hpStatus;
    }

    public class SETHP : Message
    {
        public Parts.Pokemon pokemon;
        public int hp;
    }

    public class STATUS : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Status status;
    }

    public class CURESTATUS : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Status status;
    }

    public class CURETEAM : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class BOOST : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Stat stat;
        public int amount;
    }

    public class UNBOOST : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Stat stat;
        public int amount;
    }

    public class SETBOOST : Message
    {
        public Parts.Pokemon pokemon;
        public Parts.Stat stat;
        public int amount;
    }

    public class SWAPBOOST : Message
    {
        public Parts.Pokemon source;
        public Parts.Pokemon target;
        public Parts.Stats stats;
    }

    public class INVERTBOOST : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class CLEARBOOST : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class CLEARALLBOOST : Message
    {

    }

    public class CLEARPOSITIVEBOOST : Message
    {
        public Parts.Pokemon target;
        public Parts.Pokemon pokemon;
        public string effect;
    }

    public class CLEARNEGATIVEBOOST : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class COPYBOOST : Message
    {
        public Parts.Pokemon source;
        public Parts.Pokemon target;
    }

    public class WEATHER : Message
    {
        public string weather;
    }

    public class FIELDSTART : Message
    {
        public string condition;
    }

    public class FIELDEND : Message
    {
        public string condition;
    }

    public class SIDESTART : Message
    {
        public string side;
        public string condition;
    }

    public class SIDEEND : Message
    {
        public string side;
        public string condition;
    }

    public class SWAPSIDECONDITIONS : Message
    {

    }

    public class START : Message
    {
        public Parts.Pokemon pokemon;
        public string effect;
    }

    public class END : Message
    {
        public Parts.Pokemon pokemon;
        public string effect;
    }

    public class CRIT : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class SUPEREFFECTIVE : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class RESISTED : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class IMMUNE : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class ITEM : Message
    {
        public Parts.Pokemon pokemon;
        public string item;
        public string effect;
    }

    public class ENDITEM : Message
    {
        public Parts.Pokemon pokemon;
        public string item;
    }

    public class ABILITY : Message
    {
        public Parts.Pokemon pokemon;
        public string ability;
        [Optional]
        public string effect;
    }

    public class ENDABILITY : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class TRANSFORM : Message
    {
        public Parts.Pokemon pokemon;
        public string species;
    }

    public class MEGA : Message
    {
        public Parts.Pokemon pokemon;
        public string megastone;
    }

    public class PRIMAL : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class BURST : Message
    {
        public Parts.Pokemon pokemon;
        public string species;
        public string item;
    }

    public class ZPOWER : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class ZBROKEN : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class ACTIVATE : Message
    {
        public string effect;
    }

    public class HINT : Message
    {
        public string message;
    }

    public class CENTER : Message
    {

    }

    public class MESSAGE : Message
    {
        public string message;
    }

    public class COMBINE : Message
    {

    }

    public class WAITING : Message
    {
        public Parts.Pokemon source;
        public Parts.Pokemon target;
    }

    public class PERPARE : Message
    {
        public Parts.Pokemon attacker;
        public string move;
    }

    public class MUSTRECHARGE : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class NOTHING : Message
    {

    }

    public class HITCOUNT : Message
    {
        public Parts.Pokemon pokemon;
    }

    public class SINGLEMOVE : Message
    {
        public Parts.Pokemon pokemon;
        public string move;
    }

    public class SINGLETURN : Message
    {
        public Parts.Pokemon pokemon;
        public string move;
    }
}
