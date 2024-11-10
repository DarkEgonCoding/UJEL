
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class representing a single event that happened in a battle.
    /// </summary>
    public class EvEffectiveness : Event
    {
        private Move mv;
        private Pokemon src;
        private List<Pokemon> targets;

        /// <summary>
        /// Constructs a new move event.
        /// </summary>
        public EvTyping(Battle battle, Move mv, Pokemon src, List<Pokemon> targets) {
            this.mv = mv;
            this.src = src;
            this.targets = targets;
        }

        public override string[] GetDescription() {
            return new string[] {
                "And it was super effective."
            };
        }
    }
}

