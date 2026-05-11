using VAProject.Audio;
using VAProject.CommandsLogic;
using VAProject.Logger;
using VAProject.UI;

namespace VAProject
{
    internal class VACore
    {
        public AudioCapturer AudioCapturer { get; }

        private readonly SpeechToText _speechToText;
        private readonly CommandRouter _commandRouter;

        public VACore()
        {
            LogManager.Initialize();
            NotificationManager.Initialize(new NotificationWindow());

            AudioCapturer = new AudioCapturer();
            _speechToText = new SpeechToText();
            _commandRouter = new CommandRouter();

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
