using DoctorWPFApp.Networking;
using System.ComponentModel;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;

namespace DoctorWPFApp.MVVM.View
{
    /// <summary>
    /// Window containing messages between patient and doctor
    /// </summary>
    public partial class ChatWindowD : Window
    {
        public ChatWindowD()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
           
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.NavToSessionWindow();
        }

        // Todo maybe change return type to task?
        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            JsonObject chatObject = DoctorFormat.ChatsSendMessage(sendBox.Text);
            await ClientConn.SendJson(chatObject);
        }

       
    }
}
