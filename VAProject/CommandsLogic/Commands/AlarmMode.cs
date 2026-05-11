using Media = System.Windows.Media;
using VAProject.UI;
using VAProject.Logger;
using System.Text.RegularExpressions;

namespace VAProject.CommandsLogic.Commands
{
    internal class AlarmMode : IVoiceCommand
    {
        public List<string> Triggers => new List<string>
        {
            "enable alarm",
            "alarm mode"
        };

        public string TTSResponse => "Alarm mode";

        public CommandResult OnExecute(string cmdText)
        {
            Regex triggerRegex = new Regex(@"^(Alarm mode|Enable alarm) (for (?<durationSeconds>\d+) seconds)?$", RegexOptions.IgnoreCase);
            Match match = triggerRegex.Match(cmdText);

            int deffaultDurationSeconds = 10;
            int durationSeconds = match.Groups["durationSeconds"].Success ? int.Parse(match.Groups["durationSeconds"].Value) : deffaultDurationSeconds;

            Task.Run(async () =>
            {
                var colorGenerator = GetColorCycleGenerator();

                await ProcessColorsWithTimeoutAsync(colorGenerator, durationSeconds, (color) =>
                {
                    NotificationManager.ShowNotification("Alarm mode!", color, 0);
                });

                NotificationManager.ShowNotification("Alarm mode disabled", Media.Colors.Green, 3000);
            });

            return new CommandResult
            (
                true,
                $"Alarm mode enabled for {durationSeconds} seconds",
                TTSResponse + $" enabled for {durationSeconds} seconds",
                CommandType.General
            );
        }

        private IEnumerable<Media.Color> GetColorCycleGenerator()
        {
            Media.Color[] colors = { Media.Colors.Red, Media.Colors.Blue };
            int index = 0;

            while (true)
            {
                yield return colors[index];
                index = (index + 1) % colors.Length;
            }
        }

        private async Task ProcessColorsWithTimeoutAsync(IEnumerable<Media.Color> iterator, int timeoutSeconds, Action<Media.Color> actionProcess)
        {
            using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                foreach (var color in iterator)
                {
                    cts.Token.ThrowIfCancellationRequested();

                    actionProcess(color);

                    await Task.Delay(300, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                LogManager.Log("Timeout elapsed. Stopping iterator.");
            }
        }
    }
}
