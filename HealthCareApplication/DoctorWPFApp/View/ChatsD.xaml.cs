using System.Windows;

namespace DoctorWPFApp.View
{
    public partial class ChatsD : Window
    {
        public ChatsD()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            SessionSettingD sessionSetting = new SessionSettingD();
            Close();
            sessionSetting.Show();
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
