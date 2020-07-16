using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PaulZero.WindowsRoutine.Wpf.Models.View
{
    public class AbstractViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
