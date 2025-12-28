
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PsLib.Dex
{
    public class Stats
    {
        public int hp;
        public int atk;
        public int def;
        public int spa;
        public int spd;
        public int spe;
    }

    public class Pokemon
    {
        public int num;
        public string name;
        public List<string> types;
        public Stats baseStats;
        public Dictionary<string, string> abilities;
        public double heightm;
        public double weightkg;
        public string color;
        
        public string serialize(Pokemon pkmn)
        {
            return JsonConvert.SerializeObject(pkmn);
        }

        public static Pokemon deserialize(string json)
        {
            Pokemon req = JsonConvert.DeserializeObject<Pokemon>(json);
            return req;
        }
    }
}
