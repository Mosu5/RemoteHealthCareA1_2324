using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using DoctorWPFApp.Networking;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

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

        private void StopExitButton_Click(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(1000);
            Close();
        }
    }
}
