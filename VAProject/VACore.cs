using VAProject.Audio;
using VAProject.CommandsLogic;

namespace VAProject
{
    internal class VACore
    {
        private readonly AudioCapturer _audioCapturer;
        private readonly SpeechToText _speechToText;
        private readonly CommandRouter _commandRouter;

        public VACore()
        {
            _audioCapturer = new AudioCapturer();
            _speechToText = new SpeechToText();
            _commandRouter = new CommandRouter();

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

            Console.WriteLine($"Recognized Text: {recognizedText}"); //LOGS

            if (!string.IsNullOrEmpty(recognizedText))
            {
                _commandRouter.RouteInput(recognizedText);
            }
        }
    }
}
