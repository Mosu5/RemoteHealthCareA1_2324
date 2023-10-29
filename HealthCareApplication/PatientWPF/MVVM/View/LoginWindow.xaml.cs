using PatientWPFApp.MVVM.ViewModel;
using PatientWPFApp.PatientLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PatientWPFApp.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            
            InitializeComponent();

            // Create a new MainWindowViewModel for DataContext. This can
            // only happen in the window which the UI starts up with. In
            // this case, it's the login window.
            Navigator.CurrentWindow = this;
        }
    }
}
