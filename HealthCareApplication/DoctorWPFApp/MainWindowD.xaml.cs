﻿using System.Windows;
using DoctorWPFApp.View;

namespace DoctorWPFApp
{
   
    public partial class MainWindowD : Window
    {
        public MainWindowD()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (gbBox.Text.ToString() == "super" && wwBox.Password.ToString() == "sexy")
            {
                SessionSettingD sessionSetting = new SessionSettingD();
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