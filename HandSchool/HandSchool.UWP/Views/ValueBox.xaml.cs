using HandSchool.Models;
using HandSchool.Services;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace HandSchool.UWP.Views
{
    public sealed partial class ValueBox: UserControl
    {
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(ValueBox), new PropertyMetadata(null));
        public static readonly DependencyProperty NumericValueProperty =
            DependencyProperty.Register(nameof(NumericValue), typeof(int), typeof(ValueBox), new PropertyMetadata(0, (d, e) => d.SetValue(ValueProperty, e.NewValue)));
        public static readonly DependencyProperty BooleanValueProperty =
            DependencyProperty.Register(nameof(BooleanValue), typeof(bool), typeof(ValueBox), new PropertyMetadata(false, (d, e) => d.SetValue(ValueProperty, e.NewValue)));
        public static readonly DependencyProperty StringValueProperty =
            DependencyProperty.Register(nameof(StringValue), typeof(string), typeof(ValueBox), new PropertyMetadata("", (d, e) => d.SetValue(ValueProperty, e.NewValue)));
        public static readonly DependencyProperty TypeProperty = 
            DependencyProperty.Register(nameof(Type), typeof(SettingTypes), typeof(ValueBox), new PropertyMetadata(SettingTypes.Unkown, (d, e) => (d as ValueBox).SetControl((SettingTypes)e.NewValue)));
        public static readonly DependencyProperty AttributeProperty = 
            DependencyProperty.Register(nameof(Attribute), typeof(SettingsAttribute), typeof(ValueBox), new PropertyMetadata(default(SettingsAttribute)));
        public static readonly DependencyProperty WrapperProperty =
            DependencyProperty.Register(nameof(Wrapper), typeof(SettingWrapper), typeof(ValueBox), new PropertyMetadata(default(SettingWrapper)));

        public ValueBox()
        {
            InitializeComponent();
        }
        
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public SettingWrapper Wrapper
        {
            get => GetValue(WrapperProperty) as SettingWrapper;
            set => SetValue(WrapperProperty, value);
        }

        public SettingTypes Type
        {
            get => (SettingTypes)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }
        
        private int NumericValue
        {
            get => (int)GetValue(NumericValueProperty);
            set => SetValue(NumericValueProperty, value);
        }

        private bool BooleanValue
        {
            get => (bool)GetValue(BooleanValueProperty);
            set => SetValue(BooleanValueProperty, value);
        }

        private string StringValue
        {
            get => (string)GetValue(StringValueProperty);
            set => SetValue(StringValueProperty, value);
        }
        
        public SettingsAttribute Attribute
        {
            get => (SettingsAttribute)GetValue(AttributeProperty);
            set => SetValue(AttributeProperty, value); 
        }

        private void SetControl(SettingTypes value)
        {
            switch (value)
            {
                case SettingTypes.Integer:
                    NumericValue = (int)Value;

                    var nmr = new Slider
                    {
                        Minimum = Attribute.RangeDown,
                        Maximum = Attribute.RangeUp,
                        TickFrequency = 1,
                        TickPlacement = TickPlacement.Outside
                    };
                    nmr.SetBinding(RangeBase.ValueProperty, new Binding { Source = this, Path = new PropertyPath(nameof(NumericValue)), Mode = BindingMode.TwoWay });

                    var ind = new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(16, 0, 0, 0)
                    };
                    Grid.SetColumn(ind, 1);
                    ind.SetBinding(TextBlock.TextProperty, new Binding { Source = this, Path = new PropertyPath(nameof(NumericValue)) });

                    Grid.Children.Add(nmr);
                    Grid.Children.Add(ind);
                    break;

                case SettingTypes.String:
                    StringValue = (string)Value;
                    var tb = new TextBox();
                    tb.SetBinding(TextBox.TextProperty, new Binding { Source = this, Path = new PropertyPath(nameof(StringValue)), Mode = BindingMode.TwoWay });
                    Grid.Children.Add(tb);
                    break;

                case SettingTypes.Const:
                    break;

                case SettingTypes.Boolean:
                    BooleanValue = (bool)Value;
                    var sw = new ToggleSwitch();
                    Grid.SetColumnSpan(sw, 2);
                    sw.SetBinding(ToggleSwitch.IsOnProperty, new Binding { Source = this, Path = new PropertyPath(nameof(BooleanValue)), Mode = BindingMode.TwoWay });
                    Grid.Children.Add(sw);
                    break;

                case SettingTypes.Action:
                    (Parent as Control).Tapped += (s,e) => Wrapper.MethodInfo.CreateDelegate(typeof(Action)).DynamicInvoke();
                    break;

                default:
                    Grid.Children.Add(new TextBlock { Text = "Unknown" });
                    break;
            }
        }
    }
}
