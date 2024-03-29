﻿using HandSchool.Internals;
using HandSchool.ViewModels;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using HandSchool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace HandSchool.Views
{
    /// <summary>
    /// 一个既可以在Xamarin.Forms环境中使用，也可以在本机环境下使用的视图基类。
    /// </summary>
	[ContentProperty("Content")]
    public class ViewObject : ContentPage, IViewPage, IViewLifecycle
    {
        #region Bindable Properties
        
        public static readonly BindableProperty UseTabletModeProperty =
            BindableProperty.Create(
                propertyName: nameof(UseTabletMode),
                returnType: typeof(bool),
                declaringType: typeof(ViewObject),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty UseSafeAreaProperty =
            BindableProperty.Create(
                propertyName: nameof(UseSafeArea),
                returnType: typeof(bool),
                declaringType: typeof(ViewObject),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay);

        #endregion
    
        /// <summary>
        /// 是否开启在iOS下的平板模式
        /// </summary>
        public bool UseTabletMode
        {
            get => (bool)GetValue(UseTabletModeProperty);
            protected set => SetValue(UseTabletModeProperty, value);
        }

        public EventHandler<IsBusyEventArgs> IsBusyChanged;
        /// <summary>
        /// 是否开启在iOS下的边界安全
        /// </summary>
        public bool UseSafeArea
        {
            get => (bool)GetValue(UseSafeAreaProperty);
            protected set => SetValue(UseSafeAreaProperty, value);
        }

        /// <summary>
        /// 与此页面沟通的视图模型
        /// </summary>
        public BaseViewModel ViewModel
        {
            get => BindingContext as BaseViewModel;
            set
            {
                if (ReferenceEquals(BindingContext, value)) return;
                ViewModel?.RemoveView(this);
                BindingContext = value;
                value?.AddView(this);
            }
        }
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(IsBusy):
                    IsBusyChanged?.Invoke(this, new IsBusyEventArgs{IsBusy = IsBusy});
                    break;
            }
        }

        /// <summary>
        /// 工具栏的菜单
        /// </summary>
        public IList<MenuEntry> ToolbarMenu { get; }

        /// <summary>
        /// For internal use only
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ToolbarMenuTracker ToolbarTracker { get; }

        /// <summary>
        /// 添加工具栏菜单。
        /// </summary>
        /// <param name="item">菜单项</param>
        public void AddToolbarEntry(MenuEntry item)
        {
            ToolbarMenu.Add(item);
        }

        public ViewObject()
        {
            ToolbarTracker = new ToolbarMenuTracker();
            ToolbarTracker.List = new ObservableCollection<MenuEntry>();
            ToolbarMenu = ToolbarTracker.List;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (Content != null)
                SetInheritedBindingContext(Content, BindingContext);
            foreach (var menu in ToolbarMenu)
                SetInheritedBindingContext(menu, BindingContext);
        }

        /// <summary>
        /// 是否为模态页面
        /// </summary>
        public bool IsModal { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var safeInsets = On<iOS>().SafeAreaInsets();
            Padding = safeInsets;
            ViewModel?.AddView(this);
        }

        protected override void OnDisappearing()
        {
            ViewModel?.RemoveView(this);
            base.OnDisappearing();
        }


        /// <summary>
        /// 处理导航的参数，在页面显示之前调用。
        /// </summary>
        /// <param name="param">导航的参数内容</param>
        public virtual void SetNavigationArguments(object param) { }
        
        public new INavigate Navigation { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterNavigation(INavigate navigate)
        {
            Navigation = navigate;
        }
        
        #region 视图的响应：通过 MessagingCenter 传递

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
            var args = new RequestInputArguments(title, description, cancel, accept);
            Core.Platform.ViewResponseImpl.ReqInpAsync(this, args);
            return args.Result.Task;
        }
        public Task<string> RequestInputWithPicAsync(string title, string description, string cancel, string accept,byte[]src)
        {
            var args = new RequestInputWithPicArguments(title, description, cancel, accept,src);
            Core.Platform.ViewResponseImpl.ReqInpWPicAsync(this, args);
            return args.Result.Task;
        }
        public Task<string> RequestWebDialogAsync(string title, string description, string url, string cancel, string accept, bool navigation, bool hasInput, string inputHint, WebDialogAdditionalArgs additionalArgs)
        {
            var args = new RequestWebDialogArguments(title, description, url, cancel, accept, navigation, hasInput, inputHint);
            Core.Platform.ViewResponseImpl.ReqWebDiaAsync(this, args, additionalArgs);
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
            var args = new ActionSheetArguments(title, cancel, destruction, buttons);
            Core.Platform.ViewResponseImpl.ReqActAsync(this, args);
            return args.Result.Task;
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
            var args = new AlertArguments(title, description, accept, cancel);
            Core.Platform.ViewResponseImpl.ReqMsgAsync(this, args);
            return args.Result.Task;
        }

        /// <summary>
        /// 弹出消息对话框，用作消息提醒。
        /// </summary>
        /// <param name="title">对话框的标题。</param>
        /// <param name="message">弹出消息的正文。</param>
        /// <param name="button">确认按钮的文字。</param>
        public Task RequestMessageAsync(string title, string message, string button)
        {
            var args = new AlertArguments(title, message, null, button);
            Core.Platform.ViewResponseImpl.ReqMsgAsync(this, args);
            return args.Result.Task;
        }

        /// <summary>
        /// 弹出图表对话框，用作展示图表。
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="title">对话框标题</param>
        /// <param name="close">关闭按钮文字</param>
        public Task RequestChartAsync(Chart chart, string title = "", string close = "关闭")
        {
            var args = new RequestChartArguments(chart, title, close);
            Core.Platform.ViewResponseImpl.ReqChtAsync(this, args);
            return args.ReturnTask;
        }

        public Task NoticeError(string error) => RequestMessageAsync("错误", error, "好");

        #endregion
    }
}
