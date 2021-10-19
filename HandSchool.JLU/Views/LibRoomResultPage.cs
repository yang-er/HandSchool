using System;
using System.Threading.Tasks;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LibRoomResultPage : ViewObject
    {
        public static Time StartTime { get; } = new Time("6:30");
        public static Time EndTime { get; } = new Time("21:30");

        private Grid _timeTable;
        private StackLayout _timeLineStack;

        private void InitTable()
        {
            var mainStack = new StackLayout {Orientation = StackOrientation.Horizontal};
            var scroller = new ScrollView {Orientation = ScrollOrientation.Horizontal};
            var timeLineStack = new StackLayout {Orientation = StackOrientation.Horizontal};
            var timeTable = new Grid {RowSpacing = 0};
            timeLineStack.LayoutChanged += OnSizeChanged;
            scroller.Content = timeLineStack;
            mainStack.Children.Add(timeTable);
            mainStack.Children.Add(scroller);
            _timeTable = timeTable;
            _timeLineStack = timeLineStack;
            Content = mainStack;
            
            _timeTable.ColumnDefinitions.Add(new ColumnDefinition {Width = 40});
            _timeTable.RowDefinitions.Add(new RowDefinition {Height = 20});
            for (var i = 0; i < 16; i++)
            {
                _timeTable.RowDefinitions.Add(_row);
            }

            InitTimeLabel(6, 21, 1);
        }

        private LibRoomResultPageParams Params { get; set; }
        private readonly RowDefinition _row = new RowDefinition {Height = GridLength.Star};

        private void ClearView()
        {
            if (!(_timeTable is null))
            {
                _timeTable.Children.Clear();
                _timeTable.RowDefinitions.Clear();
                _timeTable.ColumnDefinitions.Clear();
            }

            if (!(_timeLineStack is null))
            {
                _timeLineStack.Children.Clear();
            }
            _timeTable = null;
            _timeLineStack = null;
            Content = null;
        }

        private void InitTimeLabel(int start, int end, int startRow)
        {
            var curRow = startRow;
            for (var i = start; i <= end; i++)
            {
                var oClock = new Label
                    {Text = $"{i}:30", TextColor = Color.Gray, HorizontalOptions = LayoutOptions.CenterAndExpand};
                _timeTable.Children.Add(oClock);
                Grid.SetColumn(oClock, 0);
                Grid.SetRow(oClock, curRow);
                curRow += 1;
            }
        }

        private void InitTimeLine()
        {
            foreach (var item in Params.Rooms)
            {
                //添加标题
                var stack = new StackLayout {Orientation = StackOrientation.Vertical, Spacing = 0, WidthRequest = 50};
                var label = new Label
                {
                    Text = item.Name, TextColor = Color.Gray, HorizontalOptions = LayoutOptions.Center,
                    HeightRequest = 20
                };
                stack.Children.Add(label);
                _timeLineStack.Children.Add(stack);

                TimeLine usage;
                int weight;

                #region 处理一些特殊情况

                //如果这个房间不可用，全线置为不可用
                if (item.State == LibRoom.LibRoomState.Close)
                {
                    var close = TimeLine.GetClosed(1, TimeLine.TimeLineState.StartAndEnd);
                    stack.Children.Add(close);
                    continue;
                }

                //如果这个房间没有预约数据
                if (item.Times is null || item.Times.Count == 0)
                {
                    if (Params.Date == NearDays.Tomorrow) //若是明天的预约，全线都是空闲
                    {
                        usage = TimeLine.GetFree(1, TimeLine.TimeLineState.StartAndEnd);
                        SolveFreeTimeLine(usage, StartTime, EndTime, item);
                        stack.Children.Add(usage);
                        continue;
                    }

                    if (Now.CompareTo(EndTime) > 0) //现在晚于关闭时间，全线置为过时
                    {
                        stack.Children.Add(TimeLine.GetOutTime(1, TimeLine.TimeLineState.StartAndEnd));
                        continue;
                    }

                    if (Now.CompareTo(StartTime) < 0) //现在时间早于起始时间，全线都是空闲
                    {
                        usage = TimeLine.GetFree(1, TimeLine.TimeLineState.StartAndEnd);
                        SolveFreeTimeLine(usage, StartTime, EndTime, item);
                        stack.Children.Add(usage);
                        continue;
                    }

                    //现在时间在开放时间内，时间线被过时和空闲两种状态分别占有
                    weight = (Now - StartTime) / 5;
                    usage = TimeLine.GetOutTime(weight, TimeLine.TimeLineState.Start);
                    stack.Children.Add(usage);

                    weight = (EndTime - Now) / 5;
                    usage = TimeLine.GetFree(weight, TimeLine.TimeLineState.End);
                    SolveFreeTimeLine(usage, Now, EndTime, item);
                    stack.Children.Add(usage);
                    continue;
                }

                #endregion

                #region 绘制第一个预约之前的时间线

                //如果是明天，第一个预约之前全是空闲时间
                if (Params.Date == NearDays.Tomorrow)
                {
                    weight = (item.Times[0].Start - StartTime) / 5;
                    usage = TimeLine.GetFree(weight, TimeLine.TimeLineState.Start);
                    SolveFreeTimeLine(usage, StartTime, item.Times[0].Start, item);
                    stack.Children.Add(usage);
                }
                else
                {
                    if (Now.CompareTo(item.Times[0].Start) < 0) //现在时间早于第一个预约的开始时间
                    {
                        //现在时间早于开始时间，那么从开馆到第一个预约全是空闲时间
                        if (Now.CompareTo(StartTime) < 0)
                        {
                            weight = (item.Times[0].Start - StartTime) / 5;
                            usage = TimeLine.GetFree(weight, TimeLine.TimeLineState.Start);
                            SolveFreeTimeLine(usage, StartTime, item.Times[0].Start, item);
                            stack.Children.Add(usage);
                        }
                        else
                        {
                            //如果现在晚于开馆时间，那么开馆到第一个预约之间被过时和空闲分占
                            var outTime = (Now - StartTime) / 5;
                            usage = TimeLine.GetOutTime(outTime, TimeLine.TimeLineState.Start);
                            stack.Children.Add(usage);

                            weight = (item.Times[0].Start - StartTime) / 5 - outTime;
                            usage = TimeLine.GetFree(weight);
                            SolveFreeTimeLine(usage, Now, item.Times[0].Start, item);
                            stack.Children.Add(usage);
                        }
                    }
                    else
                    {
                        //现在时间在第一个时间之后，实际上一定在第一个预约之中
                        weight = (item.Times[0].Start - StartTime) / 5;
                        usage = TimeLine.GetOutTime(weight, TimeLine.TimeLineState.Start);
                        stack.Children.Add(usage);
                    }
                }

                #endregion

                #region 绘制第一个预约到最后一个预约的时间线

                var len = item.Times.Count - 1;
                int freeT;
                TimeLine u, f;
                for (var i = 0; i < len; i++)
                {
                    weight = (item.Times[i].End - item.Times[i].Start) / 5;
                    u = TimeLine.GetUsing(weight);
                    stack.Children.Add(u);
                    SolveUsingTimeLine(u, item.Times[i]);

                    freeT = (item.Times[i + 1].Start - item.Times[i].End) / 5;
                    if (freeT == 0) continue;
                    f = TimeLine.GetFree(freeT);
                    SolveFreeTimeLine(f, item.Times[i].End, item.Times[i + 1].Start, item);
                    stack.Children.Add(f);
                }

                #endregion

                #region 绘制最后一个预约之后的时间线

                freeT = (EndTime - item.Times[len].End) / 5;
                weight = (item.Times[len].End - item.Times[len].Start) / 5;
                u = TimeLine.GetUsing(weight, freeT <= 0 ? TimeLine.TimeLineState.End : TimeLine.TimeLineState.Mid);
                stack.Children.Add(u);
                
                SolveUsingTimeLine(u, item.Times[len]);

                if (freeT <= 0) continue;
                f = TimeLine.GetFree(freeT, TimeLine.TimeLineState.End);
                SolveFreeTimeLine(f, item.Times[len].End, EndTime, item);
                stack.Children.Add(f);

                #endregion
            }

        }

        private void SolveFreeTimeLine(TimeLine line, Time start, Time end, object bindingContext)
        {
            line.SetBindingMsg(start, end, bindingContext);
            line.Click += StartResvRoom;
        }

        private void SolveUsingTimeLine(TimeLine line, TimeSlot times)
        {
            line.SetBindingMsg(times.Start, times.End, null);
            line.TextMessage = $"{times.Msg}\n开始时间：{times.Start}\n结束时间：{times.End}";
            line.Click += ShowMessage;
        }
        private async void ShowMessage(object sender, EventArgs e)
        {
            if (!(sender is TimeLine line)) return;
            if (line.TextMessage is null) return;
            await RequestMessageAsync("预约信息", line.TextMessage, "彳亍");
        }
        private void OnSizeChanged(object sender, EventArgs e)
        {
            foreach (var item in _timeLineStack.Children)
            {
                if (!(item is StackLayout stackLayout)) continue;
                var totalWeight = 0.0;
                var deHeight = 0.0;
                foreach (var line in stackLayout.Children)
                {
                    if (line is TimeLine timeLine)
                    {
                        totalWeight += timeLine.Weight;
                    }
                    else
                    {
                        deHeight += line.Height;
                    }
                }

                var height = stackLayout.Height - deHeight - (_timeTable.Height - 20) / 15;

                foreach (var line in stackLayout.Children)
                {
                    if (!(line is TimeLine timeLine)) continue;
                    timeLine.HeightRequest = (timeLine.Weight / totalWeight) * height;
                }
            }
        }
        private bool IsPushing { get; set; }
        private async void StartResvRoom(object sender, EventArgs e)
        {
            if (IsPushing) return;
            IsPushing = true;
            var timeLine = sender as TimeLine;
            if (!(timeLine?.BindingContext is LibRoom libRoom)) return;
            if (timeLine.End - timeLine.Start < libRoom.MinMins)
            {
                await NoticeError($"该时间段小于{libRoom.MinMins}分钟");
                IsPushing = false;
                return;
            }

            var requPage = new LibRoomRequestPage();
            await requPage.PushAsync();
            requPage.SetNavigationArguments(new LibRoomRequestParams
            {
                ResultPage = this,
                TimeSlot = new TimeSlot {Start = timeLine.Start, End = timeLine.End},
                LibRoom = libRoom,
                Date = Params.Date
            });
            IsPushing = false;
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

        public async Task Refresh()
        {
            var res = await LibRoomReservationViewModel.Instance.GetRoomAsync(Params.GetRoomUsageParams);
            if (!res.IsSuccess)
            {
                if (res.Msg is null) return;
                await NoticeError(res.ToString());
                return;
            }

            Params = res.Msg as LibRoomResultPageParams;
            ClearView();
            InitTable();
            Now = new Time(DateTime.Now);
            InitTimeLine();
        }

        public LibRoomResultPage()
        {
            InitTable();
            Now = new Time(DateTime.Now);
        }
    }
}