using System.Windows.Media;
using VAProject.Core.Interfaces;
using VAProject.Core.Models.Notifications;

namespace VAProject.Core.CommandsLogic
{
    public class CommandRouter
    {
        private readonly IEnumerable<IVoiceCommand> _commands;
        private readonly ResultHandler _resultHandler;

        public CommandRouter(INotificationService notificationService, IEnumerable<IVoiceCommand> voiceCommands)
        {
            _resultHandler = new ResultHandler(notificationService);

            _commands = voiceCommands;
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
                {
                    Success = false,
                    LogMessage = $"No command found for input: '{recognizedText}'",
                    TTSResponse = "Sorry, I didn't understand that command.",
                    NotificationPayload = new TextPayload
                    {
                        Text = $"No command found for input: '{recognizedText}'",
                        AccentColor = Colors.Red
                    }
                };

                _resultHandler.HandleCommandResult(result);
            }
        }
    }
}