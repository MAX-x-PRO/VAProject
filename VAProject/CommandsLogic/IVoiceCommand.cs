namespace VAProject.CommandsLogic
{
    internal interface IVoiceCommand
    {
        public List<string> Triggers { get; }
        public string TTSResponse { get; }
        public CommandResult OnExecute(string cmdText);
    }
}
