using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HandSchool.Views
{
    public sealed partial class CardView : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Symbol), typeof(CardView), new PropertyMetadata(Symbol.Emoji));

        // Thanks for help from cnbluefire@github

        public Symbol Icon
        {
            get => (Symbol)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public UIElementCollection Children => StackPanel.Children;

        public double ItemSpacing
        {
            get => StackPanel.Spacing;
            set => StackPanel.Spacing = value;
        }

        public CardView()
        {
            InitializeComponent();
        }
    }
}
