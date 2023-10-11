using System.Windows;

namespace DoctorWPFApp.View
{
    public partial class ChatsD : Window
    {
        public ChatsD()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SessionSetting sessionSetting = new SessionSetting();
            Close();
            sessionSetting.Show();
        }
    }
}
