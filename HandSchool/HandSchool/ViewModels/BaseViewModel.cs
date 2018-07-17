using HandSchool.Internal;

namespace HandSchool.ViewModels
{
    public class BaseViewModel : NotifyPropertyChanged
    {
        bool isBusy = false;
        string title = string.Empty;
        public IViewResponse View { get; set; }
        
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
    }
}
