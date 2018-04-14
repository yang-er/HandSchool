using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace HandSchool.UWP
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
            return new SolidColorBrush(Internal.Helper.ScheduleColors2[ColorId % 8]);
        }
    }
}
