using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using VAProject.Core.Interfaces;

namespace VAProject.Core.CommandsLogic.CommandDecorators
{
    public class JsonStatisticsTracker : IStatisticTracker
    {
        private readonly string _fileName = "command_stats.json";
        private readonly string _fileDir = "Analytics";
        private readonly string _filePath;
        private Dictionary<string, int> _stats;

        private readonly object _lock = new();

        public JsonStatisticsTracker()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileDir, _fileName);

            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _stats = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new();
            }
            else
            {
                _stats = new Dictionary<string, int>();
            }
        }

        public async Task RecordUsageAsync(string commandName)
        {
            lock (_lock)
            {
                if (_stats.ContainsKey(commandName))
                {
                    _stats[commandName]++;
                }
                else
                {
                    _stats[commandName] = 1;
                }

                string json = JsonSerializer.Serialize(_stats, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }

            await Task.CompletedTask;
        }
    }
}
