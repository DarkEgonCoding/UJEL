
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace PsLib.Sim
{
    // A class that handles parsing data as defined by SimActions.cs.
    class Parser
    {
        private const string initMessageNamespace = "PsLib.Sim.Messages.Init";
        private const string progMessageNamespace = "PsLib.Sim.Messages.Progress";
        private const string majorActionNamespace = "PsLib.Sim.Messages.Major";
        private const string minorActionNamespace = "PsLib.Sim.Messages.Minor";
        private PsLib.Sim.Messages.MessageGroup[] groups;

        private Type[] initMessageTypes;
        private Type[] progMessageTypes;
        private Type[] majorActionTypes;
        private Type[] minorActionTypes;
        private Type[][] types;

        private Dictionary<string, Type> initMessageMapping;
        private Dictionary<string, Type> progMessageMapping;
        private Dictionary<string, Type> majorActionMapping;
        private Dictionary<string, Type> minorActionMapping;
        private Dictionary<string, Type>[] maps;

        private PsLib.Sim.Messages.Stream currentStream;

        public Parser()
        {
            // Prepare arrays to loop over.
            string[] namespaces = {
                initMessageNamespace, progMessageNamespace,
                majorActionNamespace, minorActionNamespace
            };

            initMessageMapping = new Dictionary<string, Type>();
            progMessageMapping = new Dictionary<string, Type>();
            majorActionMapping = new Dictionary<string, Type>();
            minorActionMapping = new Dictionary<string, Type>();

            initMessageTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
                    t => t.IsClass && t.Namespace == initMessageNamespace).ToArray();
            progMessageTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
                    t => t.IsClass && t.Namespace == progMessageNamespace).ToArray();
            majorActionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
                    t => t.IsClass && t.Namespace == majorActionNamespace).ToArray();
            minorActionTypes = Assembly.GetExecutingAssembly().GetTypes().Where(
                    t => t.IsClass && t.Namespace == minorActionNamespace).ToArray();

            // Prepare arrays to loop over.
            groups = new PsLib.Sim.Messages.MessageGroup[] {
                Messages.MessageGroup.init, Messages.MessageGroup.progress,
                Messages.MessageGroup.major, Messages.MessageGroup.minor
            };
            types = new Type[][] {
                initMessageTypes, progMessageTypes,
                majorActionTypes, minorActionTypes
            };
            maps = new Dictionary<string, Type>[] {
                initMessageMapping, progMessageMapping,
                majorActionMapping, minorActionMapping
            };

            // Populate all of the dictionaries.
            for (int i = 0; i < groups.Length; i++)
            {
                foreach (Type msg in types[i])
                {
                    Messages.StreamText textAttr = msg.
                        GetCustomAttribute<Messages.StreamText>();
                    if (textAttr != null) {
                        maps[i].Add(textAttr.text, msg);
                    } else {
                        maps[i].Add(msg.Name.ToLower(), msg);
                    }
                }
            }
        }

        // Given a string, returns a message and the specific type of the message.
        // Removes the message from lines if possible.
        public bool TryParseMessage(string text, out Messages.Message msg)
        {
            string[] split = text.Split('|');
            Type eventType;
            msg = null;
            
            // Handle all of the stream-updating messages.
            if (text == "update") {
                currentStream = PsLib.Sim.Messages.Stream.update;
                return false;
            } else if (text == "p1") {
                currentStream = PsLib.Sim.Messages.Stream.p1;
                return false;
            } else if (text == "p2") {
                currentStream = PsLib.Sim.Messages.Stream.p2;
                return false;
            }

            // Find the matching action for the input text.
            for (int i = 0; i < groups.Length; i++)
            {
                if (maps[i].TryGetValue(split[1], out eventType)) {
                    // Start the new message.
                    msg = new PsLib.Sim.Messages.Message();
                    msg.stream = currentStream;
                    msg.group = groups[i];

                    // Parse all of the fields based on the properties.
                    PsLib.Sim.Messages.Action action =
                        (PsLib.Sim.Messages.Action)Activator.CreateInstance(eventType);
                    PropertyInfo[] properties = eventType.GetProperties();
                    for (int j = 0; j < properties.Length; j++)
                    {
                        ParseSingleProperty(action, split[j+2], properties[j]);
                    }
                    msg.action = action;

                    return true;
                }
            }

            // Return false on failed parse.
            return false;
        }

        private void ParseSingleProperty(PsLib.Sim.Messages.Action action, string text, PropertyInfo prop)
        {
            prop.SetValue(action, prop.GetType().GetMethod("Parse").Invoke(null, new object[] {text}));
        }
    }
}
