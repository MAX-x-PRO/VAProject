namespace VAProject.Core.Logger
{
    internal static class LogManager
    {
        private static Logger? _logger;
        
        public static void Initialize(LogLevel logLevel = 0)
        {
            if (_logger != null)
            {
                throw new InvalidOperationException("LogManager is already initialized");
            }
            _logger = new Logger(logLevel);
        }

        public static void Log(string message, LogLevel logLevel = 0)
        {
            if (_logger == null)
            {
                throw new InvalidOperationException("LogManager is not initialized");
            }
            _logger.Log(message, logLevel);
        }
    }
}
