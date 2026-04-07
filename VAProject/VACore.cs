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
            _audioCapturer = new AudioCapturer();
            _speechToText = new SpeechToText();
            _commandRouter = new CommandRouter();
            _logger = new Logger(logLevel: 0);

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

            _logger.Log($"Recognized Text: {recognizedText}", 0);

            if (!string.IsNullOrEmpty(recognizedText))
            {
                _commandRouter.RouteInput(recognizedText);
            }
        }
    }
}
