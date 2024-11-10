
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class representing a single event that happened in a battle.
    /// </summary>
    public class EvMove : Event
    {
        private Move mv;
        private Pokemon src;
        private List<Pokemon> targets;

        /// <summary>
        /// Constructs a new move event.
        /// </summary>
        public EvMove(Battle battle, Move mv, Pokemon src, List<Pokemon> targets) {
            this.mv = mv;
            this.src = src;
            this.targets = targets;
        }

        /// <summary>
        /// Applys an event's effects to the battle.
        /// </summary>
        public override void Apply() {
            return;
        }

        /// <summary>
        /// Gets the description string that can be desplayed to the player.
        /// Returns null if the event should go by without description.
        /// </summary>
        public override string[] GetDescription() {
            return new string[] {
                ""
            };
        }
    }
}

