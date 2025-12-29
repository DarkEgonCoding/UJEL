
using System;
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
        private static string _psRoot = Application.dataPath + "/StreamingAssets/ps/";
        private static string _battlePath = _psRoot + "ps-battle-" +
            (Application.platform == RuntimePlatform.WindowsPlayer ? "win.exe" :
            Application.platform == RuntimePlatform.LinuxPlayer ? "linux" :
            Application.platform == RuntimePlatform.WindowsEditor ? "win.exe" :
            Application.platform == RuntimePlatform.LinuxEditor ? "linux" :
            throw new Exception("Invalid platform! Can only be run on linux or windows!"));

        public Battle()
        {
        }

        private void OnError(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
                UnityEngine.Debug.LogError($"Node error: {args.Data}");
        }

        public void Start(DataReceivedEventHandler dataHandler, string p1spec, string p2spec, string formatid)
        {
            // Spawn the simulator.
            UnityEngine.Debug.Log($"Starting server using nodejs with path {_battlePath}");
            _battle = new Process();
            _battle.StartInfo.FileName = _battlePath;
            _battle.StartInfo.Arguments = "";
            _battle.StartInfo.UseShellExecute = false;
            _battle.StartInfo.RedirectStandardInput = true;
            _battle.StartInfo.RedirectStandardOutput = true;
            _battle.StartInfo.RedirectStandardError = true;
            _battle.StartInfo.CreateNoWindow = true;

            _battle.OutputDataReceived += dataHandler;
            _battle.ErrorDataReceived += OnError;

            _battle.Start();
            _battle.BeginOutputReadLine();
            _battle.BeginErrorReadLine();

            _battle.StandardInput.WriteLine(">start {\"formatid\":\"" + formatid + "\"}");
            _battle.StandardInput.WriteLine(">player p1 " + p1spec);
            _battle.StandardInput.WriteLine(">player p2 " + p2spec);
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
