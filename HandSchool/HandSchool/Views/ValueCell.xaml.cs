using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ValueCell : ViewCell
	{
        #region propertys
        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(object), typeof(ValueCell), null, BindingMode.TwoWay);

        public static readonly BindableProperty TitleTextProperty =
    BindableProperty.Create(nameof(TitleText), typeof(object), typeof(ValueCell), null, BindingMode.TwoWay);

        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(nameof(TitleText), typeof(object), typeof(ValueCell), null, BindingMode.TwoWay);

        public static readonly BindableProperty NumericValueProperty =
            BindableProperty.Create(nameof(NumericValue), returnType: typeof(int), declaringType: typeof(ValueCell), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay, propertyChanged: ((bindable, oldvalue, newvalue) => bindable.SetValue(NumericValueProperty, newvalue)));


        public static readonly BindableProperty TypeProperty =
                   BindableProperty.Create(nameof(Type), typeof(SettingTypes), typeof(ValueCell), SettingTypes.Unkown, propertyChanged: ((bindable, oldvalue, newvalue) => (bindable as ValueCell).SetControl((SettingTypes)newvalue)));

        public static readonly BindableProperty StringValueProperty =
            BindableProperty.Create(nameof(StringValue), typeof(string), typeof(ValueCell), "", BindingMode.TwoWay, propertyChanged: ((bindable, oldvalue, newvalue) => bindable.SetValue(ValueProperty, newvalue)));

        public static readonly BindableProperty AttributeProperty =
            BindableProperty.Create(nameof(Attribute), typeof(SettingsAttribute), typeof(ValueCell), default(SettingsAttribute), BindingMode.TwoWay);
        #endregion

        public ValueCell()
        {
            InitializeComponent();
        }
        #region values
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
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
        
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }

        }
        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }
        public string StringValue
        {
            get { return (string)GetValue(StringValueProperty); }
            set { SetValue(StringValueProperty, value); }

        }

        #endregion

        private void SetControl(SettingTypes value)
        {
            switch (value)
            {
                case SettingTypes.Integer:
                    var nmr = new Slider
                    {
                        Maximum = Attribute.RangeUp,
                        Minimum = Attribute.RangeDown,
                        Value = 1,
                    };
                    var title = new Label
                    {
                        Text = TitleText,
                        FontSize = 20
                    };
                    var descriptionlabel = new Label
                    {
                        Text = Description,
                        FontSize = 20
                    };
                    nmr.SetBinding(Slider.ValueProperty, new Binding { Source = this, Path = "NumericValue", Mode = BindingMode.TwoWay });
                    var ind = new Label();
                    ind.VerticalOptions = LayoutOptions.Center;
                    Grid.SetColumn(ind, 1);
                    Grid.SetRow(ind, 1);
                    Grid.SetColumn(nmr, 0);
                    Grid.SetRow(nmr, 1);

                    Grid.SetColumn(title, 0);
                    Grid.SetRow(title, 0);
                    Grid.SetColumn(descriptionlabel,0);
                    Grid.SetRow(descriptionlabel,2);
                    ind.SetBinding(Label.TextProperty, new Binding { Source = this, Path = "NumericValue" });
                    grid.Children.Add(nmr);
                    grid.Children.Add(ind);
                    grid.Children.Add(descriptionlabel);
                    grid.Children.Add(title);
                    break;
                case SettingTypes.String:
                    var tb = new Entry();
                    Grid.SetRow(tb, 1);
                    tb.SetBinding(Entry.TextProperty, new Binding { Source = this, Path = "StringValue", Mode = BindingMode.TwoWay });
                    grid.Children.Add(tb);
                    break;
                default:
                    grid.Children.Add(new Entry { Text = "Unknown" });
                    break;
            }
        }
    }
}