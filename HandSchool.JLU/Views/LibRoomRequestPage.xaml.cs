using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HandSchool.Internals;
using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    public class StudentLibBasicInfo : NotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _tips;
        public string Tips
        {
            get => _tips;
            set => SetProperty(ref _tips, value);
        }

        private string _innerId;
        public string InnerId
        {
            get => _innerId;
            set => SetProperty(ref _innerId, value);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is StudentLibBasicInfo info)
            {
                return _tips == info._tips;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Tips.GetHashCode();
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LibRoomRequestPage : ViewObject
    {
        private LibRoomRequestParams _params;
        private LibRoomReservationViewModel _viewModel;
        public LibRoomRequestPage()
        {
            InitializeComponent();
            ViewModel = _viewModel = LibRoomReservationViewModel.Instance;
            _viewModel.Recommends.Clear();
        }

        private async void SchoolCardIdChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Trim().Length < 11)
            {
                if (_viewModel.Recommends.Count != 0)
                {
                    Core.Platform.EnsureOnMainThread(() => _viewModel.Recommends.Clear());
                }
                return;
            }

            if (IsBusy) return;
            IsBusy = true;
            var res = await Loader.LibRoom.GetUserInfoAsync(e.NewTextValue.Trim());
            if (!res.IsSuccess) await NoticeError("查询用户信息失败");
            Core.Platform.EnsureOnMainThread(() =>
            {
                _viewModel.Recommends.Clear();
                if (!(res.Msg is IEnumerable<StudentLibBasicInfo> list)) return;
                foreach (var info in list)
                {
                    _viewModel.Recommends.Add(info);
                }
            });
            IsBusy = false;
        }

        private void InitStartPicker()
        {
            if (_params is null) return;
            var start = _params.TimeSlot.Start;
            if (start.Min % 5 != 0)
            {
                var delta = 5 - start.Min % 5;
                start += delta;
            }

            var end = _params.TimeSlot.End;
            if (end.Min % 5 != 0)
            {
                var delta = end.Min % 5;
                end -= delta;
            }
            
            end -= _params.LibRoom.MinMins ?? 0;
            StartTimePicker.Items.Clear();
            for (var i = start; i.CompareTo(end) <= 0; i += 5)
            {
                StartTimePicker.Items.Add(i.ToString());
            }

            if (StartTimePicker.Items.Count == 0) return;
            StartTimePicker.SelectedIndex = 0;
        }

        private void InitEndPicker(object sender = null, EventArgs e = null)
        {
            if (_params is null) return;
            var end = _params.TimeSlot.End;
            if (end.Min % 5 != 0)
            {
                var delta = end.Min % 5;
                end -= delta;
            }

            var start = new Time(StartTimePicker.SelectedItem.ToString());
            var maxEnd = start + (_params.LibRoom.MaxMins ?? 0);
            if (maxEnd.CompareTo(end) < 0) end = maxEnd;
            start += _params.LibRoom.MinMins ?? 0;
            EndTimePicker.Items.Clear();
            for (var i = start; i.CompareTo(end) <= 0; i += 5)
            {
                EndTimePicker.Items.Add(i.ToString());
            }
            if (StartTimePicker.Items.Count == 0) return;
            EndTimePicker.SelectedIndex = 0;
        }

        public override void SetNavigationArguments(object param)
        {
            base.SetNavigationArguments(param);
            _params = param as LibRoomRequestParams;
            TitleLabel.Text = "预约" + _params?.LibRoom.Name;
            StartTimePicker.SelectedIndexChanged += InitEndPicker;
            InitStartPicker();
        }
        
        private void NameSelected(object sender, EventArgs e)
        {
            var frame = (Frame) sender;
            var label = frame.Content as Label;
            var info = label?.BindingContext as StudentLibBasicInfo;
            SchoolCardNum.Text = "";
            _viewModel.Recommends.Clear();
            if (string.IsNullOrWhiteSpace(info?.Name)) return;
            if (_viewModel.Selected.Contains(info)) return;
            _viewModel.Selected.Add(info);
        }

        private async void DelUser(object sender, EventArgs e)
        {
            if (IsBusy) return;
            IsBusy = true;
            var info = (sender as BindableObject)?.BindingContext as StudentLibBasicInfo;
            if (info is null) return;
            var res = await RequestAnswerAsync("取消选定", info.Tips, "否", "是");
            IsBusy = false;
            if (!res) return;
            Core.Platform.EnsureOnMainThread(() => _viewModel.Selected.Remove(info));
        }

        private async void SelectedOk(object sender, EventArgs e)
        {
            var start = DateTime.Parse(StartTimePicker.SelectedItem as string);
            var end =  DateTime.Parse(EndTimePicker.SelectedItem as string);
            var res = await _viewModel.StartResvAsync(_params.LibRoom, _params.Date, start, end);
            if (!res.IsSuccess) return;
            Core.Platform.EnsureOnMainThread(_viewModel.Selected.Clear);
            await PopAsync();
            await _params.ResultPage.Refresh();
            _viewModel.RefreshInfosCommand.Execute(null);
        }

        public async Task PushAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(this);
        }

        public static async Task PopAsync()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
        private async void ReSelectOnClicked(object sender, EventArgs e)
        {
            await PopAsync();
        }
    }
}