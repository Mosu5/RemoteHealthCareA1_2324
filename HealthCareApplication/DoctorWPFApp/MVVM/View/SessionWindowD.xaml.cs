using System.ComponentModel;
using System.Windows;

namespace DoctorWPFApp.MVVM.View
{
    /// <summary>
    /// Main window of the application 
    /// After connecting to server, user can select patients and go to the respective views to see their data during the session
    /// </summary>
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
            Navigator.NavToChatWindow();
        }

        private void statsBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.NavToStatWindow();
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
