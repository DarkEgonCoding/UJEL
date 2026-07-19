
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PsLib.Sim
{
    [JsonConverter(typeof(StringEnumConverter))] 
    public enum MoveTarget {
        normal,
        any,
        self,
        allySide,
        foeSide,
        allAdjacent,
        allAdjacentFoes
    }

    public class Stats {
        public int atk { get; set; }
        public int def { get; set; }
        public int spa { get; set; }
        public int spd { get; set; }
        public int spe { get; set; }
    }

    public class Pokemon {
        public string ident { get; set; }
        public string details { get; set; }
        public string condition { get; set; }
        public bool active { get; set; }
        public Stats stats { get; set; }
        public List<string> moves { get; set; }
        public string baseAbility { get; set; }
        public string item { get; set; }
        public string pokeball { get; set; }
        public string ability { get; set; }
    }

    public class Move {
        public string move { get; set; }
        public string id { get; set; }
        public int pp { get; set; }
        public int maxpp { get; set; }
        public MoveTarget target { get; set; }
        public bool disables { get; set; }
    }

    public class Side {
        public string name { get; set; }
        public string id { get; set; }
        public Pokemon[] pokemon { get; set; }
    }
}
