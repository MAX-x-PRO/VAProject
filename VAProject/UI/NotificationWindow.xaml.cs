using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace VAProject.UI
{
    public partial class NotificationWindow : Window
    {
        private DispatcherTimer _autoHideTimer;

        public NotificationWindow()
        {
            InitializeComponent();

            _autoHideTimer = new DispatcherTimer();
            _autoHideTimer.Tick += (s, e) => HideNotification();
            
            this.Loaded += NotificationWindow_Loaded;
        }

        private void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;

            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Top + 10;
        }

        public void ShowNotification(string message, System.Drawing.Color indicatorColor, int autoHideMilliseconds = 3000)
        {
            Dispatcher.Invoke(() =>
            {
                MessageText.Text = message;
                StatusIndicator.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(indicatorColor.A, indicatorColor.R, indicatorColor.G, indicatorColor.B));

                SettingsPanel.Visibility = Visibility.Collapsed;

                var fadeInAnimation = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                this.BeginAnimation(Window.OpacityProperty, fadeInAnimation);

                this.Show();

                _autoHideTimer.Stop();
                if (autoHideMilliseconds > 0) 
                {
                    _autoHideTimer.Interval = TimeSpan.FromMilliseconds(autoHideMilliseconds);
                    _autoHideTimer.Start();
                }
            });
        }

        private void HideNotification()
        {
            _autoHideTimer.Stop();
            this.Hide();
        }
    }
}
