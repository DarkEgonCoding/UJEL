
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class representing a single event that happened in a battle.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Boolean representing whether or not this event has been canceled.
        /// Canceled events can mean different things for different event types.
        /// It is up to the implementer of Apply to determine how "canceled" behaves.
        /// </summary>
        public bool Canceled { get; set; }

        /// <summary>
        /// The default constructor for a class. All subclasses should implement this.
        /// </summary>
        public Event() {
        }

        /// <summary>
        /// Applys an event's effects to the battle.
        /// </summary>
        public void Apply() {
            
        }

        /// <summary>
        /// Gets the description string that can be desplayed to the player.
        /// Returns null if the event should go by without description.
        /// </summary>
        public string[] GetDescription() {
            return new string[0];
        }
    }
}
