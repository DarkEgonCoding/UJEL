
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Concurrent;
using System.Diagnostics;
using UnityEngine;

namespace PsLib
{
    public class Battle
    {
        // Battle process
        private Process _battle;

        // Paths
        private static string _psRoot = Application.dataPath + "/StreamingAssets/";
        private static string _nodePath = _psRoot + "node/" +
            (Application.platform == RuntimePlatform.WindowsPlayer ? "node-win-x64.exe" :
            Application.platform == RuntimePlatform.LinuxPlayer ? "node-linux-x64" :
            Application.platform == RuntimePlatform.WindowsEditor ? "node-win-x64.exe" :
            Application.platform == RuntimePlatform.LinuxEditor ? "node-linux-x64" :
            throw new Exception("Invalid platform! Can only be run on linux or windows!"));
        private static string _battlePath = _psRoot + "pokemon-showdown/battle.js";
        
        // Event queue
        private ConcurrentQueue<Sim.Message> _msgQ;
        private ConcurrentQueue<string> _simStream;

        public bool TryGetMessage(out Sim.Message msg)
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
            _msgQ = new ConcurrentQueue<Sim.Message>();
            UnityEngine.Debug.Log(Application.platform);
        }

        private void OnData(object sender, DataReceivedEventArgs args) {
            _simStream.Enqueue(args.Data);
            UnityEngine.Debug.Log(args.Data);
        }

        private void OnError(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
                UnityEngine.Debug.LogError($"Node error: {args.Data}");
        }

        public void Start(string p1spec, string p2spec) {
            // Spawn the simulator.
            UnityEngine.Debug.Log($"Starting server using nodejs with path {_battlePath}");
            _battle = new Process();
            _battle.StartInfo.FileName = _nodePath;
            _battle.StartInfo.Arguments = _battlePath;
            _battle.StartInfo.UseShellExecute = false;
            _battle.StartInfo.RedirectStandardInput = true;
            _battle.StartInfo.RedirectStandardOutput = true;
            _battle.StartInfo.RedirectStandardError = true;
            _battle.StartInfo.CreateNoWindow = true;
            _battle.Start();
            _battle.OutputDataReceived += OnData;
            _battle.ErrorDataReceived += OnError;
            _battle.BeginOutputReadLine();
            _battle.BeginErrorReadLine();

            _battle.StandardInput.WriteLine(">start {\"formatid\":\"gen7ou\"}");
            _battle.StandardInput.WriteLine($">player p1 {{\"name\":\"Player1\",\"team\":\"{p1spec}\"}}");
            _battle.StandardInput.WriteLine($">player p2 {{\"name\":\"Player2\",\"team\":\"{p2spec}\"}}");
            _battle.StandardInput.WriteLine(">p1 team 123456");
            _battle.StandardInput.WriteLine(">p2 team 123456");

            _msgQ = new ConcurrentQueue<Sim.Message>();
            _simStream = new ConcurrentQueue<string>();
        }

        public void WriteLine(string line)
        {
            if (_battle == null || _battle.HasExited)
            {
                UnityEngine.Debug.LogWarning("Tried to write to battle process after it exited.");
                return;
            }

            _battle.StandardInput.WriteLine(line);
        }

        ~Battle()
        {
            if (_battle != null) { _battle.Kill(); }
        }
    }
}
