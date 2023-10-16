using System.Windows;

namespace DoctorWPFApp.MVVM.View
{
    public partial class ChatWindowD : Window
    {
        public ChatWindowD()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.navToSessionWindow(this);
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
