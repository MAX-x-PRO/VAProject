using System.Windows.Automation.Peers;
using System.Windows.Media;
using VAProject.Core.Audio;
using VAProject.Core.Interfaces;
using VAProject.Core.Logger;
using VAProject.Core.Models.Notifications;

namespace VAProject.Core.CommandsLogic
{
    public class ResultHandler
    {
        private readonly INotificationService _notificationManager;

        private readonly AudioCacher _cacher;
        private readonly TextToSpeech _textToSpeech;

        public ResultHandler(INotificationService notificationManager)
        {
            _notificationManager = notificationManager;

            _textToSpeech = new TextToSpeech();
            _cacher = new AudioCacher(_textToSpeech);
        }

        public void HandleCommandResult(CommandResult result)
        {
            LogManager.Log($"Command execution result: {(result.Success ? "Success" : "Failure")}", LogLevel.Debug);
            LogManager.Log(result.LogMessage, LogLevel.Debug);

            //switch (result.CommandType)
            //{
            //    case CommandType.General:
            //        if (!string.IsNullOrEmpty(result.TTSResponse))
            //        {
            //            string phrasePath = _cacher.GetPhrasePath(result.TTSResponse);

            //            if (!string.IsNullOrEmpty(phrasePath))
            //            {
            //                LogManager.Log($"Playing TTS audio for response: {result.TTSResponse}", LogLevel.Info);

            //                TextPayload textPayload = new TextPayload
            //                {
            //                    Text = result.TTSResponse,
            //                    AccentColor = Colors.Green
            //                };
            //                _notificationManager.ShowWidget(textPayload);

            //                _textToSpeech.PlayAudio(phrasePath);
            //            }
            //            else
            //            {
            //                LogManager.Log($"Failed to retrieve audio for TTS response: {result.TTSResponse}", LogLevel.Warning);
            //            }
            //        }
            //        break;

            //    case CommandType.Weather:
            //        if (!string.IsNullOrEmpty(result.TTSResponse))
            //        {
            //            string phrasePath = _cacher.GetPhrasePath(result.TTSResponse);

            //            if (!string.IsNullOrEmpty(phrasePath))
            //            {
            //                LogManager.Log($"Playing TTS audio for response: {result.TTSResponse}", LogLevel.Info);
            //                if (result.Data is WeatherPayload weatherPayload)
            //                {
            //                    _notificationManager.ShowWidget(weatherPayload);
            //                }
            //                else
            //                {
            //                    LogManager.Log("Expected WeatherPayload data for Weather command type, but received different type.", LogLevel.Warning);
            //                }

            //                _textToSpeech.PlayAudio(phrasePath);
            //            }
            //            else
            //            {
            //                LogManager.Log($"Failed to retrieve audio for TTS response: {result.TTSResponse}", LogLevel.Warning);
            //            }
            //        }
            //        break;

            //    case CommandType.Unknown:
            //        string unknownPhrasePath = _cacher.GetPhrasePath(result.TTSResponse);
            //        _textToSpeech?.PlayAudio(unknownPhrasePath);
            //        break;
            //}

            if (!string.IsNullOrEmpty(result.TTSResponse))
            {
                string phrasePath = _cacher.GetPhrasePath(result.TTSResponse);
                if (!string.IsNullOrEmpty(phrasePath))
                {
                    LogManager.Log($"Playing TTS audio for response: {result.TTSResponse}", LogLevel.Info);
                    _textToSpeech.PlayAudio(phrasePath);
                }
                else
                {
                    LogManager.Log($"Failed to retrieve audio for TTS response: {result.TTSResponse}", LogLevel.Warning);
                }

                _notificationManager.ShowWidget(result.NotificationPayload);
            }
        }
    }
}
