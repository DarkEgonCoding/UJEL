
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PsLib.Sim
{
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
}
