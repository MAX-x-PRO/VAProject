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
        private VACore _VACore;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _VACore = new VACore();
            _VACore.Start(); 
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _VACore.Stop();

            base.OnExit(e);
        }
    }
}
