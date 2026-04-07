namespace VAProject.Logger
{
    internal interface ILogger
    {
        public sbyte LogLevel { get; set; }
        public void Log(string message, sbyte logLevel);
    }
}
