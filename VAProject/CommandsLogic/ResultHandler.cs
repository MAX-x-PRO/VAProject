using VAProject.Audio;
using VAProject.Logger;

namespace VAProject.CommandsLogic
{
    internal class ResultHandler
    {
        private readonly TextToSpeech _textToSpeech;
        private readonly ILogger _logger;

        public ResultHandler(ILogger logger)
        {
            _logger = logger;
            _textToSpeech = new TextToSpeech(_logger);
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
                        _textToSpeech.Speak(result.TTSResponse);
                    }
                    break;
            }
        }
    }
}
