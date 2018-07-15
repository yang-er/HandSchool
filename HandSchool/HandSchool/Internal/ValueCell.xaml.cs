using HandSchool.Internal;
using HandSchool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ValueCell : ContentView
    {
        public static readonly BindableProperty WrapperProperty =
            BindableProperty.Create(nameof(Wrapper), typeof(SettingWrapper), typeof(ValueCell), null);

        public static readonly BindableProperty NumericValueProperty =
            BindableProperty.Create(nameof(NumericValue), returnType: typeof(int), declaringType: typeof(ValueCell), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ((bindable, oldvalue, newvalue) => (bindable as ValueCell).Wrapper.Value = newvalue));

        public static readonly BindableProperty TypeProperty =
            BindableProperty.Create(nameof(Type), typeof(SettingTypes), typeof(ValueCell), SettingTypes.Unkown, propertyChanged: ((bindable, oldvalue, newvalue) => (bindable as ValueCell).SetControl((SettingTypes)newvalue)));

        public static readonly BindableProperty StringValueProperty =
            BindableProperty.Create(nameof(StringValue), typeof(string), typeof(ValueCell), "", BindingMode.TwoWay, propertyChanged: ((bindable, oldvalue, newvalue) => (bindable as ValueCell).Wrapper.Value = newvalue));

        public static readonly BindableProperty BooleanValueProperty =
            BindableProperty.Create(nameof(BooleanValue), returnType: typeof(bool), declaringType: typeof(ValueCell), defaultValue: false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ((bindable, oldvalue, newvalue) => (bindable as ValueCell).Wrapper.Value = newvalue));

        public static readonly BindableProperty AttributeProperty =
            BindableProperty.Create(nameof(Attribute), typeof(SettingsAttribute), typeof(ValueCell), default(SettingsAttribute), BindingMode.OneWay);
        
        public ValueCell()
        {
            InitializeComponent();
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

        public SettingsAttribute Attribute
        {
            get => (SettingsAttribute)GetValue(AttributeProperty);
            set => SetValue(AttributeProperty, value);
        }

        public int NumericValue
        {
            get => (int)GetValue(NumericValueProperty);
            set => SetValue(NumericValueProperty, value);
        }

        public bool BooleanValue
        {
            get => (bool)GetValue(BooleanValueProperty);
            set => SetValue(BooleanValueProperty, value);
        }

        public string StringValue
        {
            get => (string)GetValue(StringValueProperty);
            set => SetValue(StringValueProperty, value);

        }
        
        private void SetControl(SettingTypes value)
        {
            switch (value)
            {
                case SettingTypes.Integer:
                    NumericValue = (int)Wrapper.Value;

                    var nmr = new Slider
                    {
                        Maximum = Attribute.RangeUp,
                        Minimum = Attribute.RangeDown,
                        Value = 1,
                    };

                    nmr.SetBinding(Slider.ValueProperty, new Binding { Source = this, Path = nameof(NumericValue), Mode = BindingMode.TwoWay });
                    var ind = new Label { VerticalOptions = LayoutOptions.Center };
                    Grid.SetColumn(ind, 1);
                    Grid.SetColumn(nmr, 0);

                    ind.SetBinding(Label.TextProperty, new Binding { Source = this, Path = nameof(NumericValue) });
                    grid.Children.Add(nmr);
                    grid.Children.Add(ind);
                    break;

                case SettingTypes.String:
                    StringValue = (string)Wrapper.Value;

                    var tb = new Entry();
                    Grid.SetColumnSpan(tb, 2);
                    tb.SetBinding(Entry.TextProperty, new Binding { Source = this, Path = nameof(StringValue), Mode = BindingMode.TwoWay });
                    grid.Children.Add(tb);
                    break;

                case SettingTypes.Const:
                    break;

                case SettingTypes.Boolean:
                    BooleanValue = (bool)Wrapper.Value;

                    var sw = new Switch();
                    Grid.SetColumnSpan(sw, 2);
                    sw.SetBinding(Switch.IsToggledProperty, new Binding { Source = this, Path = nameof(BooleanValue), Mode = BindingMode.TwoWay });
                    grid.Children.Add(sw);
                    break;

                default:
                    grid.Children.Add(new Label { Text = "Unknown" });
                    break;
            }
        }
    }
}