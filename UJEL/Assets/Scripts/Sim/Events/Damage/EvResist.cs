
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class representing a single event that happened in a battle.
    /// </summary>
    public class EvResist : EvDamageMod
    {
        private Move mv;
        private Pokemon src;
        private Pokemon target;
        public float mult;

        /// <summary>
        /// Constructs a new effectiveness event.
        /// </summary>
        public EvResist(Battle battle, Move mv, Pokemon src, Pokemon target, float mult) {
            this.mv = mv;
            this.src = src;
            this.target = target;
            this.mult = mult;
        }

        /// <summary>
        /// Multiply the power by the multiplier.
        /// </summary>
        public override int ModifyPower(int power) {
            return Mathf.FloorToInt(mult * power);
        }

        /// <summary>
        /// Returns the human readable string description of the event.
        /// </summary>
        public override string[] GetDescription() {
            return new string[] {
                "But it was not very effective."
            };
        }
    }
}

