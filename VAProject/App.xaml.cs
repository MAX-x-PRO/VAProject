using System.Configuration;
using System.Data;
using System.Windows;
using VAProject.Audio;

namespace VAProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private VACore VACore;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            VACore = new VACore();
            VACore.Start(); 
        }

        protected override void OnExit(ExitEventArgs e)
        {
            VACore.Stop();

            base.OnExit(e);
        }
    }
}
