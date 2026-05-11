using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using VAProject.UI;

namespace VAProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                double  clickY = e.GetPosition(this).Y;

                if (clickY <= 40)
                {
                    this.DragMove();
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void SetMicrophoneStatus(MicStates micState)
        {
            if (micState == MicStates.Active)
            {
                MicIndicator.Fill = new SolidColorBrush(Colors.LightGreen);
                StatusText.Text = "Listening command...";
            }
            else
            {
                MicIndicator.Fill = new SolidColorBrush(Colors.Gray);
                StatusText.Text = "Waiting for wakeword...";
            }
        }
    }
}