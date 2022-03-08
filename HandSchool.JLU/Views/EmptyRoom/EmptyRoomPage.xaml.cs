using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.Views;
using Xamarin.Forms;
using HandSchool.Internals;
using HandSchool.JLU.InfoQuery;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmptyRoomPage : ViewObject
    {
        private readonly EmptyRoomViewModel _viewModel;

        public EmptyRoomPage()
        {
            InitializeComponent();
            ViewModel = _viewModel = EmptyRoomViewModel.Instance;
            _viewModel.Clear();
            InitClassesPicker();
            var now = DateTime.Now;
            DatePicker.MinimumDate = now;
            DatePicker.MaximumDate = DatePicker.MinimumDate.AddDays(30);
            DatePicker.Date = DatePicker.MinimumDate;
            Task.Run(LoadSchoolArea);
            SchoolAreaPicker.SelectedIndexChanged += (s, e) => LoadBuilding();
        }

        private void InitClassesPicker()
        {
            StartSection.ItemsSource = new ObservableCollection<int>();
            EndSection.ItemsSource = new ObservableCollection<int>();
            for (var i = 1; i <= Core.App.DailyClassCount; i++)
            {
                StartSection.ItemsSource.Add(i);
            }

            StartSection.SelectedIndexChanged += (sender, args) =>
            {
                var start = (Picker) sender;
                if (start.SelectedItem is null) return;
                if (start.SelectedItem is int startSec)
                {
                    Core.Platform.EnsureOnMainThread(() =>
                    {
                        var oldSelection = EndSection.SelectedItem as int? ?? -1;
                        EndSection.ItemsSource.Clear();
                        for (var i = startSec; i <= Core.App.DailyClassCount; i++)
                        {
                            EndSection.ItemsSource.Add(i);
                        }

                        if (oldSelection >= startSec)
                        {
                            EndSection.SelectedItem = oldSelection;
                        }
                        else
                        {
                            if (EndSection.ItemsSource.Count != 0)
                            {
                                EndSection.SelectedIndex = 0;
                            }
                        }
                    });
                }
            };
            if (StartSection.ItemsSource.Count != 0)
            {
                StartSection.SelectedIndex = 0;
            }
        }

        private async Task LoadSchoolArea()
        {
            try
            {
                var isVisible = !await _viewModel.GetSchoolAreaAsync();
                Core.Platform.EnsureOnMainThread(() =>
                {
                    RefreshSchoolArea.IsVisible = isVisible;
                    if ((SchoolAreaPicker.ItemsSource?.Count ?? 0) > 0)
                        SchoolAreaPicker.SelectedIndex = 0;
                });
            }
            catch
            {
                Core.Platform.EnsureOnMainThread(() => RefreshSchoolArea.IsVisible = true);
            }
        }

        private async void RefreshSchoolAreaAsync(object sender, EventArgs e)
        {
            await LoadSchoolArea();
        }

        private async Task LoadBuilding()
        {
            Core.Platform.EnsureOnMainThread(_viewModel.Building.Clear);
            if (!(SchoolAreaPicker.SelectedItem is string saPicker)) return;
            try
            {
                var isVisible = !await _viewModel.GetBuildingAsync(saPicker);
                Core.Platform.EnsureOnMainThread(() =>
                {
                    RefreshBuilding.IsVisible = isVisible;
                    if ((BuildingPicker.ItemsSource?.Count ?? 0) > 0)
                        BuildingPicker.SelectedIndex = 0;
                });
            }
            catch
            {
                Core.Platform.EnsureOnMainThread(() => RefreshBuilding.IsVisible = true);
            }
        }

        private async void RefreshBuildingAsync(object sender, EventArgs e)
        {
            await LoadBuilding();
        }

        private async void EmptyRoomQuery(object sender, EventArgs e)
        {
            var schoolArea = (string) SchoolAreaPicker.SelectedItem;
            var building = (string) BuildingPicker.SelectedItem;
            var start = (int?) StartSection.SelectedItem;
            var end = (int?) EndSection.SelectedItem;
            if (schoolArea is null)
            {
                await NoticeError("校区不能为空");
                return;
            }

            if (building is null)
            {
                await NoticeError("教学楼不能为空");
                return;
            }

            if (start is null)
            {
                await NoticeError("起始节不能为空");
                return;
            }

            if (end is null)
            {
                await NoticeError("结束节不能为空");
                return;
            }

            var res = await _viewModel.GetEmptyRoomAsync(DatePicker.Date, building, (int) start, (int) end);
            if (res)
            {
                await Navigation.PushAsync(typeof(EmptyRoomDetail), null);
            }
            else
            {
                await NoticeError("加载失败");
            }
        }

        private async void ClassroomCurriculumQuery(object sender, EventArgs e)
        {
            var schoolArea = (string) SchoolAreaPicker.SelectedItem;
            var building = (string) BuildingPicker.SelectedItem;
            if (schoolArea is null)
            {
                await NoticeError("校区不能为空");
                return;
            }

            if (building is null)
            {
                await NoticeError("教学楼不能为空");
                return;
            }

            var bdInfo = _viewModel.FindBuilding(building);
            if (bdInfo.compus is null || bdInfo.buildingId is null)
            {
                await NoticeError("服务器返回信息错误");
                return;
            }

            await Navigation.PushAsync<IWebViewPage>(new RoomSchedule((int) bdInfo.compus, (int) bdInfo.buildingId));
        }
    }

    [Entrance("JLU", "空教室及教室课程表查询", "没地方自习？试试这个吧。", EntranceType.InfoEntrance)]
    public class EmptyRoomPageShell : ITapEntrace
    {
        public Task Action(INavigate navigate)
        {
            navigate.PushAsync(typeof(EmptyRoomPage), null);
            return Task.CompletedTask;
        }
    }
}