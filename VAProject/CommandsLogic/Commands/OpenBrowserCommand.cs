namespace VAProject.CommandsLogic.Commands
{
    internal class OpenBrowserCommand: IVoiceCommand
    {
        public List<string> Triggers => new List<string>() 
        { 
            "open browser", 
            "launch browser", 
            "start browser",
            "browser"
        };

        public string TTSResponse => "Opening browser";

        public CommandResult OnExecute(string cmdText)
        {
            CommandResult result;

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://www.google.com",
                    UseShellExecute = true
                });

                result = new CommandResult(true, "Browser opened \n", TTSResponse);
            }
            catch (Exception ex)
            {
                result = new CommandResult(false, $"Open browser command: failed - {ex.Message} \n", "Failed to open browser");
            }

            return result;
        }
    }
}
