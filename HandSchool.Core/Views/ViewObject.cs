using HandSchool.Internals;
using HandSchool.ViewModels;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HandSchool.Views
{
    /// <summary>
    /// 一个既可以在Xamarin.Forms环境中使用，也可以在本机环境下使用的视图基类。
    /// </summary>
	[ContentProperty("Content")]
    public class ViewObject : BindableObject, IViewPage, IViewLifecycle
    {
        #region Bindable Properties

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
                propertyName: nameof(Title),
                returnType: typeof(string),
                declaringType: typeof(ViewObject),
                defaultValue: default(string),
                defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty IsBusyProperty =
            BindableProperty.Create(
                propertyName: nameof(IsBusy),
                returnType: typeof(bool),
                declaringType: typeof(ViewObject),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay);
        
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
        /// 是否正在忙
        /// </summary>
        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        /// <summary>
        /// 是否开启在iOS下的平板模式
        /// </summary>
        public bool UseTabletMode
        {
            get => (bool)GetValue(UseTabletModeProperty);
            protected set => SetValue(UseTabletModeProperty, value);
        }

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
            set { BindingContext = value; if (value != null) value.View = this; }
        }

        /// <summary>
        /// 页面的控件内容，实际呈现的内容。
        /// </summary>
        public View Content { get; set; }

        /// <summary>
        /// 推入时结束此任务。
        /// </summary>
        public Task Pushed { get; set; }
        
        /// <summary>
        /// 页面的标题。
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
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
            Pushed = new Task(() => { });

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
        /// 处理导航的参数，在页面显示之前调用。
        /// </summary>
        /// <param name="param">导航的参数内容</param>
        public virtual void SetNavigationArguments(object param) { }

        #region 页面的生命周期：出现与消失

        /// <summary>
        /// 页面消失时调用。
        /// </summary>
        public event EventHandler Disappearing;

        /// <summary>
        /// 页面显示时调用。
        /// </summary>
        public event EventHandler Appearing;
        
        /// <summary>
        /// 是否为模态页面
        /// </summary>
        public bool IsModal { get; set; }

        /// <summary>
        /// 页面显示时处理。
        /// </summary>
        protected virtual void OnAppearing()
        {
            Appearing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 页面消失时处理。
        /// </summary>
        protected virtual void OnDisappearing()
        {
            Disappearing?.Invoke(this, EventArgs.Empty);
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SendAppearing() => OnAppearing();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SendDisappearing() => OnDisappearing();
        
        #endregion

        #region 页面的导航：平台相关实现

        public INavigate Navigation { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterNavigation(INavigate navigate)
        {
            Navigation = navigate;
        }

        #endregion

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
        
        #endregion
    }
}
