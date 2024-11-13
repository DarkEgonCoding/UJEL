
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class representing damage done to a pokemon. Damage should then be amplified by crits,
    /// abilities, status modifiers, etc...
    /// </summary>
    public class EvDamage : Event
    {
        private Battle battle;
        private Pokemon target;
        private GenData.DamageType dtype;
        private int power;

        /// <summary>
        /// The default constructor for a class. All subclasses should implement this.
        /// </summary>
        public EvDamage(Battle battle, Pokemon target, GenData.DamageType dtype, int power) {
        }

        /// <summary>
        /// Applys an event's effects to the battle.
        /// </summary>
        public override void Apply() {

        }

        /// <summary>
        /// Gets the description string that can be desplayed to the player.
        /// Returns null if the event should go by without description.
        /// </summary>
        public override string[] GetDescription() {
            return null;
        }
    }
}

