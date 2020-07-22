using PaulZero.RoutineEnforcer.Views.Models;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PaulZero.RoutineEnforcer.Models.View.Window
{
    public class MainWindowViewModel : AbstractViewModel
    {
        public bool HasLoadingError
        {
            get => _hasLoadingError;
            set
            {
                if (_hasLoadingError == value)
                    return;

                _hasLoadingError = value;

                NotifyPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage == value)
                    return;

                _statusMessage = value;

                NotifyPropertyChanged();
            }
        }

        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                if (_visibility == value)
                    return;

                _visibility = value;

                NotifyPropertyChanged();
            }
        }

        public ImageSource ApplicationIcon
        {
            get => _applicationIcon;
            set
            {
                if (_applicationIcon == value)
                    return;

                _applicationIcon = value;

                NotifyPropertyChanged();
            }
        }

        private ImageSource _applicationIcon;
        private bool _hasLoadingError;
        private string _statusMessage = "Loading...";
        private Visibility _visibility;

        public void Load()
        {
            using var imageStream = new MemoryStream(Resource.clock_icon);

            var wpfIcon = new BitmapImage();
            wpfIcon.BeginInit();
            wpfIcon.StreamSource = imageStream;
            wpfIcon.EndInit();

            ApplicationIcon = wpfIcon;

            Visibility = Visibility.Collapsed;
        }
    }
}
