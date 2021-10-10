using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.Views;
using Xamarin.Forms;
using HandSchool.Internals;
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
                        EndSection.ItemsSource.Clear();
                        for (var i = startSec; i <= Core.App.DailyClassCount; i++)
                        {
                            EndSection.ItemsSource.Add(i);
                        }
                    });
                }
            };
            SchoolAreaPicker.SelectedIndexChanged += (s, e) =>
            {
                Core.Platform.EnsureOnMainThread(() =>
                {
                    BuildingPicker.SelectedItem = null;
                    BuildingPicker.ItemsSource.Clear();
                    Buildings.IsVisible = false;
                });
            };
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

        private void Start(object sender, EventArgs e)
        {
            Task.Run(async () => await _viewModel.GetEmptyRoomAsync((string) SchoolAreaPicker.SelectedItem,
                (string) BuildingPicker.SelectedItem, (int?) StartSection.SelectedItem, (int?) EndSection.SelectedItem));
        }
    }

    [Entrance("JLU", "查空教室", "没地方自习？试试这个吧。", EntranceType.InfoEntrance)]
    public class EmptyRoomPageShell : ITapEntrace
    {
        public Task Action(INavigate navigate)
        {
            navigate.PushAsync(typeof(EmptyRoomPage), null);
            return Task.CompletedTask;
        }
    }
}