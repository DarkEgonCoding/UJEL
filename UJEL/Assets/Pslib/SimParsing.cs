
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace PsLib.Sim
{
    // A class that handles parsing data as defined by SimActions.cs.
    class Parser
    {
        private const string majorActionNamespace = "PsLib.Sim.Messages.Actions.Major";
        private const string minorActionNamespace = "PsLib.Sim.Messages.Actions.Minor";

        private Type[] majorActionTypes;
        private Type[] minorActionTypes;

        private Dictionary<string, Type> majorActionMapping;
        private Dictionary<string, Type> minorActionMapping;

        public Parser()
        {
            majorActionMapping = new Dictionary<string, Type>();
            minorActionMapping = new Dictionary<string, Type>();

            majorActionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
                    t => t.IsClass && t.Namespace == majorActionNamespace).ToArray();
            minorActionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
                    t => t.IsClass && t.Namespace == minorActionNamespace).ToArray();

            // Loop through each of the major actions and add the info to the dictionary.
            foreach (Type majorAction in majorActionTypes) {
                Messages.StreamText textAttr = majorAction.
                    GetCustomAttribute<Messages.StreamText>();

                if (textAttr != null) {
                    majorActionMapping.Add(textAttr.text, majorAction);
                } else {
                    majorActionMapping.Add(majorAction.Name.ToLower(), majorAction);
                }
            }

            // Loop through each of the minor actions and add the info to the dictionary.
            foreach (Type minorAction in minorActionTypes) {
                Messages.StreamText textAttr = minorAction.
                    GetCustomAttribute<Messages.StreamText>();

                if (textAttr != null) {
                    minorActionMapping.Add(textAttr.text, minorAction);
                } else {
                    minorActionMapping.Add(minorAction.Name.ToLower(), minorAction);
                }
            }
        }

        // Given a string, returns a message and the specific type of the message.
        // Removes the message from lines if possible.
        public bool TryParseMessage(string text, out Messages.Message msg)
        {
            string[] split = text.Split('|');
            Type ev;

            // TODO: Fix me, don't want to assign null mesage.
            msg = null;

            if (text[0] != '|' || split.Length < 2)
            {
                return false;
            }

            // Handle major actions.
            if (majorActionMapping.TryGetValue(split[1], out ev)) {
                PropertyInfo[] properties = ev.GetProperties();
                return true;
            }

            // Handle minor actions
            if (minorActionMapping.TryGetValue(split[1], out ev)) {
                PropertyInfo[] properties = ev.GetProperties();
                return true;
            }

            // Return false on failed parse.
            return false;
        }
    }
}
