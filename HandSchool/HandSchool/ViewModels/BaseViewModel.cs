using HandSchool.Internal;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 提供了 ViewModel 的基类。
    /// </summary>
    public class BaseViewModel : NotifyPropertyChanged
    {
        bool isBusy = false;
        string title = string.Empty;

        /// <summary>
        /// 程序绑定的页面
        /// </summary>
        public IViewResponse View { get; set; }
        
        /// <summary>
        /// ViewModel是否正忙
        /// </summary>
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        /// <summary>
        /// View的窗口标题
        /// </summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
    }
}
