using System.Windows;

namespace DoctorWPFApp.MVVM.View
{
    public partial class ChatWindowD : Window
    {
        public ChatWindowD()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            SessionWindowD sessionWindow = new SessionWindowD();
            Close();
            sessionWindow.Show();
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
