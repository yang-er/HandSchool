using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    public sealed partial class TextDialog : ContentDialog
    {
        public TextDialog(string title, string message, string button = "确认")
        {
            InitializeComponent();
            Title = title;
            TextBlock.Text = message;
            CloseButtonText = button;
        }
    }
}
