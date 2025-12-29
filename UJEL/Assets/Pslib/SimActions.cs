
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace PsLib.Sim.Messages
{
    public enum Stream
    {
        update,
        p1,
        p2
    }

    public enum MessageGroup
    {
        init,
        progress,
        major,
        minor
    }

    public class Message
    {
        public Stream stream;
        public MessageGroup group;
        public Action action;
    }

    public abstract class Action {}

    public class StreamText : Attribute
    {
        public string text;
        public StreamText(string str) { text = str; }
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
    }

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

        public static Stats Parse(string text)
        {
            Stats stats = new Stats();
            string[] statSplit = text.Split(", ");
            stats.arr = statSplit.Select(x => (Stat) Enum.Parse(typeof(Stat), x.Trim())).ToArray();
            return stats;
        }
    }

    /*
     * When it comes time for a player to make a choice, they are provided a request object
     * that holds information about their active pokemon and about their team.
     * 
     * Some examples of the format are as follows:
     * - moves[1][3] refers to the third move by the second active pokemon (double battle only).
     * - side.pokemon[0] refers to the first pokemon on your side.
     */
    public class Request
    {
        public List<List<Move>> moves;
        public Side side;
        public int rqid;

        public static Request Parse(string text)
        {
            return JsonConvert.DeserializeObject<Request>(text);
        }
    }
}

namespace PsLib.Sim.Messages.Init
{
    public class PLAYER : Action
    {
        public Parts.Player player;
        public string username;
        public string avatar;
        public string rating;
    }

    public class TEAMSIZE : Action
    {
        public Parts.Player player;
        public string number;
    }

    public class GAMETYPE : Action
    {
        public Parts.Player player;
        public string number;
    }

    public class GEN : Action
    {
        public int gennum;
    }

    public class TIER : Action
{
        public string formatname;
    }

    public class RATED : Action
    {
        public string message;
    }

    public class RULE : Action
    {
        public string rule;
    }

    public class CLEARPOKE : Action
    {

    }

    public class POKE : Action
    {
        public Parts.Player player;
        public string details;
        public string item;
    }

    public class TEAMPREVIEW : Action
    {

    }

    public class START : Action
    {

    }
}

namespace PsLib.Sim.Messages.Progress
{
    [StreamText("")]
    public class NULLMSG : Action
    {

    }

    public class REQUEST : Action
    {
        public string request;
    }

    public class INACTIVE : Action
    {
        public string message;
    }

    public class UPKEEP : Action
    {

    }

    public class TURN : Action
    {
        public int number;
    }

    public class WIN : Action
    {
        public string user;
    }

    public class TIE : Action
    {

    }

    [StreamText("t:")]
    public class TIMESTAMP : Action
    {
        public int timestamp;
    }
}

namespace PsLib.Sim.Messages.Actions.Major
{
    public class MOVE : Action
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

    public class SWITCH : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Details details;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return pokemon.species + " was switched in.";
        }
    }

    public class DRAG : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Details details;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return pokemon.species + " was dragged in.";
        }
    }

    public class DETAILSCHANGE : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Details details;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return pokemon.species + " turned into " + details.species + ".";
        }
    }
    
    [StreamText("-formechange")]
    public class FORMECHANGE : Action
    {
        public Parts.Pokemon pokemon;
        public string species;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return pokemon.species + " turned into " + species + ".";
        }
    }

    public class REPLACE : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Details details;
        public Parts.HpStatus hpStatus;

        public string GetDesc() {
            return "Illusion ended for " + pokemon.species;
        }
    }

    public class SWAP : Action
    {
        public Parts.Pokemon pokemon;
        public int position;
    }

    public class CANT : Action
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

    public class FAINT : Action
    {
        public Parts.Pokemon pokemon;

        public string GetDesc() {
            return pokemon.species + " fainted.";
        }
    }
}

namespace PsLib.Sim.Messages.Actions.Minor
{

    public class FAIL : Action
    {
        public Parts.Pokemon pokemon;
        public string action;
    }

    public class BLOCK : Action
    {
        public Parts.Pokemon pokemon;
        public string effect;
        [Optional]
        public string move;
        [Optional]
        public string action;
    }

    public class NOTARGET : Action
    {
        [Optional]
        public Parts.Pokemon pokemon;
    }

    public class MISS : Action
    {
        public Parts.Pokemon source;
        public Parts.Pokemon target;
    }

    public class DAMAGE : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.HpStatus hpStatus;
    }

    public class HEAL : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.HpStatus hpStatus;
    }

    public class SETHP : Action
    {
        public Parts.Pokemon pokemon;
        public int hp;
    }

    public class STATUS : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Status status;
    }

    public class CURESTATUS : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Status status;
    }

    public class CURETEAM : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class BOOST : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Stat stat;
        public int amount;
    }

    public class UNBOOST : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Stat stat;
        public int amount;
    }

    public class SETBOOST : Action
    {
        public Parts.Pokemon pokemon;
        public Parts.Stat stat;
        public int amount;
    }

    public class SWAPBOOST : Action
    {
        public Parts.Pokemon source;
        public Parts.Pokemon target;
        public Parts.Stats stats;
    }

    public class INVERTBOOST : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class CLEARBOOST : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class CLEARALLBOOST : Action
    {

    }

    public class CLEARPOSITIVEBOOST : Action
    {
        public Parts.Pokemon target;
        public Parts.Pokemon pokemon;
        public string effect;
    }

    public class CLEARNEGATIVEBOOST : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class COPYBOOST : Action
    {
        public Parts.Pokemon source;
        public Parts.Pokemon target;
    }

    public class WEATHER : Action
    {
        public string weather;
    }

    public class FIELDSTART : Action
    {
        public string condition;
    }

    public class FIELDEND : Action
    {
        public string condition;
    }

    public class SIDESTART : Action
    {
        public string side;
        public string condition;
    }

    public class SIDEEND : Action
    {
        public string side;
        public string condition;
    }

    public class SWAPSIDECONDITIONS : Action
    {

    }

    public class START : Action
    {
        public Parts.Pokemon pokemon;
        public string effect;
    }

    public class END : Action
    {
        public Parts.Pokemon pokemon;
        public string effect;
    }

    public class CRIT : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class SUPEREFFECTIVE : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class RESISTED : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class IMMUNE : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class ITEM : Action
    {
        public Parts.Pokemon pokemon;
        public string item;
        public string effect;
    }

    public class ENDITEM : Action
    {
        public Parts.Pokemon pokemon;
        public string item;
    }

    public class ABILITY : Action
    {
        public Parts.Pokemon pokemon;
        public string ability;
        [Optional]
        public string effect;
    }

    public class ENDABILITY : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class TRANSFORM : Action
    {
        public Parts.Pokemon pokemon;
        public string species;
    }

    public class MEGA : Action
    {
        public Parts.Pokemon pokemon;
        public string megastone;
    }

    public class PRIMAL : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class BURST : Action
    {
        public Parts.Pokemon pokemon;
        public string species;
        public string item;
    }

    public class ZPOWER : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class ZBROKEN : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class ACTIVATE : Action
    {
        public string effect;
    }

    public class HINT : Action
    {
        public string message;
    }

    public class CENTER : Action
    {

    }

    public class MESSAGE : Action
    {
        public string message;
    }

    public class COMBINE : Action
    {

    }

    public class WAITING : Action
    {
        public Parts.Pokemon source;
        public Parts.Pokemon target;
    }

    public class PERPARE : Action
    {
        public Parts.Pokemon attacker;
        public string move;
    }

    public class MUSTRECHARGE : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class NOTHING : Action
    {

    }

    public class HITCOUNT : Action
    {
        public Parts.Pokemon pokemon;
    }

    public class SINGLEMOVE : Action
    {
        public Parts.Pokemon pokemon;
        public string move;
    }

    public class SINGLETURN : Action
    {
        public Parts.Pokemon pokemon;
        public string move;
    }
}
