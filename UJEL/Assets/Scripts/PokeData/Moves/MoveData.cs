
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    [CreateAssetMenu(fileName = "new_move", menuName = "Pokemon/Create new Move")]
    public class MoveData : ScriptableObject
    {
        [SerializeField] string moveName;
        [TextArea]
        [SerializeField] string description;

        // Typing
        [SerializeField] GenData.Type type;
        [SerializeField] GenData.DamageType dtype;

        // Stats
        [SerializeField] int power;
        [SerializeField] int accuracy;
        [SerializeField] int pp;

        public string Name {
            get { return name; }
        }

        public string Description {
            get { return description; }
        }

        public GenData.Type Type {
            get { return type; }
        }

        public GenData.DamageType DamageType {
            get { return type; }
        }

        public int Power {
            get { return power; }
        }

        public int Accuracy {
            get { return accuracy; }
        }

        public int PP {
            get { return pp; }
        }
    }

}
