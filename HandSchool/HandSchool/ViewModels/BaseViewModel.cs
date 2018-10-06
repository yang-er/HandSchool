using HandSchool.Internal;
using System;

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
        /// 状态更改的事件处理
        /// </summary>
        public event Action IsBusyChanged;

        /// <summary>
        /// ViewModel是否正忙
        /// </summary>
        public bool IsBusy => isBusy;
        
        /// <summary>
        /// 设置忙的函数
        /// </summary>
        /// <param name="value">是否正忙</param>
        /// <param name="tips">忙的提示</param>
        public void SetIsBusy(bool value, string tips = "")
        {
            SetProperty(ref isBusy, value, nameof(IsBusy), IsBusyChanged);
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
