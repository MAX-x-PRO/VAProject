using System.Diagnostics;
using System.IO;
using NAudio.Wave;

namespace VAProject.Audio
{
    internal class TextToSpeech
    {
        private readonly string _piperExePath;
        private readonly string _modelPath;

        public TextToSpeech()
        {
            _piperExePath = "Models\\PiperTTS\\piper.exe";
            _modelPath = "Models\\PiperTTS\\en_US-norman-medium.onnx";

            if (!File.Exists(_piperExePath) || !File.Exists(_modelPath))
            {
                throw new FileNotFoundException($"Piper TTS executable or model not found. Please ensure both {_piperExePath} and {_modelPath} exist.");
            }
        }

        public void Speak(string text)
        {
            string tempOutputPath = Path.Combine(Path.GetTempPath(), $"tts_output_{Guid.NewGuid()}.wav");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _piperExePath,
                Arguments = $"--model {_modelPath} --output_file \"{tempOutputPath}\"",
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
                    Console.WriteLine($"Piper TTS error: {errorOutput}"); // LOGS
                    return;
                }
            }

            if (File.Exists(tempOutputPath))
            {
                PlayAudio(tempOutputPath);
                File.Delete(tempOutputPath);
            }
            else           
            {
                Console.WriteLine("Failed to generate TTS audio: Output file not found."); // LOGS
            }
        }

        private void PlayAudio(string filePath)
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