using DoctorWPFApp.MVVM.ViewModel;
using System.Windows;

namespace DoctorWPFApp.MVVM.View
{
    public partial class SessionWindowD : Window
    {
        public bool isRunning = false;
        public SessionWindowD()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void chatsBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.navToChatWindow(this);
        }

        private void statsBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.navToStatWindow(this);
        }

        private void stopstartBtn_Click(object sender, RoutedEventArgs e)
        {

            if (isRunning == true)
            {
                isRunning = false;
                stopstartBtn.Content = "Start";
                stopstartBtn.Background = System.Windows.Media.Brushes.LightGreen;
            }
            else
            {
                isRunning = true;
                stopstartBtn.Content = "Stop";
                stopstartBtn.Background = System.Windows.Media.Brushes.Red;
            }

        }
    }
}
