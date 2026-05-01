using VAProject.Logger;
using System.Reflection;

// TODO
// Implement a more advanced command matching algorithm (using NLP techniques)
// Implemend notification system for command results and show them in the UI
// Add support for command aliases
// Add more commands and make them more robust (add parameters parsing, support for multiple triggers, etc.)

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

            Assembly program = Assembly.GetExecutingAssembly();
            var commandTypes = program.GetTypes().Where(t => typeof(IVoiceCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var commandType in commandTypes)
            {
                IVoiceCommand commandInstance = (IVoiceCommand)Activator.CreateInstance(commandType);
                _commands.Add(commandInstance);
            }
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
                CommandResult result = new CommandResult
                (
                    success: false,
                    logMessage: $"No command found for input: '{recognizedText}'",
                    ttsResponse: "Sorry, I didn't understand that command.",
                    commandType: CommandType.Unknown
                );

                _resultHandler.HandleCommandResult(result);
            }
        }
    }
}