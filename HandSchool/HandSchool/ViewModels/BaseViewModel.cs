using HandSchool.Internal;

namespace HandSchool.ViewModels
{
    public class BaseViewModel : NotifyPropertyChanged
    {
        bool isBusy = false;
        string title = string.Empty;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
    }
}
