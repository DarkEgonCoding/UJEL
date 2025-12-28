
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using UnityEngine;

namespace PsLib
{
    public class Battle
    {
        // Battle process
        private Process _battle;
        private string messageBuffer = "";

        // Paths
        private static string _psRoot = Application.dataPath + "/StreamingAssets/";
        private static string _nodePath = _psRoot + "node/" +
            (Application.platform == RuntimePlatform.WindowsPlayer ? "node-win-x64.exe" :
            Application.platform == RuntimePlatform.LinuxPlayer ? "node-linux-x64" :
            Application.platform == RuntimePlatform.WindowsEditor ? "node-win-x64.exe" :
            Application.platform == RuntimePlatform.LinuxEditor ? "node-linux-x64" :
            throw new Exception("Invalid platform! Can only be run on linux or windows!"));
        private static string _battlePath = _psRoot +
            "pokemon-showdown/node_modules/pokemon-showdown simulate-battle";
        
        // Event queue
        private ConcurrentQueue<Sim.Messages.Message> _msgQ;
        private ConcurrentQueue<string> _simStream;

        public bool TryGetMessage(out Sim.Messages.Message msg)
        {
            return _msgQ.TryDequeue(out msg);
        }

        public bool TryGetRaw(out string raw)
        {
            return _simStream.TryDequeue(out raw);
        }

        public Battle(
            
        ) {
            // Initialize the queue semaphore.
            _msgQ = new ConcurrentQueue<Sim.Messages.Message>();
            UnityEngine.Debug.Log(Application.platform);
        }

        private void OnData(object sender, DataReceivedEventArgs args) {
            _simStream.Enqueue(args.Data);
            UnityEngine.Debug.Log(args.Data);
        }

        private void OnError(object sender, DataReceivedEventArgs args) {
            UnityEngine.Debug.Log(args.Data);
        }

        public void Start(string p1spec, string p2spec, string formatid) {
            // Spawn the simulator.
            UnityEngine.Debug.Log($"Starting server using nodejs with path {_battlePath}");
            _battle = new Process();
            _battle.StartInfo.FileName = _nodePath;
            _battle.StartInfo.Arguments = _battlePath;
            _battle.StartInfo.UseShellExecute = false;
            _battle.StartInfo.RedirectStandardInput = true;
            _battle.StartInfo.RedirectStandardOutput = true;
            _battle.StartInfo.RedirectStandardError = true;
            _battle.Start();
            _battle.OutputDataReceived += OnData;
            _battle.ErrorDataReceived += OnError;
            _battle.BeginOutputReadLine();
            _battle.BeginErrorReadLine();

            _battle.StandardInput.WriteLine(">start {\"formatid\":\"" + formatid + "\"}");
            _battle.StandardInput.WriteLine(">player p1 " + "");
            _battle.StandardInput.WriteLine(">player p2 " + "");

            _msgQ = new ConcurrentQueue<Sim.Messages.Message>();
            _simStream = new ConcurrentQueue<string>();
        }

        ~Battle() {
            if (_battle != null) { _battle.Kill(); }
        }
    }
}
