using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DoctorWPFApp.MVVM.ViewModel;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Method used to simplify calling PropertyChanged.
    /// </summary>
    public void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}