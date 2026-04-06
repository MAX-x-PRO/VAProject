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

        public void OnExecute(string cmdText)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://www.google.com",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open browser: {ex.Message}");
            }
        }
    }
}
