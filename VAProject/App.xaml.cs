using System.Configuration;
using System.Data;
using System.Windows;
using VAProj.Audio;

namespace VAProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private AudioCapturer audioCapturer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            audioCapturer = new AudioCapturer();
            audioCapturer?.StartListening();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            audioCapturer?.StopListening();
            base.OnExit(e);
        }
    }
}
