using PatientWPFApp.View;
using System.Windows;

namespace PatientWPFApp
{

    public partial class MainWindowP : Window
    {
        public MainWindowP()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (gbBox.Text.ToString() == "super" && wwBox.Password.ToString() == "sexy")
            {
                SessionSettingP sessionSetting = new SessionSettingP();
                Close();
                sessionSetting.Show();
            }
            else
            {
                MessageBox.Show("Wrong username or password.", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
