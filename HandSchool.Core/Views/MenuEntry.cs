using HandSchool.Internal;
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
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                propertyName: nameof(Command),
                returnType: typeof(ICommand),
                declaringType: typeof(MenuEntry));

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
                propertyName: nameof(Title),
                returnType: typeof(string),
                declaringType: typeof(MenuEntry));

        /// <summary>
        /// 菜单项承载运行的命令。
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// 菜单项的标题。
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public bool HiddenForPull { get; set; }

        public string UWPIcon { get; set; }
        public ToolbarItemOrder Order { get; set; }
        public string CommandBinding { get; set; }

        public event EventHandler Execute
        {
            add => Command = new CommandAction(() => value(this, EventArgs.Empty));
            remove => Command = null;
        }
    }
}
