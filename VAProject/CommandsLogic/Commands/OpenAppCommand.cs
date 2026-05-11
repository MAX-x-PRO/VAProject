using System.Text.RegularExpressions;
using System.Diagnostics;

namespace VAProject.CommandsLogic.Commands
{
    internal class OpenAppCommand: IVoiceCommand
    {
        public List<string> Triggers => new List<string>() 
        { 
            "open", 
            "launch", 
            "start",
        };

        public string TTSResponse => "Opening";

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

        public CommandResult OnExecute(string cmdText)
        {
            CommandResult result;

            Regex Match = new Regex(@"^(open|launch|start) (?<target>.+?)( with (?<app>.+))?$", RegexOptions.IgnoreCase);

            string targetMatch = Match.Match(cmdText).Groups["target"].Value;
            string appMatch = Match.Match(cmdText).Groups["app"].Value;

            string? target = _targets.GetValueOrDefault(targetMatch, null);
            string? app = _apps.GetValueOrDefault(appMatch, null);
            
            if (target == null)
            {
                return new CommandResult(false, $"Open app command: failed - no target or app found for '{cmdText}' \n", "Sorry, I couldn't find the app or website you want to open.");
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = app ?? target,
                    Arguments = app != null ? target : null,
                    UseShellExecute = true
                });

                result = new CommandResult(true, $"Open app command: succeeded - opened '{target}' \n", $"{TTSResponse} {targetMatch}");
            }
            catch (Exception ex)
            {
                result = new CommandResult(false, $"Open app command: failed - {ex.Message} \n", "Failed to open app");
            }

            return result;
        }
    }
}
