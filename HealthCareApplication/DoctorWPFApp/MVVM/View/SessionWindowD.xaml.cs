using DoctorWPFApp.Networking;
using System.ComponentModel;
using System.Windows.Media;
using System.Text.Json.Nodes;
using System.Windows;
using System.Threading.Tasks;

namespace DoctorWPFApp.MVVM.View
{
    /// <summary>
    /// Main window of the application 
    /// After connecting to server, user can select patients and go to the respective views to see their data during the session
    /// </summary>
    public partial class SessionWindowD : Window
    {
        private bool _sessionActive = false;

        public SessionWindowD()
        {
            InitializeComponent();
        }

        private void ChatsBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.NavToChatWindow();
        }

        private void StatsBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.NavToStatWindow();
        }

        private async void StopExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Small delay to wait for MainWindowViewModel to send stop message, etc.
            await Task.Delay(500);
            Close();
        }
    }
}
