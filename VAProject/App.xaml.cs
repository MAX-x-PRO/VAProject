using System.Configuration;
using System.Data;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using VAProject.UI;

namespace VAProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private VACore _VACore;

        private NotifyIcon? _trayIcon;
        private MainWindow _settingsWindow;
        private NotificationWindow _notificationWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeTrayIcon();

            _notificationWindow = new NotificationWindow();

            _VACore = new VACore();
            _VACore.AudioCapturer.OnMicStateChanged += HandleMicStateChange;
            _VACore.Start(); 
        }

        private void InitializeTrayIcon()
        {
            Icon icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI\\Icons\\icon.ico"));

            _trayIcon = new NotifyIcon
            {
                Icon = icon,
                Visible = true,
                Text = "Voice Assistant"
            };

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            
            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += ShowSettingsWindow;

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += ExitApp;

            contextMenu.Items.Add(settingsItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenuStrip = contextMenu;

            _trayIcon.DoubleClick += ShowSettingsWindow;
        }

        private void ShowSettingsWindow(object sender, EventArgs e)
        {
            if (_settingsWindow == null || !_settingsWindow.IsLoaded)
            {
                _settingsWindow = new MainWindow();
            }

            _settingsWindow.Show();
            _settingsWindow.Activate();
        }

        private void ExitApp(object sender, EventArgs e)
        {
            if(_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }

            _VACore.Stop();


            Environment.Exit(0);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _VACore.Stop();

            base.OnExit(e);
        }

        private void HandleMicStateChange(MicStates micState)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_settingsWindow != null && _settingsWindow.IsLoaded)
                {
                    _settingsWindow?.SetMicrophoneStatus(micState);
                }
            });
        }
    }
}
