
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PsLib.Sim
{
    public abstract class Message {

    }

    [JsonConverter(typeof(StringEnumConverter))] 
    public enum MoveTarget {
        normal,
        self,
        allySide
    }

    public class Stats {
        public int atk;
        public int def;
        public int spa;
        public int spd;
        public int spe;
    }

    public class Pokemon {
        public string ident;
        public string details;
        public string condition;
        public bool active;
        public Stats stats;
        public List<string> moves;
        public string baseAbility;
        public string item;
        public string pokeball;
        public string ability;
    }

    public class Move {
        public string move;
        public string id;
        public int pp;
        public int maxpp;
        public MoveTarget target;
        public bool disables;
    }

    public class Side {
        public string name;
        public string id;
        public Pokemon pokemon;
    }

    /*
     * When it comes time for a player to make a choice, they are provided a request object
     * that holds information about their active pokemon and about their team.
     * 
     * Some examples of the format are as follows:
     * - moves[1][3] refers to the third move by the second active pokemon (double battle only).
     * - side.pokemon[0] refers to the first pokemon on your side.
     */
    public class Request : Message {
        public List<List<Move>> moves;
        public Side side;
        public int rqid;

        public string serialize(Request req)
        {
            return JsonConvert.SerializeObject(req);
        }

        public Request deserialize(string json)
        {
            Request req = JsonConvert.DeserializeObject<Request>(json);
            return req;
        }
    }

    public enum MajorActionTypes
    {
        MOVE,
        SWITCH,
        DETAILSCHANGE,
        REPLACE,
        SWAP,
        CANT,
        FAINT
    }

    public enum MinorActionTypes
    {
        FAIL,
        BLOCK,
        NOTARGET,
        MISS,
        DAMAGE,
        HEAL,
        SETHP,
        STATUS,
        CURESTATUS,
        CURETEAM,
        BOOST,
        UNBOOST,
        SETBOOST,
        SWAPBOOST,
        INVERTBOOST,
        CLEARBOOST,
        CLEARALLBOOST,
        CLEARPOSITIVEBOOST,
        CLEARNEGATIVEBOOST,
        COPYBOOST,
        WEATHER,
        FIELDSTART,
        FIELDEND,
        SIDESTART,
        SIDEEND,
        SWAPSIDECONDITIONS,
        START,
        END,
        CRIT,
        SUPEREFFECTIVE,
        RESISTED,
        IMMUNE,
        ITEM,
        ENDITEM,
        ABILITY,
        TRANSFORM,
        MEGA,
        PRIMAL,
        BURST,
        ZPOWER,
        ZBROKEN,
        ACTIVATE,
        HINT,
        CENTER,
        MESSAGE,
        COMBINE,
        WAITING,
        PERPARE,
        MUSTRECHARGE,
        NOTHING,
        HITCOUNT,
        SINGLEMOVE,
        SINGLETURN
    }
}
