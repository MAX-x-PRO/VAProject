using VAProject.Audio;
using VAProject.CommandsLogic.Commands;
using VAProject.Logger;

namespace VAProject.CommandsLogic
{
    internal class CommandRouter
    {
        private readonly List<IVoiceCommand> _commands = new List<IVoiceCommand>();
        private readonly ILogger _logger;
        private readonly ResultHandler _resultHandler;

        public CommandRouter(ILogger logger)
        {
            _logger = logger;
            _resultHandler = new ResultHandler(_logger); 

            _commands.Add(new OpenBrowserCommand());
        }

        public void RouteInput(string recognizedText)
        {
            string lowerText = recognizedText.ToLower().Trim();

            IVoiceCommand cmdToExecute = _commands.FirstOrDefault(cmd => cmd.Triggers.Any(trigger => lowerText.Contains(trigger)));

            if (cmdToExecute != null)
            {
                CommandResult result = cmdToExecute.OnExecute(recognizedText);
                _resultHandler.HandleCommandResult(result);
            }
            else
            {
                _logger.Log($"Command cant be executed: '{recognizedText}'", LogLevel.Debug);
            }
        }
    }
}