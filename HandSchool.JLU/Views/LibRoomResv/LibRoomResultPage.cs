using System;
using System.Threading.Tasks;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HandSchool.JLU.Views.LibRoomResv;
using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool.JLU.Views
{
    public class LibRoomResultPage : ViewObject
    {
        public const string RequestFinishedSignal = "HandSchool.JlU.Views.LibRoomResultPage.RequestFinished";

        public static Time StartTime { get; } = new Time("6:30");
        public static Time EndTime { get; } = new Time("21:30");

        private readonly TimeLineTable _mainStack;
        
        public LibRoomResultPage()
        {
            Content = _mainStack = new TimeLineTable(StartTime, EndTime);
            Now = new Time(DateTime.Now);
        }

        private LibRoomResultPageParams Params { get; set; }
        
        private void InitTimeLine()
        {
            foreach (var item in Params.Rooms)
            {
                //添加标题
                var line = _mainStack.CreateAndAddLine(item.Name);
                line.BindingContext = item;
                line.FreeTimeClicked += StartResvRoom;
                line.BusyTimeClicked += ShowResvInfo;
                #region 处理一些特殊情况

                //如果这个房间不可用，全线置为不可用
                if (item.State == LibRoom.LibRoomState.Close)
                {
                    line.AddNotAvailTime(new TimeSlot{Start = StartTime, End = EndTime});
                    continue;
                }

                //如果这个房间没有预约数据
                if (item.Times is null || item.Times.Count == 0)
                {
                    if (Params.Date == NearDays.Tomorrow) //若是明天的预约，全线都是空闲
                    {
                        continue;
                    }

                    if (Now.CompareTo(EndTime) > 0) //现在晚于关闭时间，全线置为过时
                    {
                        line.AddTimeout(new TimeSlot{Start = StartTime, End = EndTime});
                        continue;
                    }

                    if (Now.CompareTo(StartTime) < 0) //现在时间早于起始时间，全线都是空闲
                    {
                        continue;
                    }

                    //现在时间在开放时间内，时间线被过时和空闲两种状态分别占有
                    line.AddTimeout(new TimeSlot{Start = StartTime, End = Now});
                    continue;
                }

                #endregion

                #region 绘制第一个预约之前的时间线

                //如果是明天，第一个预约之前全是空闲时间
                if (Params.Date != NearDays.Tomorrow)
                {
                    if (Now.CompareTo(item.Times[0].TimeSlot.Start) < 0) //现在时间早于第一个预约的开始时间
                    {
                        //现在时间早于开始时间，那么从开馆到第一个预约全是空闲时间
                        if (Now.CompareTo(StartTime) >= 0)
                        {
                            //如果现在晚于开馆时间，那么开馆到第一个预约之间被过时和空闲分占
                            line.AddTimeout(new TimeSlot {Start = StartTime, End = Now});
                        }
                    }
                    else
                    {
                        //现在时间在第一个时间之后，实际上一定在第一个预约之中
                        line.AddTimeout(new TimeSlot {Start = StartTime, End = item.Times[0].TimeSlot.Start});
                    }
                }

                #endregion

                #region 绘制第一个预约到最后一个预约的时间线

                foreach (var t in item.Times)
                {
                    line.AddBusyTime(new TimeSlot{Start = t.TimeSlot.Start, End = t.TimeSlot.End}, t.UserInfo);
                }

                #endregion
            }
        }

        private bool _isPushing;
        
        private async void StartResvRoom(object sender, EventArgs e)
        {
            if (_isPushing) return;
            _isPushing = true;
            var timeLine = sender as TimeLine.TimeSpanFrame;
            if (!(timeLine?.BindingContext is LibRoom libRoom)) return;
            if (timeLine.MinuteSpan < libRoom.MinMins)
            {
                await NoticeError($"该时间段小于{libRoom.MinMins}分钟");
                _isPushing = false;
                return;
            }
            
            await Navigation.PushAsync(typeof(LibRoomRequestPage), new LibRoomRequestParams
            {
                TimeSlot = timeLine.TimeSlot.Clone() as TimeSlot,
                LibRoom = libRoom,
                Date = Params.Date
            });
            _isPushing = false;
        }

        private async void ShowResvInfo(object sender, EventArgs e)
        {
            if (sender is TimeLine.TimeSpanFrame timeLine)
            {
                await RequestMessageAsync("预约信息",
                    $"{timeLine.TaskName}\n开始时间：{timeLine.TimeSlot.Start}\n结束时间：{timeLine.TimeSlot.End}", "彳亍");
            }
        }
        private async void OnRequestFinished(LibRoomRequestPage p)
        {
            await Navigation.PopAsync();
        }
        private Time Now { get; set; }

        public override void SetNavigationArguments(object param)
        {
            base.SetNavigationArguments(param);
            Params = param as LibRoomResultPageParams;
            if (Params is null) return;
            Title = Params.Title;
            InitTimeLine();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<LibRoomRequestPage>(this, RequestFinishedSignal);
            _mainStack.ClearView();
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<LibRoomRequestPage>(this, RequestFinishedSignal, OnRequestFinished);
        }

        public async Task Refresh()
        {
            var res = await LibRoomReservationViewModel.Instance.GetRoomAsync(Params.GetRoomUsageParams);
            Params = res.Msg as LibRoomResultPageParams;

            Core.Platform.EnsureOnMainThread(() =>
            {
                _mainStack.ClearView();
                Now = new Time(DateTime.Now);
                InitTimeLine();
            });
        }
    }
}
