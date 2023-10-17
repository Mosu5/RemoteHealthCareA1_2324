using DoctorWPFApp.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DoctorWPFApp.MVVM.View
{
    /// <summary>
    /// Create a 
    /// </summary>
    public partial class LoginWindowD : Window
    {
        public LoginWindowD()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            DataContext = new MainWindowViewModel();
            Navigator.CurrentWindow = this;
        }

      

        // TODO move login logic to viewmodel while making use of navigator
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (gbBox.Text.ToString() == "super" && passwordBox.Password.ToString() == "sexy")
            {
                Navigator.NavToSessionWindow();
            }
            else
            {
                MessageBox.Show("Wrong username or password.", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


    }
}
