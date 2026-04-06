using VAProject.Audio;
using VAProject.CommandsLogic.Commands;

namespace VAProject.CommandsLogic
{
    internal class CommandRouter
    {
        private readonly List<IVoiceCommand> _commands = new List<IVoiceCommand>();
        private readonly TextToSpeech _textToSpeech;

        public CommandRouter() 
        { 
            _commands.Add(new OpenBrowserCommand());

            _textToSpeech = new TextToSpeech();
        }

        public void RouteInput(string recognizedText)
        {
            string lowerText = recognizedText.ToLower().Trim();

            IVoiceCommand cmdToExecute = _commands.FirstOrDefault(cmd => cmd.Triggers.Any(trigger => lowerText.Contains(trigger)));

            if (cmdToExecute != null)
            {
                cmdToExecute.OnExecute(recognizedText);
                _textToSpeech.Speak(cmdToExecute.TTSResponse);
            }
            else
            {
                Console.WriteLine($"Command cant be executed: '{recognizedText}'");
            }
        }
    }
}