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
        private EmptyRoomViewModel _viewModel;
        public EmptyRoomPage()
        {
            InitializeComponent();
            ViewModel = _viewModel = EmptyRoomViewModel.Instance;
            _viewModel.Clear();
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
            SchoolAreaPicker.SelectedIndexChanged += (s, e) =>
            {
                Core.Platform.EnsureOnMainThread(() =>
                {
                    BuildingPicker.SelectedItem = null;
                    BuildingPicker.ItemsSource.Clear();
                    Buildings.IsVisible = false;
                });
            };
            var now = DateTime.Now;
            DatePicker.MinimumDate = now;
            DatePicker.MaximumDate = DatePicker.MinimumDate.AddDays(30);
            DatePicker.Date = DatePicker.MinimumDate;
            Task.Run(LoadSchoolArea);
        }

        private static async Task HandleLoading(Picker picker, Label msg, Button refresh, Task<bool> success)
        {
            Core.Platform.EnsureOnMainThread(() =>
            {
                picker.IsVisible = false;
                msg.IsVisible = true;
                msg.Text = "信息正在加载...";
                refresh.IsVisible = false;
            });
            await success.ContinueWith(async (task) =>
            {
                var res = await task;
                Core.Platform.EnsureOnMainThread(() =>
                {
                    if (res)
                    {
                        msg.IsVisible = false;
                        picker.IsVisible = true;
                        refresh.IsVisible = false;
                    }
                    else
                    {
                        msg.Text = "信息加载失败";
                        refresh.IsVisible = true;
                        picker.SelectedItem = null;
                    }
                });
            });
        }

        private async Task LoadSchoolArea()
        {
            await _viewModel.GetSchoolAreaAsync()
                .ContinueWith(async x => await HandleLoading(SchoolAreaPicker, SchoolAreaLoadingText, SchoolAreaReload, x));
        }

        private async Task LoadBuilding(string schoolArea)
        {
            await _viewModel.GetBuildingAsync(schoolArea)
                .ContinueWith(async task => await HandleLoading(BuildingPicker, BuildingsLoadingText, BuildingReload, task));
        }
        private void SchoolAreaSelectOk(object sender, EventArgs e)
        {
            if (!(SchoolAreaPicker.SelectedItem is string saPicker)) return;
            Buildings.IsVisible = true;
            Task.Run(async () => await LoadBuilding(saPicker));
        }
        private void ReloadBuilding(object sender, EventArgs e)
        {
            if (!(SchoolAreaPicker.SelectedItem is string saPicker)) return;
            Task.Run(async () => await LoadBuilding(saPicker));
        }

        private void ReloadSchoolArea(object sender, EventArgs e)
        {
            Task.Run(LoadSchoolArea);
        }

        private async void Start(object sender, EventArgs e)
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
            var res = await _viewModel.GetEmptyRoomAsync(DatePicker.Date, building,(int)start,(int)end);
            if (res)
            {
                await Navigation.PushAsync(typeof(EmptyRoomDetail), null);
            }
            else
            {
                await NoticeError("加载失败");
            }
        }

        private async void ClassRoomCurriculumQuery(object sender, EventArgs e)
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
            await Navigation.PushAsync<IWebViewPage>(new RoomSchedule((int)bdInfo.compus,(int)bdInfo.buildingId));
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