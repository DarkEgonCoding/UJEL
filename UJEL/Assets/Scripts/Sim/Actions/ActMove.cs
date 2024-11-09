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

        /// <summary>
        /// Constructs an instance of an action.
        /// </summary>
        public ActMove(Move mv) {
            this.mv = mv;
        }

        /// <summary>
        /// Appends the move events to the event list.
        /// </summary>
        public void Do(Queue<Event> events) {
            //events.Add(new EvMove());
            return;
        }
    }
}
