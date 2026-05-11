using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using VAProject.Logger;

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

        public void ShowNotification(string message, System.Windows.Media.Color indicatorColor, int autoHideMilliseconds = 3000)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _autoHideTimer.Stop();

                this.BeginAnimation(Window.OpacityProperty, null);

                MessageText.Text = message;
                StatusIndicator.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(indicatorColor.A, indicatorColor.R, indicatorColor.G, indicatorColor.B));
                SettingsPanel.Visibility = Visibility.Collapsed;

                this.Opacity = 0;
                this.Visibility = Visibility.Visible;
                this.Show();

                this.Topmost = true;

                var fadeInAnimation = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                this.BeginAnimation(Window.OpacityProperty, fadeInAnimation);

                if (autoHideMilliseconds > 0)
                {
                    _autoHideTimer.Interval = TimeSpan.FromMilliseconds(autoHideMilliseconds);
                    _autoHideTimer.Start();
                }
            }));
        }

        private void HideNotification()
        {
            _autoHideTimer.Stop();

            this.Show();
            this.Opacity = 1;
            this.Visibility = Visibility.Visible;
            this.Topmost = true;

            var fadeInAnimation = new System.Windows.Media.Animation.DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeInAnimation.Completed += (s, e) => this.Hide();

            this.BeginAnimation(Window.OpacityProperty, fadeInAnimation);
        }
    }
}
