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

        public SessionWindowD()
        {
            InitializeComponent();
        }

        private async void StopExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Small delay to wait for DoctorViewModel to send stop message, etc.
            await Task.Delay(500);
            Close();
        }
    }
}
