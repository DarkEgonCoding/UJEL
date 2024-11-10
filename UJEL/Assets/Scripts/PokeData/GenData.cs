
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class for different general data, enums and common tables.
    /// Types, physical non-physical, etc...
    /// </summary>
    public class GenData
    {

        public enum Type {
            T_NORMAL = 0,
            T_FIRE,
            T_WATER,
            T_ELECTRIC,
            T_GRASS,
            T_ICE,
            T_FIGHTING,
            T_POISON,
            T_GROUND,
            T_FLYING,
            T_PSYCHIC,
            T_BUG,
            T_ROCK,
            T_GHOST,
            T_DRAGON,
            T_DARK,
            T_STEEL,
            T_FAIRY
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

        public enum DamageType {
            DT_PHYSICAL,
            DT_SPECIAL
        };

        public static int AdjustByMultiplier(int damage, int multiplier) {
            if (multiplier >= 0) {
                return Mathf.FloorToInt(damage + damage * 0.5f * multiplier);
            } else {
                return Mathf.FloorToInt(damage / -multiplier);
            }
        }
    }
}

