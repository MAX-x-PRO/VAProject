using System.IO;
using VAProject.Core.Interfaces;
using VAProject.Core.Models.Notifications;
using VAProject.Core.Utils;

namespace VAProject.Core.CommandsLogic.Commands
{
    public class FindErrorsInLog : IVoiceCommand
    {
        public List<string> Triggers => new List<string> 
        { 
            "find errors" 
        };

        private readonly string _defaultLogDirPath;

        public FindErrorsInLog()
        {
            _defaultLogDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }

        public async Task<CommandResult> OnExecute(string cmdText)
        {
            int totalErrorCount = 0;

            foreach (string filePath in Directory.EnumerateFiles(_defaultLogDirPath, "*.txt"))
            {
                totalErrorCount += await CountErrorsInLogAsync(filePath);
            }

            return new CommandResult
            {
                Success = true,
                LogMessage = $"Total errors found in logs: {totalErrorCount}",
                TTSResponse = $"I found {totalErrorCount} errors in the logs.",
                NotificationPayload = new TextPayload
                {
                    Text = $"Total errors found in logs: {totalErrorCount}",
                    AccentColor = System.Windows.Media.Colors.Green
                }
            };
        }

        private async Task<int> CountErrorsInLogAsync(string filePath)
        {
            int errorCount = 0;

            LargeDataProcessor dataProcessor = new LargeDataProcessor() ;
            await foreach (var line in dataProcessor.ReadFileInChunksAsync(filePath))
            {
                if (line.Contains("[ERROR]", StringComparison.OrdinalIgnoreCase))
                {
                    errorCount++;
                }
            }

            return errorCount;
        }
    }
}
