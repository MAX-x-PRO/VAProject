using VAProject.Core.Audio;
using VAProject.Core.CommandsLogic;
using VAProject.Core.Interfaces;
using VAProject.Core.Logger;

namespace VAProject.Core
{
    public class VACore
    {
        public AudioCapturer AudioCapturer { get; }

        private readonly INotificationService _notificationService;
        private readonly SpeechToText _speechToText;
        private readonly CommandRouter _commandRouter;

        public VACore(INotificationService notificationService, CommandRouter commandRouter)
        {
            LogManager.Initialize();
            _notificationService = notificationService;
            _commandRouter = commandRouter;

            AudioCapturer = new AudioCapturer();
            _speechToText = new SpeechToText();

            AudioCapturer.OnCommandAudioCaptured += HandleCapturedAudio;
        }

        public void Start()
        {
            AudioCapturer.StartListening();
        }
        
        public void Stop()
        {
            AudioCapturer.StopListening();
        }

        private void HandleCapturedAudio(byte[] audioData)
        {
            string recognizedText = _speechToText.RecognizeFromMemory(audioData);

            LogManager.Log($"Recognized Text: {recognizedText}", LogLevel.Debug);

            if (!string.IsNullOrEmpty(recognizedText))
            {
                _commandRouter.RouteInput(recognizedText);
            }
        }
    }
}
