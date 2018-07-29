using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using HandSchool.Models;

namespace HandSchool.UWP.Views
{
    public sealed partial class CurriculumLabel : UserControl
    {
        public CurriculumItem Context { get; }
        public int ColorId { get; private set; }

        public CurriculumLabel(CurriculumItem value, int id)
        {
            InitializeComponent();
            Context = value;
            DataContext = Context;
            ColorId = id;
            Update();
        }

        public void Update()
        {
            Grid.SetColumn(this, Context.WeekDay);
            Grid.SetRow(this, Context.DayBegin);
            Grid.SetRowSpan(this, Context.DayEnd - Context.DayBegin + 1);
        }

        private async void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
        {
            args.Handled = true;
            var dialog = new CurriculumDialog(Context);
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
