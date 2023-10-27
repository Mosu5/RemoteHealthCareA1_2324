using DoctorWPFApp.Networking;
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

        private void ChatsBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.NavToChatWindow();
        }

        private void StatsBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.NavToStatWindow();
        }

        private void StopstartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning == true)
            {
                // Stop the session


                isRunning = false;
                stopstartBtn.Content = "Start";
                stopstartBtn.Background = System.Windows.Media.Brushes.LightGreen;
            }
            else
            {
                // Start a new session


                isRunning = true;
                stopstartBtn.Content = "Stop";
                stopstartBtn.Background = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
