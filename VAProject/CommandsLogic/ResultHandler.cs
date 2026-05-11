using VAProject.Audio;
using VAProject.Logger;
using VAProject.UI;
using System.Windows.Media;

namespace VAProject.CommandsLogic
{
    internal class ResultHandler
    {
        private readonly Cacher _cacher;
        private readonly TextToSpeech _textToSpeech;

        public ResultHandler()
        {
            _textToSpeech = new TextToSpeech();
            _cacher = new Cacher(_textToSpeech);
        }

        public void HandleCommandResult(CommandResult result)
        {
            LogManager.Log($"Command execution result: {(result.Success ? "Success" : "Failure")}", LogLevel.Debug);
            LogManager.Log(result.LogMessage, LogLevel.Debug);

            switch (result.CommandType)
            {
                case CommandType.General:
                    if (!string.IsNullOrEmpty(result.TTSResponse))
                    {
                        string phrasePath = _cacher.GetPhrasePath(result.TTSResponse);

                        if (!string.IsNullOrEmpty(phrasePath))
                        {
                            LogManager.Log($"Playing TTS audio for response: {result.TTSResponse}", LogLevel.Info);

                            NotificationManager.ShowNotification(result.TTSResponse, Colors.Green, 3000);

                            _textToSpeech.PlayAudio(phrasePath);
                        }
                        else
                        {
                            LogManager.Log($"Failed to retrieve audio for TTS response: {result.TTSResponse}", LogLevel.Warning);
                        }
                    }
                    break;

                case CommandType.Unknown:
                    string unknownPhrasePath = _cacher.GetPhrasePath(result.TTSResponse);
                    _textToSpeech?.PlayAudio(unknownPhrasePath);
                    break;
            }
        }
    }
}
