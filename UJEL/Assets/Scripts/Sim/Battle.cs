
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sim {

    public enum TurnPhase {
        TP_PRE_TURN,
        TP_MOVE,
        TP_MOVE_RESOLVE,
        TP_WEATHER,
        TP_STATUSES,
        TP_POST_TURN
    };

    public enum Player {
        P_ONE = 0,
        P_TWO = 1
    };

    public class Battle {
        private Pokemon[,] teams;
        private Action[] selectedActions;
        private List<SimListener> listeners;
        private Queue<Event> events;

        /// <summary>
        /// Constructs an instance of a battle.
        /// </summary>
        public Battle() { listeners = new List<SimListener>();
            events = new Queue<Event>();

            teams = new Pokemon[2, 6];
            selectedActions = new Action[2];
        }

        /// <summary>
        /// Attempts to set the action for a given player.
        /// </summary>
        public bool SetAction(Player player, Action selected) {
            return true;
        }

        /// <summary>
        /// Calculates the effects of a single turn.
        /// </summary>
        public Queue<Event> DoTurn() {
            return events;
        }

        /// <summary>
        /// Pushes an event to the event queue.
        /// </summary>
        public void EnququeEvent(Event ev) {
            events.Enqueue(ev);
            foreach (SimListener listener in listeners) {
                listener.NotifyEventPush(ev);
            }
        }

        public void AppendModifier(Event ev) {

        }

        public Event DequeueEvent() {
            return events.Dequeue();
        }
    }
}

