using DoctorWPFApp.MVVM.ViewModel;
using System.Windows;

namespace DoctorWPFApp.MVVM.View
{

    public partial class MainWindowD : Window
    {
        public MainWindowD()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (gbBox.Text.ToString() == "super" && wwBox.Password.ToString() == "sexy")
            {
                SessionWindowD sessionWindow = new SessionWindowD();
                Hide();
                sessionWindow.Show();
            }
            else
            {
                MessageBox.Show("Wrong username or password.", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        
    }
}
