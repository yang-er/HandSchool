using HandSchool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingAtom
    {
        public static readonly BindableProperty WrapperProperty =
            BindableProperty.Create(
                propertyName: nameof(Wrapper),
                returnType: typeof(SettingWrapper),
                declaringType: typeof(SettingAtom),
                defaultValue: default(SettingWrapper),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: WrapperChanged);
        
        public static readonly BindableProperty NumericValueProperty =
            BindableProperty.Create(
                propertyName: nameof(NumericValue),
                returnType: typeof(int),
                declaringType: typeof(SettingAtom),
                defaultValue: 0,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: ValueChanged);
        
        public static readonly BindableProperty StringValueProperty =
            BindableProperty.Create(
                propertyName: nameof(StringValue),
                returnType: typeof(string),
                declaringType: typeof(SettingAtom),
                defaultValue: "",
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: ValueChanged);
        
        public static readonly BindableProperty BooleanValueProperty =
            BindableProperty.Create(
                propertyName: nameof(BooleanValue),
                returnType: typeof(bool),
                declaringType: typeof(SettingAtom),
                defaultValue: false,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: ValueChanged);

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
                propertyName: nameof(Title),
                returnType: typeof(string),
                declaringType: typeof(SettingAtom),
                defaultValue: "",
                defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(
                propertyName: nameof(Description),
                returnType: typeof(string),
                declaringType: typeof(SettingAtom),
                defaultValue: "",
                defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty TypeProperty =
            BindableProperty.Create(
                propertyName: nameof(Type),
                returnType: typeof(SettingTypes),
                declaringType: typeof(SettingAtom),
                defaultValue: SettingTypes.Unknown,
                defaultBindingMode: BindingMode.OneWay);

        private static void WrapperChanged(BindableObject bind, object old, object @new)
        {
            ((SettingAtom)bind).SetWrapper((SettingWrapper)@new);
        }

        private static void ValueChanged(BindableObject bind, object old, object @new)
        {
            ((SettingAtom)bind).Wrapper.Value = @new;
        }

        /// <summary>
        /// 设置类型
        /// </summary>
        public SettingTypes Type
        {
            get => (SettingTypes)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        /// <summary>
        /// 数字值
        /// </summary>
        public int NumericValue
        {
            get => (int)GetValue(NumericValueProperty);
            set => SetValue(NumericValueProperty, value);
        }

        /// <summary>
        /// 开关值
        /// </summary>
        public bool BooleanValue
        {
            get => (bool)GetValue(BooleanValueProperty);
            set => SetValue(BooleanValueProperty, value);
        }

        /// <summary>
        /// 字符串值
        /// </summary>
        public string StringValue
        {
            get => (string)GetValue(StringValueProperty);
            set => SetValue(StringValueProperty, value);
        }

        /// <summary>
        /// 单元格标题
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// 单元格说明
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        /// <summary>
        /// 包装设置属性
        /// </summary>
        public SettingWrapper Wrapper
        {
            get => (SettingWrapper)GetValue(WrapperProperty);
            set => SetValue(WrapperProperty, value);
        }
        
        public SettingAtom()
        {
            InitializeComponent();
            CornerRadius = Device.RuntimePlatform switch
            {
                Device.iOS => 15,
                _ => 10
            };
        }

        private void SetBinding(BindableObject bind, BindableProperty prop, string name)
        {
            bind.SetBinding(prop, new Binding(name, source: this));
        }

        private void SetWrapper(SettingWrapper wrapper)
        {
            if (wrapper is null) return;
            BindingContext = wrapper;
            Title = wrapper.Title;
            Description = wrapper.Description;

            switch (wrapper.Type)
            {
                case SettingTypes.Integer:
                    NumericValue = (int)Wrapper.Value;
                    break;

                case SettingTypes.String:
                    StringValue = (string)Wrapper.Value;
                    break;

                case SettingTypes.Boolean:
                    BooleanValue = (bool)Wrapper.Value;
                    break;
            }

            if (Device.RuntimePlatform == Device.UWP) return;
            SetControl(wrapper);
        }

        private void SetControl(SettingWrapper wrapper)
        {
            switch (wrapper.Type)
            {
                case SettingTypes.Integer:
                    var numberRanger = new Slider
                    {
                        Maximum = Wrapper.AttributeData.RangeUp,
                        Minimum = Wrapper.AttributeData.RangeDown,
                        Value = 1,
                    };

                    var indicator = new Label
                    {
                        VerticalOptions = LayoutOptions.Center
                    };

                    NumericValue = (int)Wrapper.Value;
                    SetBinding(numberRanger, Slider.ValueProperty, nameof(NumericValue));
                    SetBinding(indicator, Label.TextProperty, nameof(NumericValue));

                    Grid.SetColumn(indicator, 1);
                    Grid.SetColumn(numberRanger, 0);
                    BoxGrid.Children.Add(numberRanger);
                    BoxGrid.Children.Add(indicator);
                    break;

                case SettingTypes.String:
                    StringValue = (string)Wrapper.Value;
                    var textBox = new Entry();
                    SetBinding(textBox, Entry.TextProperty, nameof(StringValue));
                    Grid.SetColumnSpan(textBox, 2);
                    BoxGrid.Children.Add(textBox);
                    break;

                case SettingTypes.Boolean:
                    var switcher = new Switch
                    {
                        HorizontalOptions = LayoutOptions.Start
                    };

                    BooleanValue = (bool)Wrapper.Value;
                    SetBinding(switcher, Switch.IsToggledProperty, nameof(BooleanValue));
                    Grid.SetColumnSpan(switcher, 2);
                    BoxGrid.Children.Add(switcher);
                    break;

                case SettingTypes.Action:
                case SettingTypes.Const:
                    break;

                default:
                    BoxGrid.Children.Add(new Label { Text = "Unknown error" });
                    break;
            }
        }
    }
}