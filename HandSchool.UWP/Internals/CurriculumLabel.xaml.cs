using HandSchool.Models;
using System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace HandSchool.Views
{
    public sealed partial class CurriculumLabel : UserControl
    {
        public CurriculumItemBase Context { get; }
        public int ColorId { get; private set; }

        public CurriculumLabel(CurriculumItemBase value, int id)
        {
            InitializeComponent();
            Context = value;
            ColorId = id;
            Update();
            
            if (Context is CurriculumItem)
            {
                IsDoubleTapEnabled = true;
                DoubleTapped += OnDoubleTapped;
            }
        }

        public void Update()
        {
            StackLayout.Children.Clear();

            foreach (var descr in Context.ToDescription())
            {
                if (StackLayout.Children.Count > 0)
                {
                    StackLayout.Children.Add(new TextBlock
                    {
                        FontSize = 16,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    });
                }

                StackLayout.Children.Add(new TextBlock
                {
                    Text = descr.Title,
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.SemiBold,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });

                StackLayout.Children.Add(new TextBlock
                {
                    Text = descr.Description,
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.Normal,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });
            }

            Grid.SetColumn(this, Context.WeekDay);
            Grid.SetRow(this, Context.DayBegin);
            Grid.SetRowSpan(this, Context.DayEnd - Context.DayBegin + 1);
        }

        private async void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
        {
            args.Handled = true;
            var dialog = new CurriculumDialog(Context as CurriculumItem);
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Update();
            }
            else if (result == ContentDialogResult.Secondary)
            {
                (Parent as Grid).Children.Remove(this);
            }
        }
        
        public Brush GetColor()
        {
            // thanks to brady
            return new SolidColorBrush(ScheduleColors[ColorId % 8]);
        }

        static readonly Color[] ScheduleColors = {
                Color.FromArgb(0xff, 0x59, 0xe0, 0x9e),
                Color.FromArgb(0xff, 0xf4, 0x8f, 0xb1),
                Color.FromArgb(0xff, 0xce, 0x93, 0xd8),
                Color.FromArgb(0xff, 0xff, 0x8a, 0x65),
                Color.FromArgb(0xff, 0x9f, 0xa8, 0xda),
                Color.FromArgb(0xff, 0x42, 0xa5, 0xf5),
                Color.FromArgb(0xff, 0x80, 0xde, 0xea),
                Color.FromArgb(0xff, 0xc6, 0xde, 0x7c)
            };
    }
}
