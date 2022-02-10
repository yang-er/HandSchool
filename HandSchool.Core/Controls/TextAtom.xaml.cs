using HandSchool.Internal;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextAtom
    {
        readonly ColumnDefinition _secondCol;

        public bool OnTop
        {
            get => (bool) GetValue(OnTopProperty);
            set => SetValue(OnTopProperty, value);
        }

        public double FirstProportion
        {
            get => (double) GetValue(FirstProportionProperty);
            set => SetValue(FirstProportionProperty, value);
        }

        public bool HasSecond
        {
            get => (bool) GetValue(HasSecondProperty);
            set => SetValue(HasSecondProperty, value);
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string AfterTitle
        {
            get => (string) GetValue(AfterTitleProperty);
            set => SetValue(AfterTitleProperty, value);
        }

        public string ContentText
        {
            get => (string) GetValue(ContentTextProperty);
            set => SetValue(ContentTextProperty, value);
        }

        public string SecondTitle
        {
            get => (string) GetValue(SecondTitleProperty);
            set => SetValue(SecondTitleProperty, value);
        }

        public string SecondContent
        {
            get => (string) GetValue(SecondContentProperty);
            set => SetValue(SecondContentProperty, value);
        }

        public Color SecondContentColor
        {
            get => (Color) GetValue(SecondContentColorProperty);
            set => SetValue(SecondContentColorProperty, value);
        }

        public Color SecondTitleColor
        {
            get => (Color) GetValue(SecondTitleColorProperty);
            set => SetValue(SecondTitleColorProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string n = null)
        {
            if (string.IsNullOrEmpty(n)) return;
            {
                switch (n)
                {
                    case nameof(OnTop):
                        OnTopIcon.IsVisible = OnTop;
                        break;
                    case nameof(Title):
                        TitleLabel.Text = Title;
                        break;
                    case nameof(ContentText):
                        ContentTextLabel.Text = ContentText;
                        break;
                    case nameof(AfterTitle):
                        AfterTitleLabel.Text = AfterTitle;
                        break;
                    case nameof(SecondContent):
                        SecondContentLabel.Text = SecondContent;
                        break;
                    case nameof(SecondTitle):
                        SecondTitleLabel.Text = SecondTitle;
                        break;
                    case nameof(SecondContentColor):
                        SecondContentLabel.TextColor = SecondContentColor;
                        break;
                    case nameof(SecondTitleColor):
                        SecondTitleLabel.TextColor = SecondTitleColor;
                        break;
                    case nameof(HasSecond):
                        if (HasSecond)
                        {
                            MainGrid.ColumnDefinitions.Add(_secondCol);
                            MainGrid.ColumnDefinitions[0].Width =
                                new GridLength(FirstProportion, GridUnitType.Star);
                            MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                        }

                        SecondTitleLabel.IsVisible = SecondContentLabel.IsVisible = HasSecond;
                        if (!HasSecond)
                        {
                            MainGrid.RowDefinitions.RemoveAt(1);
                            MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                        }

                        break;
                    case nameof(FirstProportion):
                        MainGrid.ColumnDefinitions[0].Width = new GridLength(FirstProportion, GridUnitType.Star);
                        break;
                    default:
                        base.OnPropertyChanged(n);
                        break;
                }
            }
        }

        public static readonly BindableProperty OnTopProperty =
            BindableProperty.Create(
                propertyName: nameof(OnTop),
                returnType: typeof(bool),
                declaringType: typeof(TextAtom),
                defaultValue: false);

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
            _secondCol = MainGrid.ColumnDefinitions[1];
        }
    }
}