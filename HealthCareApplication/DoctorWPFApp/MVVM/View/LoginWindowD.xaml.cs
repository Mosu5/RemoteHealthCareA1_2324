using DoctorWPFApp.MVVM.ViewModel;
using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace DoctorWPFApp.MVVM.View
{
    /// <summary>
    /// Logic for the login window with username and password fields
    /// </summary>
    public partial class LoginWindowD : Window
    {
        

        public LoginWindowD()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LineSeries mySeries = new()
            {
                Values = new ChartValues<int> { 12, 23, 55, 1 }
            };

            LineChart.Series.Add(mySeries);

            // Create a new MainWindowViewModel for DataContext. This can
            // only happen in the window which the UI starts up with. In
            // this case, it's the login window.
            Navigator.CurrentWindow = this;
        }

        private void ChartButton_Click(object sender, RoutedEventArgs e)
        {
            LineSeries mySeries = LineChart.Series[0] as LineSeries;
            mySeries.Values.Add(12);
        }
    }
}
