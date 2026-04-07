using VAProject.Audio;
using VAProject.CommandsLogic;
using VAProject.Logger;

namespace VAProject
{
    internal class VACore
    {
        private readonly AudioCapturer _audioCapturer;
        private readonly SpeechToText _speechToText;
        private readonly CommandRouter _commandRouter;
        private readonly ILogger _logger;

        public VACore()
        {
            _logger = new Logger.Logger(logLevel: 0);

            _audioCapturer = new AudioCapturer(_logger);
            _speechToText = new SpeechToText(_logger);
            _commandRouter = new CommandRouter(_logger);

            _audioCapturer.OnCommandAudioCaptured += HandleCapturedAudio;
        }

        public void Start()
        {
            _audioCapturer.StartListening();
        }
        
        public void Stop()
        {
            _audioCapturer.StopListening();
        }

        private void HandleCapturedAudio(byte[] audioData)
        {
            string recognizedText = _speechToText.RecognizeFromMemory(audioData);

            _logger.Log($"Recognized Text: {recognizedText}", LogLevel.Debug);

            if (!string.IsNullOrEmpty(recognizedText))
            {
                _commandRouter.RouteInput(recognizedText);
            }
        }
    }
}
