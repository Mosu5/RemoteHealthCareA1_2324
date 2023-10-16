using System.Windows;

namespace DoctorWPFApp.MVVM.View
{
    public static class Navigator
    {
   
        public static void navToSessionWindow(Window view)
        {
            SessionWindowD sessionWindow = new SessionWindowD();
            sessionWindow.DataContext = view.DataContext;
            view.Close();
            sessionWindow.Show();
        }

        public static void navToStatWindow(Window view)
        {
            StatsWindowD statsWindow = new StatsWindowD();
            statsWindow.DataContext = view.DataContext;
            view.Close();
            statsWindow.Show();
        }

        public static void navToChatWindow(Window view)
        {
            ChatWindowD chatWindow = new ChatWindowD();
            chatWindow.DataContext = view.DataContext;
            view.Close();
            chatWindow.Show();
        }
    }
}
