using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    [CreateAssetMenu(fileName = "new_pokemon", menuName = "Pokemon/Create new Pokemon")]
    public class PokemonData : ScriptableObject
    {
        // Name and Description
        [SerializeField] string speciesName;
        [TextArea]
        [SerializeField] string description;

        // Sprite data
        [SerializeField] Sprite frontSprite;
        [SerializeField] Sprite backSprite;

        // Typing
        [SerializeField] GenData.Type type1;
        [SerializeField] GenData.Type type2;

        // Base Stats
        [SerializeField] int maxHp;
        [SerializeField] int atk;
        [SerializeField] int def;
        [SerializeField] int spa;
        [SerializeField] int spd;
        [SerializeField] int spe;

        [SerializeField] List<LearnableMove> learnableMoves;

        // Name and Description
        public string SpeciesName {
            get { return speciesName; }
        }

        public string Description {
            get { return description; }
        }

        // Sprite data
        public Sprite FrontSprite {
            get { return frontSprite; }
        }

        public Sprite BackSprite {
            get { return backSprite; }
        }

        // Base Stats
        public int MaxHp {
            get { return maxHp; }
        }

        public int Atk {
            get { return atk; }
        }

        public int Def {
            get { return def; }
        }

        public int SpA {
            get { return spa; }
        }

        public int SpD {
            get { return spd; }
        }

        public int Spe {
            get { return spe; }
        }

        // Move data
        public List<LearnableMove> LearnableMoves {
            get { return learnableMoves;}
        }
    }

    [System.Serializable]
    public class LearnableMove{
        [SerializeField] MoveData moveData;
        [SerializeField] int level;

        public MoveData Base {
            get { return moveData; }
        }

        public int Level {
            get { return level;}
        }
    }

}
