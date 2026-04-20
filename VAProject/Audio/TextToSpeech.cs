using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using VAProject.Logger;

namespace VAProject.Audio
{
    internal class TextToSpeech
    {
        private readonly string _piperExePath;
        private readonly string _modelPath;
        private readonly ILogger _logger;

        public TextToSpeech(ILogger logger)
        {
            _logger = logger;

            _piperExePath = "Models\\PiperTTS\\piper.exe";
            _modelPath = "Models\\PiperTTS\\en_US-norman-medium.onnx";

            if (!File.Exists(_piperExePath) || !File.Exists(_modelPath))
            {
                _logger.Log($"Piper TTS executable or model not found. Please ensure both {_piperExePath} and {_modelPath} exist.", LogLevel.Error);
            }
        }

        public string CreatePhraseFile(string text, string outputPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _piperExePath,
                Arguments = $"--model {_modelPath} --output_file \"{outputPath}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
            };

            Process process = Process.Start(startInfo);

            if (process != null)
            {
                process.StandardInput.WriteLine(text);
                process.StandardInput.Close();

                string errorOutput = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    _logger.Log($"Piper TTS error: {errorOutput}", LogLevel.Error);
                    return string.Empty;
                }
            }

            if (File.Exists(outputPath))
            {
                return outputPath;  
            }
            else           
            {
                _logger.Log("Failed to generate TTS audio: Output file not found.", LogLevel.Warning);
                return string.Empty;
            }
        }

        public void PlayAudio(string filePath)
        {
            using (AudioFileReader audioFile = new AudioFileReader(filePath))
            using (WaveOutEvent outputDevice = new WaveOutEvent())
            using (AutoResetEvent playbackFinished = new AutoResetEvent(false))
            {
                outputDevice.Init(audioFile);
                outputDevice.PlaybackStopped += (s, e) => playbackFinished.Set();

                outputDevice.Play();
                playbackFinished.WaitOne();
            }
        }
    }
}