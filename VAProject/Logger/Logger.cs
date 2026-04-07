using System;
using System.IO;

namespace VAProject.Logger
{
    internal class Logger : ILogger
    {
        public LogLevel LogLevel { get; set; } = 0; 

        private string LogFilePath { get; set; }
        private string LogFolderPath { get; set; } = "Logs";  // CONFIGURABLE

        public Logger(LogLevel logLevel = 0)
        {
            LogLevel = logLevel;
        }

        public void Log(string message, LogLevel logLevel = 0)
        {
            if (logLevel != LogLevel) { return; }

            LogFilePath = LogToFile(message, LogFolderPath);
            LogToConsole(message);
        }

        private string LogToFile(string message, string folderPath)
        {
            File.AppendAllText(GetLogFilePath(), $"{message}{Environment.NewLine}");

            return Path.Combine(LogFolderPath, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
        }
        private void LogToConsole(string message)
        {
            Console.WriteLine(message);
        }

        private string GetLogFilePath()
        {
            if (!Directory.Exists(LogFolderPath))
            {
                Directory.CreateDirectory(LogFolderPath);
            }
            return LogFilePath ?? Path.Combine(LogFolderPath, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
        }
    }
}
