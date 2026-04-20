using VAProject.Audio;
using VAProject.Logger;

namespace VAProject.CommandsLogic
{
    internal class ResultHandler
    {
        private readonly Cacher _cacher;
        private readonly TextToSpeech _textToSpeech;
        private readonly ILogger _logger;

        public ResultHandler(ILogger logger)
        {
            _logger = logger;
            _textToSpeech = new TextToSpeech(_logger);
            _cacher = new Cacher(_textToSpeech);
        }

        public void HandleCommandResult(CommandResult result)
        {
            _logger.Log($"Command execution result: {(result.Success ? "Success" : "Failure")}", LogLevel.Debug);
            _logger.Log(result.LogMessage, LogLevel.Debug);

            switch (result.CommandType)
            {
                case CommandType.General:
                    if (!string.IsNullOrEmpty(result.TTSResponse))
                    {
                        string phrasePath = _cacher.GetPhrasePath(result.TTSResponse);

                        if (!string.IsNullOrEmpty(phrasePath))
                        {
                            _logger.Log($"Playing TTS audio for response: {result.TTSResponse}", LogLevel.Info);
                            _textToSpeech.PlayAudio(phrasePath);
                        }
                        else
                            _logger.Log($"Failed to retrieve audio for TTS response: {result.TTSResponse}", LogLevel.Warning);
                    }
                    break;
            }
        }
    }
}
