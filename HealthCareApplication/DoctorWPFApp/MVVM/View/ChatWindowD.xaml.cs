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
            RequestHandler.ReceivedChat += OnChatReceived;
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

        private async void OnChatReceived(object? sender, string chatMessage)
        {
            // TODO fix server sending chat to doctor
            // TODO update observable list
            MessageBox.Show($"Chat received: {chatMessage}");
        }
    }
}
