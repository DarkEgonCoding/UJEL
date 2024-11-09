using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    /// <summary>
    /// A class that represents an active listener to the battle.
    /// Listeners can cancel events, modify damage values, push responses, and more.
    /// </summary>
    public class SimListener
    {
        public enum ActionStatus {
            A_ALLOW,
            A_FORBID
        };

        enum ListenerPriority {
            
        };

        // A reference to the battle that this listener belongs to.
        private Battle battle;

        /// <summary>
        /// Constructs an instance of a sim listener.
        /// </summary>
        public SimListener(Battle battle) {
            this.battle = battle;
            return;
        }

        /// <summary>
        /// Called whenever an event is pushed to the queue.
        /// Allows an event listener to react to actions taken by pokemon and to other listeners.
        ///
        /// Useful for things like protect, which need to respond to things like damaging moves.
        /// Does nothing by default.
        /// </summary>
        public void NotifyEventPush(Event ev) {
            return;
        }

        /// <summary>
        /// Called whenever the phase of the battle is changed.
        /// Allows an event listener to react to movement between battle phases.
        ///
        /// Useful for detecting the end of protect or reflect/light-screen fading.
        /// Does nothing by default.
        /// </summary>
        public void NotifyPhaseChange(TurnPhase phase) {
            return;
        }

        /// <summary>
        /// Called whenever a player declares their intent to take an action.
        /// Allows an event listener to react to players taking different actions.
        /// This is used when a user should be notified that their action will fail before it
        /// ever takes place.
        /// </summary>
        public ActionStatus NotifyActionIntent() {
            return ActionStatus.A_ALLOW;
        }
    }

}
