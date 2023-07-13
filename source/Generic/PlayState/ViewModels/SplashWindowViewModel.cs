using System.Collections.Generic;

namespace PlayState.ViewModels
{
    class SplashWindowViewModel : ObservableObject
    {

        public string gameName { get; set; }
        public string GameName
        {
            get => gameName;
            set
            {
                gameName = value;
                OnPropertyChanged();
            }
        }

        public string notificationMessage { get; set; }
        public string NotificationMessage
        {
            get => notificationMessage;
            set
            {
                notificationMessage = value;
                OnPropertyChanged();
            }
        }
    }
}
