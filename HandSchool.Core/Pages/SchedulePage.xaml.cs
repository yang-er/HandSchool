using HandSchool.Internals;
using HandSchool.ViewModels;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SchedulePage : ViewObject
	{
        private readonly ScrollView scrollView;
        private readonly Grid scheduleGrid;
        private readonly RowDefinition defRow;
        private readonly ColumnDefinition defCol;
        private readonly GridLength rowHeight;
        private readonly GridLength colWidth;

        private bool isWider = false;
        private bool forceSize = true;
        private bool invalidated = true;
        private int lastWeek = -666;
        
        public ScheduleViewModelBase SchedViewModel
        {
            get => (ScheduleViewModelBase)ViewModel;
            set => ViewModel = value;
        }
        
        public SchedulePage()
        {
            InitializeComponent();
            scrollView = Content as ScrollView;
            scheduleGrid = scrollView.Content as Grid;
            
            colWidth = new GridLength(55, GridUnitType.Absolute);
            rowHeight = new GridLength(90, GridUnitType.Absolute);

            defCol = new ColumnDefinition { Width = GridLength.Star };
            defRow = new RowDefinition { Height = rowHeight };
            
            for (int day = 1; day <= 7; day++)
            {
                scheduleGrid.ColumnDefinitions.Add(defCol);
            }
            
            for (int cls = 1; cls <= Core.App.DailyClassCount; cls++)
            {
                scheduleGrid.RowDefinitions.Add(defRow);
                scheduleGrid.Children.Add(new Label()
                {
                    Text = cls.ToString(),
                    TextColor = Color.Gray
                }, 0, cls);
            }
            
            isWider = false;
            forceSize = true;
        }

        public override void SetNavigationArguments(object param)
        {
            if (param is null) param = ScheduleViewModel.Instance;
            SchedViewModel = (ScheduleViewModelBase)param;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (SchedViewModel is ScheduleViewModel svm)
            {
                svm.RefreshComplete += LoadList;
            }

            if (invalidated || lastWeek != SchedViewModel.Week)
            {
                LoadList();
                this.WriteLog("OnAppearing. Redrawing class table");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (SchedViewModel is ScheduleViewModel svm)
            {
                svm.RefreshComplete -= LoadList;
            }
        }
        
        public void LoadList()
        {
            invalidated = false;
            lastWeek = SchedViewModel.Week;
            scheduleGrid.BatchBegin();

            for (int i = scheduleGrid.Children.Count;
                i > 7 + Core.App.DailyClassCount; i--)
            {
                scheduleGrid.Children.RemoveAt(i - 1);
            }

            // Render classes
            SchedViewModel.RenderWeek(SchedViewModel.Week, out var list);

            int count = 0;
            foreach (var item in list)
            {
                scheduleGrid.Children.Add(new CurriculumLabel(item, count++));
            }

            scheduleGrid.BatchCommit();
        }

        private void SetTileSize(object sender, EventArgs e)
        {
            if (Core.Platform.Idiom != TargetIdiom.Phone)
            {
                if (forceSize)
                {
                    forceSize = false;
                    defRow.Height = GridLength.Star;
                    defCol.Width = GridLength.Star;
                    scrollView.Orientation = ScrollOrientation.Vertical;
                    UseSafeArea = true;
                }
            }
            else if (scrollView.Width > scrollView.Height && (!isWider || forceSize))
            {
                forceSize = false;
                isWider = true;
                defRow.Height = new GridLength(60, GridUnitType.Absolute);
                defCol.Width = GridLength.Star;
                scrollView.Orientation = ScrollOrientation.Vertical;
                UseSafeArea = true;
            }
            else if (scrollView.Width < scrollView.Height && (isWider || forceSize))
            {
                forceSize = false;
                isWider = false;
                defRow.Height = rowHeight;
                defCol.Width = GridLength.Star;
                scrollView.Orientation = ScrollOrientation.Vertical;
                UseSafeArea = true;
            }
        }

        async void iOS_Menu_Clicked(object sender, EventArgs e)
        {
            if (sender == null) return;
            var names = new string[] { "刷新课表", "添加课程", "查看任意周" };
            var vm = BindingContext as ScheduleViewModel;
            var commands = new ICommand[] { vm.RefreshCommand, vm.AddCommand, vm.ChangeWeekCommand };
            var resp = await RequestActionAsync("更多", "取消", null, names);
            if (string.IsNullOrWhiteSpace(resp)) return;
            for(int i = 0; i < names.Length; i++)
            {
                if (resp.Contains(names[i]))
                {
                    commands[i].Execute(null);
                    return;
                }
            }
        }
    }
}