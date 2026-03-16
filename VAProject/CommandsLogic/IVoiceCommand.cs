namespace VAProject.CommandsLogic
{
    internal interface IVoiceCommand
    {
        public List<string> Triggers { get; }
        public void OnExecute(string cmdText);
    }
}
