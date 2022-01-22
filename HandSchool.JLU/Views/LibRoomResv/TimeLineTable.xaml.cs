using System;
using HandSchool.JLU.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views.LibRoomResv
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeLineTable
    {
        private class TimeLineWithTitle : StackLayout, IDisposable
        {
            private bool _disposed;
            public Label Title { get; }
            private TimeLine _timeLine;
            public TimeLine TimeLine
            {
                get => _timeLine;
                set
                {
                    if (ReferenceEquals(_timeLine, value)) return;
                    if (_timeLine != null)
                    {
                        Children.Remove(_timeLine);
                        _timeLine.Dispose();
                    }
                    if (value != null)
                    {
                        Children.Add(value);
                    }
                    _timeLine = value;
                }
            }
            public TimeLineWithTitle(string title)
            {
                Title = new Label
                {
                    Text = title,
                    TextColor = Color.Gray,
                    HorizontalOptions = LayoutOptions.Center
                };
                Children.Add(Title);
            }
            public void Dispose()
            {
                if (_disposed) return;
                TimeLine?.Dispose();
                _disposed = true;
            }
        }

        public Time StartTime { get; }
        public Time EndTime { get; }
        public Time RealStartTime { get; }
        public Time RealEndTime { get; }
        private readonly RowDefinition _row = new RowDefinition { Height = GridLength.Star };
        private readonly int _lines;

        public TimeLineTable(Time start, Time end)
        {
            //根据传入的起始时间计算时间线的起始时间
            //规则是在起始时间之前和结束时间之后找到一个最近的“整点”（00分/30分）
            RealStartTime = start;
            RealEndTime = end;
            if (RealStartTime.Min != 0 && RealStartTime.Min != 30)
            {
                StartTime = new Time($"{RealStartTime.Hour}:{(RealStartTime.Min < 30 ? 00 : 30)}");
            }
            else
            {
                StartTime = RealStartTime;
            }

            if (RealEndTime.Min != 0 && RealEndTime.Min != 30)
            {
                if (RealStartTime.Min < 30)
                {
                    EndTime = StartTime.Min == 0 ? new Time($"{(RealEndTime + 60).Hour}:{00}") : new Time($"{RealEndTime.Hour}:{30}");
                }
                else
                {
                    EndTime = StartTime.Min == 0 ? new Time($"{(RealEndTime + 60).Hour}:{00}") : new Time($"{(RealEndTime + 60).Hour}:{30}");
                }
            }
            else
            {
                EndTime = StartTime.Min == RealEndTime.Min ? RealEndTime : (RealEndTime + 30);
            }
            InitializeComponent();

            TimeTable.ColumnDefinitions.Add(new ColumnDefinition { Width = Device.RuntimePlatform == Device.iOS ? 50 : 40 });
            TimeTable.RowDefinitions.Add(new RowDefinition { Height = 20 });

            //若时间线长度长于16小时，则时间标签步长为2h，否则步长为1小时
            var n = EndTime - StartTime <= 960 ? 1 : 2;
            if (n == 2 && (EndTime.Hour - StartTime.Hour) % 2 != 0)
            {
                EndTime += 60;
            }

            //添加网格行
            for (var i = StartTime; i.CompareTo(EndTime) <= 0; i += n * 60)
            {
                TimeTable.RowDefinitions.Add(_row);
            }

            //开始生成时间标签
            var curRow = 1;
            for (var i = StartTime; i.CompareTo(EndTime) <= 0; i += n * 60)
            {
                var oClock = new Label
                { Text = $"{i}", TextColor = Color.Gray, HorizontalOptions = LayoutOptions.CenterAndExpand };
                TimeTable.Children.Add(oClock);
                Grid.SetColumn(oClock, 0);
                Grid.SetRow(oClock, curRow);
                curRow += 1;
            }
            _lines = curRow - 1;
        }

        private const int TitleHeight = 20;

        /// <summary>
        /// 向时间表中加入一条时间线并返回这条时间线
        /// </summary>
        /// <param name="title">这条时间线的名字</param>
        /// <returns>一个时间线对象</returns>
        public TimeLine CreateAndAddLine(string title)
        {
            var stack = new TimeLineWithTitle(title)
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                WidthRequest = 50,
                VerticalOptions = LayoutOptions.FillAndExpand,
                TimeLine = new TimeLine(StartTime, EndTime)
                {
                    HeightRequest = (Height - TitleHeight) * (_lines - 1.0) / _lines
                }
            };
            stack.Title.HeightRequest = TitleHeight;

            var line = stack.TimeLine;
            if (StartTime.CompareTo(RealStartTime) != 0)
            {
                line.AddNotAvailTime(new TimeSlot
                {
                    Start = StartTime,
                    End = RealStartTime
                });
            }
            if (EndTime.CompareTo(RealEndTime) != 0)
            {
                line.AddNotAvailTime(new TimeSlot
                {
                    Start = RealEndTime,
                    End = EndTime
                });
            }
            TimeLineStack.Children.Add(stack);

            stack.LayoutChanged += MeasureSize;
            stack.SizeChanged += MeasureSize;
            return line;
        }

        private void MeasureSize(object sender, EventArgs args)
        {
            if (!(sender is TimeLineWithTitle tl)) return;
            if (tl.TimeLine != null)
            {
                tl.TimeLine.HeightRequest = (Height - TitleHeight) * (_lines - 1.0) / _lines;
            }
        }

        /// <summary>
        /// 清空时间表上的对象
        /// </summary>
        public void ClearView()
        {
            TimeLineStack.Children.ForEach(c =>
            {
                if (c is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });
            TimeLineStack.Children.Clear();
        }
    }
}