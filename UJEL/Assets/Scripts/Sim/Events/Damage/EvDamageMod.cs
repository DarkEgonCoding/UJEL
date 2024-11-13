
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class representing a single event that happened in a battle.
    /// </summary>
    public class EvDamageModifier : Event
    {
        /// <summary>
        /// Provides an interface for events that would modify move damage to do so.
        /// </summary>
        public virtual int ModifyPower(int power) {
            return power;
        }
    }

}

