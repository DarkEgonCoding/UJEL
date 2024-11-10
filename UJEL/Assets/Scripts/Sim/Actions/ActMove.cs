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
        private Move mv;
        private Battle battle;
        private Pokemon src;
        private List<Pokemon> targets;

        /// <summary>
        /// Constructs an instance of an action.
        /// </summary>
        public ActMove(Battle battle, Move mv, Pokemon src, List<Pokemon> targets) {
            this.mv = mv;
            this.battle = battle;
        }

        /// <summary>
        /// Appends the move events to the event list.
        /// </summary>
        public void Do() {
            battle.EnququeEvent((Event) new EvMove(battle, mv, src, targets));
            return;
        }
    }
}
