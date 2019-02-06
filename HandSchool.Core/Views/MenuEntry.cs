using HandSchool.Internals;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace HandSchool.Views
{
    /// <summary>
    /// 菜单项的数据储存类。
    /// </summary>
    public class MenuEntry : BindableObject
    {
        #region Bindable Properties

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                propertyName: nameof(Command),
                returnType: typeof(ICommand),
                declaringType: typeof(MenuEntry));

        public static readonly BindableProperty HiddenForPullProperty =
            BindableProperty.Create(
                propertyName: nameof(HiddenForPull),
                returnType: typeof(bool),
                declaringType: typeof(MenuEntry));

        public static readonly BindableProperty OrderProperty =
            BindableProperty.Create(
                propertyName: nameof(Order),
                returnType: typeof(ToolbarItemOrder),
                declaringType: typeof(MenuEntry),
                defaultValue: ToolbarItemOrder.Default);

        public static readonly BindableProperty UWPIconProperty =
            BindableProperty.Create(
                propertyName: nameof(UWPIcon),
                returnType: typeof(string),
                declaringType: typeof(MenuEntry));

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
                propertyName: nameof(Title),
                returnType: typeof(string),
                declaringType: typeof(MenuEntry));

        #endregion

        /// <summary>
        /// 菜单项承载运行的命令
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// 菜单项的标题
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// 在支持下拉刷新的平台上隐藏
        /// </summary>
        public bool HiddenForPull
        {
            get => (bool)GetValue(HiddenForPullProperty);
            set => SetValue(HiddenForPullProperty, value);
        }

        /// <summary>
        /// 在UWP上显示的图标Unicode
        /// </summary>
        public string UWPIcon
        {
            get => (string)GetValue(UWPIconProperty);
            set => SetValue(UWPIconProperty, value);
        }

        /// <summary>
        /// 在工具栏里的位置
        /// </summary>
        public ToolbarItemOrder Order
        {
            get => (ToolbarItemOrder)GetValue(OrderProperty);
            set => SetValue(OrderProperty, value);
        }

        /// <summary>
        /// 当响应时执行时间。仅可绑定一条。
        /// </summary>
        public event EventHandler Execute
        {
            add => Command = new CommandAction(() => value(this, EventArgs.Empty));
            remove => Command = null;
        }
    }
}