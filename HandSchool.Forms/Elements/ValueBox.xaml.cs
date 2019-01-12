using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    /// <summary>
    /// 设置页面的值单元格
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ValueCell : ContentView
    {
        /// <summary>
        /// 为 Wrapper 做后部存储
        /// </summary>
        public static readonly BindableProperty WrapperProperty =
            BindableProperty.Create(
                propertyName: nameof(Wrapper),
                returnType: typeof(SettingWrapper),
                declaringType: typeof(ValueCell),
                defaultValue: null);

        /// <summary>
        /// 为 NumericValue 做后部存储
        /// </summary>
        public static readonly BindableProperty NumericValueProperty =
            BindableProperty.Create(
                propertyName: nameof(NumericValue),
                returnType: typeof(int),
                declaringType: typeof(ValueCell),
                defaultValue: 0,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bind, old, newv) => (bind as ValueCell).Wrapper.Value = newv);

        /// <summary>
        /// 为 Type 做后部存储
        /// </summary>
        public static readonly BindableProperty TypeProperty =
            BindableProperty.Create(
                propertyName: nameof(Type),
                returnType: typeof(SettingTypes),
                declaringType: typeof(ValueCell),
                defaultValue: SettingTypes.Unknown,
                propertyChanged: (bind, old, newv) => (bind as ValueCell).SetControl((SettingTypes)newv));

        /// <summary>
        /// 为 StringValue 做后部存储
        /// </summary>
        public static readonly BindableProperty StringValueProperty =
            BindableProperty.Create(
                propertyName: nameof(StringValue),
                returnType: typeof(string),
                declaringType: typeof(ValueCell),
                defaultValue: "",
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bind, old, newv) => (bind as ValueCell).Wrapper.Value = newv);

        /// <summary>
        /// 为 BooleanValue 做后部存储
        /// </summary>
        public static readonly BindableProperty BooleanValueProperty =
            BindableProperty.Create(
                propertyName: nameof(BooleanValue),
                returnType: typeof(bool),
                declaringType: typeof(ValueCell),
                defaultValue: false,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bind, old, newv) => (bind as ValueCell).Wrapper.Value = newv);

        /// <summary>
        /// 为 Attribute 做后部存储
        /// </summary>
        public static readonly BindableProperty AttributeProperty =
            BindableProperty.Create(
                propertyName: nameof(Attribute),
                returnType: typeof(SettingsAttribute),
                declaringType: typeof(ValueCell),
                defaultValue: default(SettingsAttribute),
                defaultBindingMode: BindingMode.OneWay);
        
        /// <summary>
        /// 值单元格
        /// </summary>
        public ValueCell()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 包装设置属性
        /// </summary>
        public SettingWrapper Wrapper
        {
            get => GetValue(WrapperProperty) as SettingWrapper;
            set => SetValue(WrapperProperty, value);
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
        /// 设置特性元数据
        /// </summary>
        public SettingsAttribute Attribute
        {
            get => (SettingsAttribute)GetValue(AttributeProperty);
            set => SetValue(AttributeProperty, value);
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
        /// 设置内部控件
        /// </summary>
        /// <param name="value">值</param>
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
                    sw.HorizontalOptions = LayoutOptions.Start;
                    sw.SetBinding(Switch.IsToggledProperty, new Binding { Source = this, Path = nameof(BooleanValue), Mode = BindingMode.TwoWay });
                    grid.Children.Add(sw);
                    break;

                case SettingTypes.Action:
                    var gesture = new TapGestureRecognizer();
                    gesture.SetBinding(TapGestureRecognizer.CommandProperty, new Binding { Source = Wrapper, Path = "ExcuteAction", Mode = BindingMode.OneTime });
                    (Parent as StackLayout).GestureRecognizers.Add(gesture);
                    break;

                default:
                    grid.Children.Add(new Label { Text = "Unknown" });
                    break;
            }
        }
    }
}