
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class representing a single event that happened in a battle.
    /// </summary>
    public class EvWeak : Event
    {
        private Move mv;
        private Pokemon src;
        private Pokemon target;
        private float mult;

        /// <summary>
        /// Constructs a new effectiveness event.
        /// </summary>
        public EvWeak(Battle battle, Move mv, Pokemon src, Pokemon target, float mult) {
            this.mv = mv;
            this.src = src;
            this.target = target;
            this.mult = mult;
        }

        /// <summary>
        /// Returns the human readable string description of the event.
        /// </summary>
        public override string[] GetDescription() {
            return new string[] {
                "And it was super effective."
            };
        }
    }
}

