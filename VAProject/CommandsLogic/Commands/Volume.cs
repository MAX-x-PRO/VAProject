using AudioSwitcher.AudioApi.CoreAudio;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Number;

namespace VAProject.CommandsLogic.Commands
{
    internal class Volume : IVoiceCommand
    {
        public List<string> Triggers => new List<string>()
        {
            "volume"
        };

        public string TTSResponse { get; } = "Volume has set to ";

        public CommandResult OnExecute(string cmdText)
        {
            CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

            string[] parsed = cmdText.Split(' ');
            ModelResult firstMatch = NumberRecognizer.RecognizeNumber(cmdText, Culture.English).FirstOrDefault();
            if(int.TryParse(firstMatch?.Resolution["value"]?.ToString(), out int value) == false)
            {
                return new CommandResult
                (
                    true,
                    $"Current volume is {defaultPlaybackDevice.Volume}%",
                    $"Current volume is {defaultPlaybackDevice.Volume} percent."
                );
            }

            if (parsed.Contains("up") || parsed.Contains("increase") || parsed.Contains("higher"))
            {
                defaultPlaybackDevice.Volume += value;
            }
            else if (parsed.Contains("down") || parsed.Contains("decrease") || parsed.Contains("lower"))
            {
                defaultPlaybackDevice.Volume -= value;
            }
            else if (parsed.Contains("set") || parsed.Contains("to"))
            {
                defaultPlaybackDevice.Volume = value;
            }

            string  logMessage = $"Volume set to {defaultPlaybackDevice.Volume}%";
            return new CommandResult
            (
                true,
                logMessage,
                TTSResponse + defaultPlaybackDevice.Volume + " percent."
            );
        }
    }
}
