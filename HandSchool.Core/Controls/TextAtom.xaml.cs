using HandSchool.Internal;
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextAtom : Internal.TouchableFrame
    {

        ColumnDefinition second_col;
        public bool OnTop
        {
            get => (bool)base.GetValue(OnTopProperty);
            set => SetValue(OnTopProperty, value);
        }
        public double FirstProportion
        {
            get => (double)GetValue(FirstProportionProperty);
            set
            {
                SetValue(FirstProportionProperty, value);
            }
        }
        public bool HasSecond
        {
            get => (bool)GetValue(HasSecondProperty);
            set
            {
                SetValue(HasSecondProperty, value);
            }
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        public string AfterTitle
        {
            get => (string)GetValue(AfterTitleProperty);
            set
            {
                SetValue(AfterTitleProperty, value);
            }
        }
        public string ContentText
        {
            get => (string)GetValue(ContentTextProperty);
            set
            {
                SetValue(ContentTextProperty, value);
            }
        }
        public string SecondTitle
        {
            get => (string)GetValue(SecondTitleProperty);
            set
            {
                SetValue(SecondTitleProperty, value);
            }
        }
        public string SecondContent
        {
            get => (string)GetValue(SecondContentProperty);
            set
            {
                SetValue(SecondContentProperty, value);
            }
        }
        public Color SecondContentColor
        {
            get => (Color)GetValue(SecondContentColorProperty);
            set
            {
                SetValue(SecondContentColorProperty, value);
            }
        }
        public Color SecondTitleColor
        {
            get => (Color)GetValue(SecondTitleColorProperty);
            set
            {
                SetValue(SecondTitleColorProperty, value);
            }
        }
        protected override void OnPropertyChanged([CallerMemberName] string n = null)
        {
            if (string.IsNullOrEmpty(n)) return;
            Core.Platform.EnsureOnMainThread(() =>
            {
                switch (n)
                {
                    case nameof(OnTop):
                        if (on_top != null) on_top.IsVisible = OnTop;
                        break;
                    case nameof(Title):
                        if (title != null) title.Text = Title;
                        break;
                    case nameof(ContentText):
                        if (content_text != null) content_text.Text = ContentText;
                        break;
                    case nameof(AfterTitle):
                        if (after_title != null) after_title.Text = AfterTitle;
                        break;
                    case nameof(SecondContent):
                        if (second_content != null) second_content.Text = SecondContent;
                        break;
                    case nameof(SecondTitle):
                        if (second_title != null) second_title.Text = SecondTitle;
                        break;
                    case nameof(SecondContentColor):
                        if (second_content != null) second_content.TextColor = SecondContentColor;
                        break;
                    case nameof(SecondTitleColor):
                        if (second_title != null) 
                            second_title.TextColor = SecondTitleColor;
                        break;
                    case nameof(HasSecond):
                        if (second_content != null && second_title != null)
                        {
                            if (HasSecond)
                            {
                                grid.ColumnDefinitions.Add(second_col);
                                grid.ColumnDefinitions[0].Width = new GridLength(FirstProportion, GridUnitType.Star);
                                grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                            }
                            second_title.IsVisible = second_content.IsVisible = HasSecond;
                            if (!HasSecond) { grid.RowDefinitions.RemoveAt(1);
                                grid.ColumnDefinitions[0].Width = new GridLength(1,GridUnitType.Auto);
                            }
                        }
                        break;
                    case nameof(FirstProportion):
                        grid.ColumnDefinitions[0].Width = new GridLength(FirstProportion, GridUnitType.Star);
                        break;
                    default:
                        base.OnPropertyChanged(n);
                        break;
                }
            }
            );
        }
        public static readonly BindableProperty OnTopProperty =
            BindableProperty.Create(
                propertyName: nameof(OnTop),
                returnType: typeof(bool),
                declaringType: typeof(TextAtom),
                defaultValue: true);
        public static readonly BindableProperty FirstProportionProperty =
            BindableProperty.Create(
                propertyName: nameof(FirstProportion),
                returnType: typeof(double),
                declaringType: typeof(TextAtom),
                defaultValue: 4.00);
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
                propertyName: nameof(Title),
                returnType: typeof(string),
                declaringType: typeof(TextAtom),
                defaultValue: "");
        public static readonly BindableProperty AfterTitleProperty =
            BindableProperty.Create(
                propertyName: nameof(AfterTitle),
                returnType: typeof(string),
                declaringType: typeof(TextAtom),
                defaultValue: "");
        public static readonly BindableProperty ContentTextProperty =
            BindableProperty.Create(
                propertyName: nameof(ContentText),
                returnType: typeof(string),
                declaringType: typeof(TextAtom),
                defaultValue: "");
        public static readonly BindableProperty SecondTitleProperty =
            BindableProperty.Create(
                propertyName: nameof(SecondTitle),
                returnType: typeof(string),
                declaringType: typeof(TextAtom),
                defaultValue: "");
        public static readonly BindableProperty SecondContentProperty =
            BindableProperty.Create(
                propertyName: nameof(SecondContent),
                returnType: typeof(string),
                declaringType: typeof(TextAtom),
                defaultValue: "");
        public static readonly BindableProperty SecondContentColorProperty =
            BindableProperty.Create(
                propertyName: nameof(SecondContentColor),
                returnType: typeof(Color),
                declaringType: typeof(TextAtom),
                defaultValue: Color.Gray);
        public static readonly BindableProperty SecondTitleColorProperty =
            BindableProperty.Create(
                propertyName: nameof(SecondTitleColor),
                returnType: typeof(Color),
                declaringType: typeof(TextAtom),
                defaultValue: Color.Red);

        public static readonly BindableProperty HasSecondProperty =
            BindableProperty.Create(
                propertyName: nameof(HasSecond),
                returnType: typeof(bool),
                declaringType: typeof(TextAtom),
                defaultValue: true);
        public TextAtom()
        {
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case "iOS":
                    HasShadow = false;
                    BorderColor = Color.FromRgb(230, 230, 230);
                    break;
                default:
                    HasShadow = true;
                    break;
            }
            second_col = grid.ColumnDefinitions[1];
        }

    }
}