using VAProject.CommandsLogic.Commands;

namespace VAProject.CommandsLogic
{
    internal class CommandRouter
    {
        private readonly List<IVoiceCommand> _commands = new List<IVoiceCommand>();

        public CommandRouter() 
        { 
            _commands.Add(new OpenBrowserCommand());
        }

        public void RouteInput(string recognizedText)
        {
            string lowerText = recognizedText.ToLower().Trim();

            IVoiceCommand cmdToExecute = _commands.FirstOrDefault(cmd => cmd.Triggers.Any(trigger => lowerText.Contains(trigger)));

            if (cmdToExecute != null)
            {
                cmdToExecute.OnExecute(recognizedText);
            }
            else
            {
                Console.WriteLine($"Command cant be executed: '{recognizedText}'");
            }
        }
    }
}