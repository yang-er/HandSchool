#nullable enable
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Controls
{
    public enum TextAtomAfterTitlePosition
    {
        AfterTitle,
        UnderTitle
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextAtom
    {
        private Label? _secondTitleLabel;

        private Label? _secondContentLabel;

        private ColumnDefinition? _secondCol;

        private Label CreateSecondTitle()
        {
            return new Label
            {
                Text = SecondTitle,
                TextColor = SecondTitleColor,
                FontSize = 21.18,
                HorizontalOptions = LayoutOptions.End
            };
        }

        private Label CreateSecondContent()
        {
            return new Label
            {
                Text = SecondContent,
                FontSize = 13.18,
                TextColor = SecondContentColor,
                HorizontalOptions = LayoutOptions.End
            };
        }

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

        public TextAtomAfterTitlePosition AfterTitlePosition
        {
            get => (TextAtomAfterTitlePosition) GetValue(AfterTitlePositionProperty);
            set => SetValue(AfterTitlePositionProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            switch (propertyName)
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
                    if (_secondContentLabel is null) return;
                    _secondContentLabel.Text = SecondContent;
                    break;
                
                case nameof(SecondTitle):
                    if (_secondTitleLabel is null) return;
                    _secondTitleLabel.Text = SecondTitle;
                    break;
                
                case nameof(SecondContentColor):
                    if (_secondContentLabel is null) return;
                    _secondContentLabel.TextColor = SecondContentColor;
                    break;
                
                case nameof(SecondTitleColor):
                    if (_secondTitleLabel is null) return;
                    _secondTitleLabel.TextColor = SecondTitleColor;
                    break;
                
                case nameof(AfterTitlePosition):
                {
                    if (AfterTitlePosition == TextAtomAfterTitlePosition.UnderTitle)
                    {
                        MainGrid.Children.Remove(TitleBound);
                        if (ReferenceEquals(AfterTitleLabel.Parent, TitleBound))
                        {
                            TitleBound.Children.Remove(AfterTitleLabel);
                            var s = new StackLayout {Children = {TitleBound, AfterTitleLabel}, Spacing = 0};
                            MainGrid.Children.Add(s, 0, 0);
                        }
                    }
                    else
                    {
                        if (Parent.Parent is StackLayout outBound)
                        {
                            outBound.Children.Clear();
                            MainGrid.Children.Remove(outBound);
                            TitleBound.Children.Add(AfterTitleLabel);
                            MainGrid.Children.Add(TitleBound, 0, 0);
                        }
                    }

                    break;
                }
                
                case nameof(HasSecond):
                {
                    if (HasSecond)
                    {
                        _secondCol = new ColumnDefinition {Width = GridLength.Star};
                        MainGrid.ColumnDefinitions.Add(_secondCol);
                        _secondTitleLabel = CreateSecondTitle();
                        MainGrid.Children.Add(_secondTitleLabel, 1, 0);
                        _secondContentLabel = CreateSecondContent();
                        MainGrid.Children.Add(_secondContentLabel, 1, 1);
                    }
                    else
                    {
                        var children = MainGrid.Children;
                        children.Remove(_secondTitleLabel);
                        children.Remove(_secondContentLabel);
                        MainGrid.ColumnDefinitions.Remove(_secondCol);
                        _secondTitleLabel = _secondContentLabel = null;
                        _secondCol = null;
                    }

                    break;
                }
                
                case nameof(FirstProportion):
                    FirstCol.Width = new GridLength(FirstProportion, GridUnitType.Star);
                    break;
            }

            base.OnPropertyChanged(propertyName);
        }

        public static readonly BindableProperty AfterTitlePositionProperty =
            BindableProperty.Create(
                propertyName: nameof(AfterTitlePosition),
                returnType: typeof(TextAtomAfterTitlePosition),
                declaringType: typeof(TextAtom),
                defaultValue: TextAtomAfterTitlePosition.AfterTitle);

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
                defaultValue: false);

        public TextAtom()
        {
            InitializeComponent();
            TitleLabel.FontSize = 17.18;
            ContentTextLabel.FontSize = 15.18;
            AfterTitleLabel.FontSize = 13.18;
        }
    }
}