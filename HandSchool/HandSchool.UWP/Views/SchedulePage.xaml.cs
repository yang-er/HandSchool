using HandSchool.ViewModels;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.Views
{
    public sealed partial class SchedulePage : ViewPage
    {
        public int TileFontSize => 14;

        public SchedulePage()
        {
            InitializeComponent();

            ViewModel = ScheduleViewModel.Instance;

            var Brush = new SolidColorBrush(Colors.Gray);
            for (int ij = 1; ij <= Core.App.DailyClassCount; ij++)
            {
                Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                var label = new TextBlock { Text = ij.ToString(), FontSize = TileFontSize, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Foreground = Brush };
                Grid.SetRow(label, ij);
                Grid.Children.Add(label);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadList();
            ScheduleViewModel.Instance.RefreshComplete += LoadList;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ScheduleViewModel.Instance.RefreshComplete -= LoadList;
        }

        public void LoadList()
        {
            for (int i = Grid.Children.Count; i > 7 + Core.App.DailyClassCount; i--)
            {
                Grid.Children.RemoveAt(i - 1);
            }

            // Render classes
            Core.App.Schedule.RenderWeek(ScheduleViewModel.Instance.Week, out var list);
            int count = 0;
            foreach (var item in list)
                Grid.Children.Add(new CurriculumLabel(item, count++));
        }
    }
}
