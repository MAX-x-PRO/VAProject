using VAProject.Core.Audio;
using VAProject.Core.CommandsLogic;
using VAProject.Core.Interfaces;
using VAProject.Core.Logger;
using VAProject.Core.Utils.EventBus;
using VAProject.Core.Utils.EventBus.Events;

namespace VAProject.Core
{
    public class VACore
    {
        public AudioCapturer AudioCapturer { get; }

        private readonly INotificationService _notificationService;
        private readonly SpeechToText _speechToText;
        private readonly CommandRouter _commandRouter;
        private readonly EventBus _eventBus;

        private readonly ISubscription _audioCaptureEventSubscription;

        public VACore(INotificationService notificationService, CommandRouter commandRouter, EventBus eventBus)
        {
            LogManager.Initialize();
            _notificationService = notificationService;
            _commandRouter = commandRouter;
            _eventBus = eventBus;

            _speechToText = new SpeechToText();
            AudioCapturer = new AudioCapturer(_eventBus);

            _audioCaptureEventSubscription = _eventBus.Subscribe<CommandAudioCapturedEvent>((msg) => HandleCapturedAudio(msg.AudioData));
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
