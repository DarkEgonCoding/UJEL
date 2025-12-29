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
        private static string _psRoot = Application.dataPath + "/StreamingAssets/ps/";
        private static string _dataPath = _psRoot + "ps-data-" +
            (Application.platform == RuntimePlatform.WindowsPlayer ? "win.exe" :
            Application.platform == RuntimePlatform.LinuxPlayer ? "linux" :
            Application.platform == RuntimePlatform.WindowsEditor ? "win.exe" :
            Application.platform == RuntimePlatform.LinuxEditor ? "linux" :
            throw new Exception("Invalid platform! Can only be run on linux or windows!"));

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
            UnityEngine.Debug.Log($"Starting Pokemon Data Server: {_dataPath}");
            _dataServer = new Process();
            _dataServer.StartInfo.FileName = _dataPath;
            _dataServer.StartInfo.Arguments = "";
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
