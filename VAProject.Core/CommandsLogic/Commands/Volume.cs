using AudioSwitcher.AudioApi.CoreAudio;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Number;
using VAProject.Core.CommandsLogic;
using VAProject.Core.Interfaces;
using VAProject.Core.Models.Notifications;

namespace VAProject.CommandsLogic.Commands
{
    internal class Volume : IVoiceCommand
    {
        public List<string> Triggers => new List<string>()
        {
            "volume"
        };

        public Task<CommandResult> OnExecute(string cmdText)
        {
            CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
            string[] parsed = cmdText.Split(' ');

            ModelResult firstMatch = NumberRecognizer.RecognizeNumber(cmdText, Culture.English).FirstOrDefault();

            if(int.TryParse(firstMatch?.Resolution["value"]?.ToString(), out int value) == false)
            {
                return Task.FromResult(new CommandResult
                {
                    Success = true,
                    LogMessage = $"Current volume is {defaultPlaybackDevice.Volume}%",
                    TTSResponse = $"Current volume is {defaultPlaybackDevice.Volume} percent.",
                    NotificationPayload = new TextPayload
                    {
                        Text = $"Current volume is {defaultPlaybackDevice.Volume}%",
                        AccentColor = System.Windows.Media.Colors.Blue
                    }
                });
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
            return Task.FromResult(new CommandResult
            {
                Success = true,
                LogMessage = logMessage,
                TTSResponse = $"Volume set to {defaultPlaybackDevice.Volume} percent.",
                NotificationPayload = new TextPayload
                {
                    Text = logMessage,
                    AccentColor = System.Windows.Media.Colors.Green
                }
            });
        }
    }
}
