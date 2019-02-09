using HandSchool.Internal;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Views
{
    /// <summary>
    /// 基于Xamarin封装的页面文件
    /// </summary>
    public class ViewPage : ContentPage, IViewPage
    {
        public const string RequestInputSignalName = "HandSchool.AskInput";

        /// <summary>
        /// 与此页面沟通的视图模型
        /// </summary>
        public BaseViewModel ViewModel
        {
            get => BindingContext as BaseViewModel;

            set
            {
                BindingContext = value;
                value.View = this;
                SetBinding(IsBusyProperty, new Binding("IsBusy", BindingMode.OneWay));
            }
        }

        /// <summary>
        /// 是否显示正忙的弹窗
        /// </summary>
        public bool ShowIsBusyDialog { get; protected set; }

        /// <summary>
        /// 是否使用平板电脑的分割页面版本优化。
        /// </summary>
        public bool TabletEnabled { get; set; } = false;

        /// <summary>
        /// 是否为模态框
        /// </summary>
        public bool IsModal { get; private set; }
        
        void IViewPage.AddToolbarEntry(MenuEntry item)
        {
            var tool = new ToolbarItem
            {
                Text = item.Title,
                Order = item.Order,
            };

            if (item.CommandBinding is null)
                tool.Command = item.Command;
            else
                tool.SetBinding(MenuItem.CommandProperty, item.CommandBinding, BindingMode.OneTime);

            ToolbarItems.Add(tool);
        }

        /// <summary>
        /// 视图导航控制器
        /// </summary>
        public new INavigate Navigation => throw new NotImplementedException();
        
        #region INavigate Page Impl thanks to shanhongyue

        private Task ContinueTask { get; set; }

        private bool Destoried { get; set; }

        private void Page_Disappearing(object sender, EventArgs e)
        {
            if (Destoried) return;
            Destoried = true;
            Disappearing -= Page_Disappearing;
            ContinueTask?.Start();
        }

        private void Page_Popped(object sender, NavigationEventArgs e)
        {
            if (Destoried) return;
            Destoried = true;
            (sender as NavigationPage).Popped -= Page_Popped;
            ContinueTask?.Start();
        }

        /// <summary>
        /// 关闭此视图页面。
        /// </summary>
        public async Task CloseAsync()
        {
            if (IsModal)
                await base.Navigation.PopModalAsync();
            else
                await Navigation.PopAsync();
        }

        /// <summary>
        /// 显示此视图页面。
        /// </summary>
        /// <param name="parent">浏览导航控制器。</param>
        public async Task ShowAsync(INavigate parent = null)
        {
            Destoried = false;
            ContinueTask = new Task(() => { });

            if (parent is null)
            {
                Disappearing += Page_Disappearing;
                IsModal = true;
                await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
            }
            else
            {
                await parent.PushAsync(this);

                if (Parent is NavigationPage navigationPage)
                {
                    navigationPage.Popped += Page_Popped;
                }
                else
                {
                    Disappearing += Page_Disappearing;
                    Core.Log("Not support this kind of access, may occured some errors.");
                    Core.Log("Maybe double tapped but event is one tap.");
                }
            }

            await ContinueTask;
        }

        #endregion

        #region IViewResponse Impl

        /// <summary>
        /// 弹出询问对话框，用作请求输入内容。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="description">弹出消息的正文。</param>
        /// <param name="cancel">取消按钮的文字。</param>
        /// <param name="accept">确认按钮的文字。</param>
        /// <returns>用户输入的内容，如果点击取消则为null。</returns>
        public Task<string> RequestInputAsync(string title, string description, string cancel, string accept)
        {
            var args = new RequestInputArguments
            {
                Title = title,
                Message = description,
                Cancel = cancel,
                Accept = accept
            };

            MessagingCenter.Send(this, RequestInputSignalName, args);
            return args.Result.Task;
        }

        /// <summary>
        /// 弹出选择对话框，从中选择一个操作。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="cancel">对话框的取消按钮文字。为 <see cref="null"/> 时不显示按钮。</param>
        /// <param name="destruction">对话框的删除按钮文字。为 <see cref="null"/> 时不显示按钮。</param>
        /// <param name="buttons">可选的动作列表每一项的文字。</param>
        /// <returns>按下的按钮标签文字。</returns>
        public Task<string> RequestActionAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return DisplayActionSheet(title, cancel, destruction, buttons);
        }

        /// <summary>
        /// 弹出询问对话框，用作操作确认。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="description">弹出消息的正文。</param>
        /// <param name="cancel">取消按钮的文字。</param>
        /// <param name="accept">确认按钮的文字。</param>
        /// <returns>按下的是否为确定。</returns>
        public Task<bool> RequestAnswerAsync(string title, string description, string cancel, string accept)
        {
            return DisplayAlert(title, description, accept, cancel);
        }

        /// <summary>
        /// 弹出消息对话框，用作消息提醒。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="message">弹出消息的正文。</param>
        /// <param name="button">确认按钮的文字。</param>
        public Task RequestMessageAsync(string title, string message, string button)
        {
            return DisplayAlert(title, message, button);
        }

        #endregion
    }
}