using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using UnityEngine;

namespace PsLib
{
    public class PokemonDataServer
    {
        private Process _dataServer;

        // Paths
        private static string _psRoot = Application.dataPath + "/StreamingAssets/";
        private static string _nodePath = _psRoot + "node/" +
            (Application.platform == RuntimePlatform.WindowsPlayer ? "node-win-x64.exe" :
            Application.platform == RuntimePlatform.LinuxPlayer ? "node-linux-x64" :
            Application.platform == RuntimePlatform.WindowsEditor ? "node-win-x64.exe" :
            Application.platform == RuntimePlatform.LinuxEditor ? "node-linux-x64" :
            throw new Exception("Invalid platform! Can only be run on linux or windows!"));
        private static string _dataServerPath = _psRoot + "pokemon-showdown/PokemonDataServer.js";

        // Event queue
        private ConcurrentQueue<string> _outputQueue;

        public bool TryGetOutput(out string output)
        {
            return _outputQueue.TryDequeue(out output);
        }

        public PokemonDataServer()
        {
            _outputQueue = new ConcurrentQueue<string>();
            UnityEngine.Debug.Log(Application.platform);
        }

        private void OnData(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                _outputQueue.Enqueue(args.Data);
                UnityEngine.Debug.Log($"[PokemonDataServer] {args.Data}");
            }
        }

        private void OnError(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
                UnityEngine.Debug.LogError($"[PokemonDataServer Error] {args.Data}");
        }

        public void Start()
        {
            UnityEngine.Debug.Log($"Starting Pokemon Data Server: {_dataServerPath}");
            _dataServer = new Process();
            _dataServer.StartInfo.FileName = _nodePath;
            _dataServer.StartInfo.Arguments = _dataServerPath;
            _dataServer.StartInfo.UseShellExecute = false;
            _dataServer.StartInfo.RedirectStandardInput = true;
            _dataServer.StartInfo.RedirectStandardOutput = true;
            _dataServer.StartInfo.RedirectStandardError = true;
            _dataServer.StartInfo.CreateNoWindow = true;

            _dataServer.OutputDataReceived += OnData;
            _dataServer.ErrorDataReceived += OnError;

            _dataServer.Start();
            _dataServer.BeginOutputReadLine();
            _dataServer.BeginErrorReadLine();

            _outputQueue = new ConcurrentQueue<string>();
        }

        public void WriteLine(string line)
        {
            if (_dataServer == null || _dataServer.HasExited)
            {
                UnityEngine.Debug.LogWarning("Tried to write to data server after it exited.");
                return;
            }

            _dataServer.StandardInput.WriteLine(line);
        }

        ~PokemonDataServer()
        {
            if (_dataServer != null) { _dataServer.Kill(); }
        }
    }
}
