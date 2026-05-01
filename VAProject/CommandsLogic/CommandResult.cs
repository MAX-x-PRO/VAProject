namespace VAProject.CommandsLogic
{
    internal class CommandResult
    {
        public bool Success { get; set; }
        public string LogMessage { get; set; }
        public string TTSResponse { get; set; }
        public CommandType CommandType { get; set; }
        public object Data { get; set; }

        public CommandResult(bool success, string logMessage, string ttsResponse, CommandType commandType = CommandType.General, object? data = null) 
        {
            CommandType = commandType;
            Success = success;
            LogMessage = logMessage;
            TTSResponse = ttsResponse;
            Data = data;
        }  
    }
}
