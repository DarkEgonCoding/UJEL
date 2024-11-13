
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim
{
    /// <summary>
    /// A class that represents a move action that can be taken during a battle.
    /// </summary>
    public class ActMove
    {
        private MoveData mv;
        private Battle battle;
        private Pokemon src;
        private List<Pokemon> targets;

        /// <summary>
        /// Constructs an instance of an action.
        /// </summary>
        public ActMove(Battle battle, MoveData mv, Pokemon src, List<Pokemon> targets) {
            this.mv = mv;
            this.battle = battle;
        }

        /// <summary>
        /// Appends the move events to the event list.
        /// </summary>
        public void Do() {
            // Declare that the move was intended to be used.
            EvMove moveEvent = new EvMove(battle, mv, src, targets);
            battle.EnququeEvent((Event) moveEvent);

            // Check if the move was canceled.
            if (moveEvent.Canceled) {
                return;
            }

            // If the move wasn't canceled. Push its effects to the event list.
            foreach (Pokemon target in targets) {
                EvDamage dmgEvent = new EvDamage(battle, target, mv.DamageType, mv.Power);
                battle.EnququeEvent((Event) dmgEvent);

                float effect = GenData.GetEffectiveness(mv.Type, target.Species.Type1);
                if (target.Species.Type2 != GenData.Type.T_NULL) {
                    effect *= GenData.GetEffectiveness(mv.Type, target.Species.Type2);
                }

                if (effect > 1.1f) {
                    battle.
                } else if {
                    
                }
            }
            return;
        }
    }
}
