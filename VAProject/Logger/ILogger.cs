namespace VAProject.Logger
{
    internal interface ILogger
    {
        public LogLevel LogLevel { get; set; }
        public void Log(string message, LogLevel logLevel = 0);
    }
}
