using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    public sealed partial class CardView : UserControl
    {
        public Symbol Icon { get; set; } = Symbol.Emoji;
        public UIElementCollection Children => StackPanel.Children;

        public CardView()
        {
            InitializeComponent();
        }
    }
}
