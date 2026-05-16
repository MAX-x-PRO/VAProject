using System.Windows;
using System.Windows.Threading;
using VAProject.Core.Models.Notifications;

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
            this.SizeChanged += OnWindowSizeChanged;
        }

        private void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;

            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Top + 10;
        }

        public void ShowWidget(NotificationPayload payload)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _autoHideTimer.Stop();

                WidgetContainer.Content = payload;

                this.BeginAnimation(Window.OpacityProperty, null);

                WidgetContainer.Content = payload;

                this.Opacity = 0;
                this.Visibility = Visibility.Visible;
                this.Show();

                this.Topmost = true;

                var fadeInAnimation = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                this.BeginAnimation(Window.OpacityProperty, fadeInAnimation);

                if (payload.DurationMs > 0)
                {
                    _autoHideTimer.Interval = TimeSpan.FromMilliseconds(payload.DurationMs);
                    _autoHideTimer.Start();
                }
            }));
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var workingArea = SystemParameters.WorkArea;

            this.Left = workingArea.Right - this.ActualWidth - 20;
            this.Top = workingArea.Bottom - this.ActualHeight - 20;
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
