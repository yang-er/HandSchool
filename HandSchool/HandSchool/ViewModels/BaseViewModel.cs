using HandSchool.Internal;
using System;
using System.Threading.Tasks;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 提供了视图模型的基类。
    /// </summary>
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

        /// <summary>
        /// ViewModel 绑定的 View 提供的基础交互接口。
        /// </summary>
        public IViewResponse View { get; set; }

        /// <summary>
        /// 弹出选择对话框，从中选择一个操作。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="cancel">对话框的取消按钮文字。为 <see cref="null"/> 时不显示按钮。</param>
        /// <param name="destruction">对话框的删除按钮文字。为 <see cref="null"/> 时不显示按钮。</param>
        /// <param name="buttons">可选的动作列表每一项的文字。</param>
        /// <returns>按下的按钮标签文字。</returns>
        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return Core.EnsureOnMainThread(() => View.DisplayActionSheet(title, cancel, destruction, buttons));
        }

        /// <summary>
        /// 弹出询问对话框，用作操作确认。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="description">弹出消息的正文。</param>
        /// <param name="cancel">取消按钮的文字。</param>
        /// <param name="accept">确认按钮的文字。</param>
        /// <returns>按下的是否为确定。</returns>
        public Task<bool> ShowAskMessage(string title, string description, string cancel, string accept)
        {
            return Core.EnsureOnMainThread(() => View.ShowAskMessage(title, description, cancel, accept));
        }

        /// <summary>
        /// 弹出消息对话框，用作消息提醒。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="message">弹出消息的正文。</param>
        /// <param name="button">确认按钮的文字。</param>
        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Core.EnsureOnMainThread(() => View.ShowMessage(title, message, button));
        }

        #endregion
    }
}
