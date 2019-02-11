using HandSchool.Models;
using HandSchool.UWP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace HandSchool.Views
{
    public sealed partial class ValueBox: UserControl
    {
        public static readonly DependencyProperty WrapperProperty =
            DependencyProperty.Register(
                name: nameof(Wrapper), 
                propertyType: typeof(SettingWrapper),
                ownerType: typeof(ValueBox),
                typeMetadata: new PropertyMetadata(
                    defaultValue: default(SettingWrapper),
                    propertyChangedCallback: Callback));

        static void Callback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ValueBox)obj).SetControl((SettingWrapper)args.NewValue);
        }

        public ValueBox()
        {
            InitializeComponent();
        }
        
        public SettingWrapper Wrapper
        {
            get => (SettingWrapper)GetValue(WrapperProperty);
            set => SetValue(WrapperProperty, value);
        }
        
        private void SetControl(SettingWrapper value)
        {
            switch (value.Type)
            {
                case SettingTypes.Integer:
                    var nmr = new Slider
                    {
                        Minimum = value.AttributeData.RangeDown,
                        Maximum = value.AttributeData.RangeUp,
                        TickFrequency = 1,
                        TickPlacement = TickPlacement.Outside
                    };

                    nmr.SetBinding(RangeBase.ValueProperty, "NumericValue");

                    var ind = new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(16, 0, 0, 0)
                    };

                    Grid.SetColumn(ind, 1);
                    ind.SetBinding(TextBlock.TextProperty, "NumericValue");

                    Grid.Children.Add(nmr);
                    Grid.Children.Add(ind);
                    break;

                case SettingTypes.String:
                    var textBox = new TextBox();
                    textBox.SetBinding(TextBox.TextProperty, "StringValue");
                    Grid.Children.Add(textBox);
                    break;

                case SettingTypes.Boolean:
                    var switcher = new ToggleSwitch();
                    Grid.SetColumnSpan(switcher, 2);
                    switcher.SetBinding(ToggleSwitch.IsOnProperty, "BooleanValue");
                    Grid.Children.Add(switcher);
                    break;

                case SettingTypes.Action:
                    var btn = new Button();
                    btn.Content = "执行";
                    btn.SetBinding(ButtonBase.CommandProperty, "ExcuteAction", Wrapper);
                    Grid.Children.Add(btn);
                    break;

                case SettingTypes.Const:
                    Visibility = Visibility.Collapsed;
                    break;

                default:
                    Grid.Children.Add(new TextBlock { Text = "Unknown" });
                    break;
            }
        }
    }
}
