using System.Text.RegularExpressions;
using System.Diagnostics;
using VAProject.Core.Interfaces;
using VAProject.Core.Models.Notifications;

namespace VAProject.Core.CommandsLogic.Commands
{
    internal class OpenAppCommand: IVoiceCommand
    {
        public List<string> Triggers => new List<string>() 
        { 
            "open", 
            "launch", 
            "start",
        };

        private Dictionary<string, string> _targets = new Dictionary<string, string>()
        {
            { "youtube" , "https://www.youtube.com" },
            { "google" , "https://www.google.com" },
            { "github" , "https://www.github.com" },
            { "youtube music" , "https://music.youtube.com" }
        };

        private Dictionary<string, string> _apps = new Dictionary<string, string>()
        {
            { "chrome", @"C:\Program Files\Google\Chrome\Application\chrome.exe" },
            { "zen", @"C:\Program Files\Zen Browser\Application\zen.exe" },
        };

        public Task<CommandResult> OnExecute(string cmdText)
        {
            CommandResult result;

            Regex Match = new Regex(@"^(open|launch|start) (?<target>.+?)( with (?<app>.+))?$", RegexOptions.IgnoreCase);

            string targetMatch = Match.Match(cmdText).Groups["target"].Value;
            string appMatch = Match.Match(cmdText).Groups["app"].Value;

            string? target = _targets.GetValueOrDefault(targetMatch, null);
            string? app = _apps.GetValueOrDefault(appMatch, null);

            if (target == null)
            {
                return Task.FromResult(new CommandResult
                {
                    Success = false,
                    LogMessage = $"Open app command: failed - no target or app found for '{cmdText}' \n",
                    TTSResponse = "Sorry, I couldn't find the app or website you want to open.",
                    NotificationPayload = new TextPayload
                    {
                        Text = $"No target or app found for '{cmdText}'",
                        AccentColor = System.Windows.Media.Colors.Red
                    }
                });
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = app ?? target,
                    Arguments = app != null ? target : null,
                    UseShellExecute = true
                });

                result = new CommandResult
                {
                    Success = true,
                    LogMessage = $"Open app command: succeeded - opened '{target}' \n",
                    TTSResponse = $"Opening {targetMatch}",
                    NotificationPayload = new TextPayload
                    {
                        Text = $"Opened '{target}'",
                        AccentColor = System.Windows.Media.Colors.Green
                    }
                };
            }
            catch (Exception ex)
            {
                result = new CommandResult
                {
                    Success = false,
                    LogMessage = $"Open app command: failed - {ex.Message} \n",
                    TTSResponse = "Failed to open app",
                    NotificationPayload = new TextPayload
                    {
                        Text = $"Failed to open '{target}'",
                        AccentColor = System.Windows.Media.Colors.Red
                    }
                };
            }

            return Task.FromResult(result);
        }
    }
}
