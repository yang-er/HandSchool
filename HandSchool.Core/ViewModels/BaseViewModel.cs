using HandSchool.Internal;
using System;
using System.Threading.Tasks;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 提供了视图模型的基类。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    /// <inheritdoc cref="IViewResponse" />
    public class BaseViewModel : NotifyPropertyChanged, IViewResponse
    {
        bool isBusy = false;
        string title = string.Empty;

        /// <summary>
        /// 忙碌状态更改的事件处理。
        /// </summary>
        public event Action IsBusyChanged;

        /// <summary>
        /// 视图模型是否处于忙碌状态。
        /// </summary>
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value, nameof(IsBusy), IsBusyChanged);
        }

        /// <summary>
        /// 设置忙碌状态的函数。
        /// </summary>
        /// <param name="value">目前的忙碌状态。</param>
        /// <param name="tips">忙碌状态对话框里的消息提示，表示目前正在进行什么。</param>
        public void SetIsBusy(bool value, string tips = "")
        {
            SetProperty(ref isBusy, value, nameof(IsBusy), IsBusyChanged);
        }

        /// <summary>
        /// 视图显示的窗口标题。
        /// </summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        #region IViewResponse 实现
        
        public IViewResponse View { get; set; }
        
        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return Core.Platform.EnsureOnMainThread(() => View.DisplayActionSheet(title, cancel, destruction, buttons));
        }
        
        public Task<bool> ShowAskMessage(string title, string description, string cancel, string accept)
        {
            return Core.Platform.EnsureOnMainThread(() => View.ShowAskMessage(title, description, cancel, accept));
        }
        
        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Core.Platform.EnsureOnMainThread(() => View.ShowMessage(title, message, button));
        }

        #endregion
    }
}
