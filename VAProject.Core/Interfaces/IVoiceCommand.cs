using VAProject.Core.CommandsLogic;

namespace VAProject.Core.Interfaces
{
    public interface IVoiceCommand
    {
        public List<string> Triggers { get; }
        public CommandResult OnExecute(string cmdText);
    }
}
