using Media = System.Windows.Media;
using System.Text.RegularExpressions;
using VAProject.Core.Logger;
using VAProject.Core.Interfaces;
using System.Printing;
using VAProject.Core.Models.Notifications;

namespace VAProject.Core.CommandsLogic.Commands
{
    public class AlarmMode : IVoiceCommand
    {
        private readonly INotificationService _notificationService;

        public List<string> Triggers => new List<string>
        {
            "enable alarm",
            "alarm mode"
        };

        public AlarmMode(INotificationService notificationService) 
        { 
            _notificationService = notificationService;
        }

        public Task<CommandResult> OnExecute(string cmdText)
        {
            int durationSeconds = GetDurationSecondsFromCommand(cmdText);

            _ = RunAlarmVisualsAsync(durationSeconds);

            CommandResult result = new CommandResult
            {
                Success = true,
                LogMessage = $"Alarm mode enabled for {durationSeconds} seconds",
                TTSResponse = $"Alarm mode enabled for {durationSeconds} seconds",
                NotificationPayload = new TextPayload
                {
                    Text = $"Alarm mode enabled for {durationSeconds} seconds",
                    AccentColor = Media.Colors.Red
                }
            };

            return Task.FromResult(result);
        }

        private async Task RunAlarmVisualsAsync(int durationSeconds)
        {
            var colorGenerator = GetColorCycleGenerator();

            await ProcessColorsWithTimeoutAsync(colorGenerator, durationSeconds, (color) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _notificationService.ShowWidget(new TextPayload
                    {
                        Text = "Alarm mode active!",
                        AccentColor = color
                    });
                });
            });

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _notificationService.ShowWidget(new TextPayload
                {
                    Text = "Alarm mode ended.",
                    AccentColor = Media.Colors.Green
                });
            });
        }

        private int GetDurationSecondsFromCommand(string cmdText)
        {
            Regex triggerRegex = new Regex(@"^(Alarm mode|Enable alarm) (for ((?<durationSeconds>\d+) seconds|(?<durationMinutes>\d+) minutes))?$", RegexOptions.IgnoreCase);
            Match match = triggerRegex.Match(cmdText);

            int deffaultDurationSeconds = 10;
            int durationSeconds = match.Groups["durationSeconds"].Success ? int.Parse(match.Groups["durationSeconds"].Value) : deffaultDurationSeconds;

            if (match.Groups["durationMinutes"].Success)
                durationSeconds = int.Parse(match.Groups["durationMinutes"].Value) * 60;

            return durationSeconds;
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
