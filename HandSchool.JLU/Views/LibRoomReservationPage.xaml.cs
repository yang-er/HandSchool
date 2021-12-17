﻿using System;
using System.Linq;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    public class ScoreLabel : StackLayout
    {
        private readonly Label _title;
        private readonly Label _desc;

        public string Title
        {
            get => _title.Text;
            set => _title.Text = value;
        }

        public string Tips
        {
            get => _desc.Text;
            set => _desc.Text = value;
        }
        public ScoreLabel()
        {
            Orientation = StackOrientation.Horizontal;
            _title = new Label {TextColor = Color.DarkBlue};
            _title.FontSize = Device.GetNamedSize(NamedSize.Medium, _title);
            _desc = new Label {TextColor = Color.Black, VerticalTextAlignment = TextAlignment.Center};
            Children.Add(_title);
            Children.Add(_desc);
        }
    }
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LibRoomReservationPage : ViewObject
    {
        private LibRoomReservationViewModel _viewModel;


        public LibRoomReservationPage()
        {
            InitializeComponent();
            Loader.LibRoom.LoginStateChanged += (s, e) =>
            {
                if (e.State == LoginState.Succeeded)
                {
                    RefreshScoreStack();
                }
            };
            ViewModel = _viewModel = LibRoomReservationViewModel.Instance;
            RefreshScoreStack();

            IrregularitiesView.IndicatorView = IrregularitiesIndicator;
            ReservationRecordsView.IndicatorView = ReservationRecordsIndicator;
        }

        private void ClearScoreStack()
        {
            var scoreLabels = InfoStack.Children.Where(v => v is ScoreLabel).ToList();
            foreach (var item in scoreLabels)
            {
                InfoStack.Children.Remove(item);
            }
        }
        private void RefreshScoreStack()
        {
            Core.Platform.EnsureOnMainThread(ClearScoreStack);
            var credits = _viewModel?.Credits;
            if (credits is null) return;
            var list = credits.Select(item => new ScoreLabel {Title = item[0], Tips = $"{item[1]} / {item[2]}"})
                .ToList();

            Core.Platform.EnsureOnMainThread(() =>
                {
                    foreach (var item in list)
                    {
                        InfoStack.Children.Add(item);
                    }
                }
            );
        }

        private bool _isSending = false;
        private async void ResvClicked(object sender, EventArgs e)
        {
            if (_isSending) return;
            _isSending = true;
            NearDays date;
            if (ReferenceEquals(sender, _3to6TodayButton) || ReferenceEquals(sender, _5to10TodayButton))
                date = NearDays.Today;
            else date = NearDays.Tomorrow;
            int type;
            if (ReferenceEquals(sender, _3to6TodayButton) || ReferenceEquals(sender, _3to6TomButton))
            {
                type = 0;
            }
            else
            {
                type = 1;
            }

            var par = new GetRoomUsageParams
            {
                Date = date, RoomType = type
            };
            var res = await _viewModel.GetRoomAsync(par);
            if (!res.IsSuccess)
            {
                _isSending = false;
                if (res.Msg is null) return;
                await NoticeError(res.ToString());
            }
            else
            {
                await Navigation.PushAsync(typeof(LibRoomResultPage), res.Msg);
            }
            _isSending = false;
        }

        private async void CancelResv(object sender, EventArgs e)
        {
            var info = (sender as BindableObject)?.BindingContext as ReservationInfo;
            if (info?.ResvInnerId is null) return;
            if (!await RequestAnswerAsync("确认", "取消此次预约？", "否", "是"))
            {
                return;
            }

            var res = await _viewModel.CancelResvAsync(info.ResvInnerId);
            if (res.IsSuccess)
            {
                RefreshUserInfo();
                await RequestMessageAsync("提示","成功取消", "彳亍");
            }
            else
            {
                if (res.Msg is null) return;
                await NoticeError(res.ToString());
            }
        }

        private async void RefreshUserInfo(object sender = null, EventArgs e = null)
        {
            var res = await _viewModel.RefreshInfosAsync();
            if (res.IsSuccess) return;
            if (res.Msg is null) return;
            await NoticeError(res.ToString());
        }
        private void ClearUserInfo(object sender, EventArgs e)
        {
            _viewModel.ClearUserInfo();
            Core.Platform.EnsureOnMainThread(ClearScoreStack);
            RefreshUserInfo();
        }
    }
}