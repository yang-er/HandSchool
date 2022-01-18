using HandSchool.Internals;
using HandSchool.ViewModels;
using System;
using HandSchool.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SchedulePage : ViewObject
	{
        private readonly RowDefinition _defRow;
        private readonly ColumnDefinition _defCol;
        private readonly GridLength _rowHeight;
        private readonly GridLength _colWidth;

        private bool _isWider;
        private bool _forceSize = true;
        private bool _invalidated = true;
        private int _lastWeek = -666;
        private SchoolState _lastState = SchoolState.Normal;

        private ScheduleViewModelBase SchedViewModel
        {
            get => (ScheduleViewModelBase)ViewModel;
            set => ViewModel = value;
        }
        
        public SchedulePage()
        {
            InitializeComponent();
            
            _colWidth = new GridLength(55, GridUnitType.Absolute);
            _rowHeight = new GridLength(Device.RuntimePlatform == Device.iOS ? 100 : 90, GridUnitType.Absolute);

            _defCol = new ColumnDefinition { Width = GridLength.Star };
            _defRow = new RowDefinition { Height = _rowHeight };
            
            for (var day = 1; day <= 7; day++)
            {
                ScheduleGrid.ColumnDefinitions.Add(_defCol);
            }
            
            for (int cls = 1; cls <= Core.App.DailyClassCount; cls++)
            {
                ScheduleGrid.RowDefinitions.Add(_defRow);
                ScheduleGrid.Children.Add(new Label
                {
                    Text = cls.ToString(),
                    TextColor = Color.Gray
                }, 0, cls);
            }
            
            _isWider = false;
            _forceSize = true;
        }

        public override void SetNavigationArguments(object param)
        {
            param ??= ScheduleViewModel.Instance;
            SchedViewModel = (ScheduleViewModelBase)param;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (SchedViewModel is ScheduleViewModel svm)
            {
                //两次反转是因为如果当前页正忙时切出，加载条会强制隐藏，让属性改变来激活加载条
                svm.IsBusy = !svm.IsBusy;
                svm.IsBusy = !svm.IsBusy;
                svm.RefreshComplete += LoadList;
            }
            if (_invalidated || _lastWeek != SchedViewModel.Week || _lastState != SchedViewModel.SchoolState)
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
            _invalidated = false;
            _lastWeek = SchedViewModel.Week;
            _lastState = SchedViewModel.SchoolState;
            ScheduleGrid.BatchBegin();

            for (int i = ScheduleGrid.Children.Count;
                i > 7 + Core.App.DailyClassCount; i--)
            {
                ScheduleGrid.Children.RemoveAt(i - 1);
            }

            // Render classes
            SchedViewModel.RenderWeek(SchedViewModel.Week, SchedViewModel.SchoolState,out var list);

            var count = 0;
            foreach (var item in list)
            {
                ScheduleGrid.Children.Add(new CurriculumLabel(item, count++));
            }

            ScheduleGrid.BatchCommit();
        }

        private void SetTileSize(object sender, EventArgs e)
        {
            if (Core.Platform.Idiom != TargetIdiom.Phone)
            {
                if (_forceSize)
                {
                    _forceSize = false;
                    _defRow.Height = GridLength.Star;
                    _defCol.Width = GridLength.Star;
                    ScrollView.Orientation = ScrollOrientation.Vertical;
                    UseSafeArea = true;
                }
            }
            else if (ScrollView.Width > ScrollView.Height && (!_isWider || _forceSize))
            {
                _forceSize = false;
                _isWider = true;
                _defRow.Height = new GridLength(60, GridUnitType.Absolute);
                _defCol.Width = GridLength.Star;
                ScrollView.Orientation = ScrollOrientation.Vertical;
                UseSafeArea = true;
            }
            else if (ScrollView.Width < ScrollView.Height && (_isWider || _forceSize))
            {
                _forceSize = false;
                _isWider = false;
                _defRow.Height = _rowHeight;
                _defCol.Width = GridLength.Star;
                ScrollView.Orientation = ScrollOrientation.Vertical;
                UseSafeArea = true;
            }
        }

        private async void iOS_MenuClicked(object sender, EventArgs e)
        {
            if (sender == null) return;
            var names = new[] {"查看任意周", "刷新课程表", "添加课程"};
            if (BindingContext is ScheduleViewModel vm)
            {
                var commands = new[] {vm.ChangeWeekCommand, vm.RefreshCommand, vm.AddCommand};
                var resp = await RequestActionAsync("更多", "取消", null, names);
                if (string.IsNullOrWhiteSpace(resp)) return;
                for (var i = 0; i < names.Length; i++)
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
}