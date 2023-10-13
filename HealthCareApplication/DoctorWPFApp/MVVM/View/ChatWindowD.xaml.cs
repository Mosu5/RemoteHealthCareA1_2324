using DoctorWPFApp.MVVM.ViewModel;
using System.Windows;

namespace DoctorWPFApp.MVVM.View
{
    public partial class ChatWindowD : Window
    {
        public ChatWindowD()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            SessionWindowD sessionWindow = new SessionWindowD();
            Hide();
            sessionWindow.Show();
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
