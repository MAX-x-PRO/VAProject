using System;
using System.Collections.Generic;
using System.Text;
using VAProject.Core.Interfaces;

namespace VAProject.Core.CommandsLogic.CommandDecorators
{
    public class AnalyticsDecorator : IVoiceCommand
    {
        private readonly IVoiceCommand _innerCommand;
        private readonly IStatisticTracker _tracker;
        private readonly string _commandName;

        public List<string> Triggers => _innerCommand.Triggers;

        public AnalyticsDecorator(IVoiceCommand innerCommand, IStatisticTracker tracker)
        {
            _innerCommand = innerCommand;
            _tracker = tracker;

            _commandName = innerCommand.GetType().Name;
        }

        public async Task<CommandResult> OnExecute(string commandText)
        {
            _tracker.RecordUsageAsync(_commandName);

            return await _innerCommand.OnExecute(commandText);
        }
    }
}
