using System.IO;
using System.Text.Json;
using VAProject.Logger;
using Vosk;

namespace VAProject.Audio
{
    internal class SpeechToText
    {
        private readonly ILogger _logger;
        private readonly Model _voskModel;
        private readonly VoskRecognizer _recognizer;

        public SpeechToText(ILogger logger)
        {
            _logger = logger;

            string modelPath = "Models\\vosk-model-small-en-us-0.15";
            
            if (!Directory.Exists(modelPath))
            {
                throw new DirectoryNotFoundException($"Vosk model directory not found: {modelPath}");
            }
            
            _logger.Log("Loading Vosk model...", LogLevel.Debug);

            Vosk.Vosk.SetLogLevel(-1);

            _voskModel  = new Model(modelPath);
            _recognizer = new VoskRecognizer(_voskModel, 16000.0f);

            _logger.Log("Vosk model loaded successfully.", LogLevel.Debug);
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
