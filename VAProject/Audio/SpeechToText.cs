using System.IO;
using System.Text.Json;
using Vosk;

namespace VAProject.Audio
{
    internal class SpeechToText
    {
        private readonly Model _voskModel;
        private readonly VoskRecognizer _recognizer;

        public SpeechToText()
        {
            string modelPath = "Models\\vosk-model-small-en-us-0.15";
            
            if (!Directory.Exists(modelPath))
            {
                throw new DirectoryNotFoundException($"Vosk model directory not found: {modelPath}");
            }
            
            Console.WriteLine("Loading Vosk model..."); //LOGS

            Vosk.Vosk.SetLogLevel(-1);

            _voskModel  = new Model(modelPath);
            _recognizer = new VoskRecognizer(_voskModel, 16000.0f);

            Console.WriteLine("Vosk model loaded successfully."); //LOGS
        }

        public string RecognizeFromMemory(byte[] audioData)
        {
            _recognizer.AcceptWaveform(audioData, audioData.Length);
            string jsonResult = _recognizer.Result(); 

            string recognizedText = JsonDocument.Parse(jsonResult).RootElement.GetProperty("text").GetString();

            return recognizedText ?? String.Empty;
        }

    }
}
