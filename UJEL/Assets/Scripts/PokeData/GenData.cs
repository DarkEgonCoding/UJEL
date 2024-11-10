
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class representing a single event that happened in a battle.
    /// </summary>
    public class GenData
    {
        public enum Type {
            NORMAL = 0,
            FIRE,
            WATER,
            ELECTRIC,
            GRASS,
            ICE,
            FIGHTING,
            POISON,
            GROUND,
            FLYING,
            PSYCHIC,
            BUG,
            ROCK,
            GHOST,
            DRAGON,
            DARK,
            STEEL,
            FAIRY
        };

        private static float[,] _typeChart = new float[,] {
/*            NOR  FIR  WAT  ELE  GRA  ICE  FIG  POI  GRO  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE  FAI */
/*NORMAL  */{ 1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,0.0f,1.0f,1.0f,0.5f,1.0f },
/*FIRE    */{ 1.0f,0.5f,0.5f,1.0f,2.0f,2.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,0.5f,1.0f,2.0f,1.0f },
/*WATER   */{ 1.0f,2.0f,0.5f,1.0f,0.5f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,1.0f,1.0f },
/*ELECTRIC*/{ 1.0f,1.0f,2.0f,0.5f,0.5f,1.0f,1.0f,1.0f,0.0f,2.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f },
/*GRASS   */{ 1.0f,0.5f,2.0f,1.0f,0.5f,1.0f,1.0f,0.5f,2.0f,0.5f,1.0f,0.5f,2.0f,1.0f,0.5f,1.0f,0.5f,1.0f },
/*ICE     */{ 1.0f,0.5f,0.5f,1.0f,2.0f,0.5f,1.0f,1.0f,2.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f },
/*FIGHTING*/{ 2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,0.5f,0.5f,0.5f,2.0f,0.0f,1.0f,2.0f,2.0f,0.5f },
/*POISON  */{ 1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,0.5f,0.5f,1.0f,1.0f,1.0f,0.5f,0.5f,1.0f,1.0f,0.0f,2.0f },
/*GROUND  */{ 1.0f,2.0f,1.0f,2.0f,0.5f,1.0f,1.0f,2.0f,1.0f,0.0f,1.0f,0.5f,2.0f,1.0f,1.0f,1.0f,2.0f,1.0f },
/*FLYING  */{ 1.0f,1.0f,1.0f,0.5f,2.0f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,1.0f,1.0f,0.5f,1.0f },
/*PSYCHIC */{ 1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,2.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,1.0f,0.0f,0.5f,1.0f },
/*BUG     */{ 1.0f,0.5f,1.0f,1.0f,2.0f,1.0f,0.5f,0.5f,1.0f,0.5f,2.0f,1.0f,1.0f,0.5f,1.0f,2.0f,0.5f,0.5f },
/*ROCK    */{ 1.0f,2.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,0.5f,2.0f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f },
/*GHOST   */{ 0.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,1.0f },
/*DRAGON  */{ 1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,0.5f,0.0f },
/*DARK    */{ 1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.5f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,2.0f,1.0f,0.5f,1.0f,0.5f },
/*STEEL   */{ 1.0f,0.5f,0.5f,0.5f,1.0f,2.0f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,1.0f,1.0f,1.0f,0.5f,2.0f },
/*FAIRY   */{ 1.0f,0.5f,1.0f,1.0f,1.0f,1.0f,2.0f,0.5f,1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,2.0f,2.0f,0.5f,1.0f }
        };

        public static float GetEffectiveness(Type attacker, Type defender) {
            return _typeChart[(int) attacker, (int) defender];
        }
    }
}

