using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    /// <summary>
    /// 文字提示或询问的对话框。
    /// </summary>
    public sealed partial class TextDialog : ContentDialog
    {
        /// <summary>
        /// 创建文字提示对话框。
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">提示内容</param>
        /// <param name="button">按钮文字</param>
        public TextDialog(string title, string message, string button)
        {
            InitializeComponent();
            Title = title;
            TextBlock.Text = message;
            CloseButtonText = button;
            PrimaryButtonText = "";
        }

        /// <summary>
        /// 创建文字输入对话框。
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">提示内容</param>
        /// <param name="cancel">取消按钮文字</param>
        /// <param name="accept">接受按钮文字</param>
        public TextDialog(string title, string message, string accept, string cancel, string defaultValue)
        {
            InitializeComponent();
            Title = title;
            TextBlock.Text = message;
            CloseButtonText = cancel;
            PrimaryButtonText = accept;
            TextBox.Visibility = Visibility.Visible;
            TextBox.Text = defaultValue;
            TextBlock.Margin = new Thickness(0, 0, 0, 15);
        }
        
        /// <summary>
        /// 创建文字提示对话框。
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">提示内容</param>
        /// <param name="cancel">取消按钮文字</param>
        /// <param name="accept">接受按钮文字</param>
        public TextDialog(string title, string message, string accept, string cancel)
        {
            InitializeComponent();
            Title = title;
            TextBlock.Text = message;
            CloseButtonText = cancel;
            PrimaryButtonText = accept;
        }
    }
}
