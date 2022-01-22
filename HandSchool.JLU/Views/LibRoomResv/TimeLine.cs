using System;
using System.Collections.Generic;
using HandSchool.Internal;
using HandSchool.JLU.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HandSchool.JLU.Views
{
    /// <summary>
    /// 标识时间线状态的枚举类型
    /// </summary>
    public enum TimeLineState
    {
        NotAvail, Timeout, Free, Busy
    }
    
    /// <summary>
    /// 单条时间线
    /// </summary>
    public class TimeLine : StackLayout, IDisposable
    {
        /// <summary>
        /// 绘制时间线用
        /// </summary>
        internal class TimeSpanFrame : TouchableFrame, IDisposable
        {
            public TimeSlot TimeSlot { get; }
            private readonly TimeLine _father;
            
            /// <summary>
            /// 表示绑定内容是否为自定义， 默认BindingContext与包含它的TimeLine一致
            /// </summary>
            internal bool IsCustomBinding;
            public string TaskName { get; set; }
            private bool _isFirst;
            public bool IsFirst
            {
                get => _isFirst;
                set
                {
                    if (_isFirst == value) return;
                    var color = _innerLine.Color;
                    _innerLine.Color = Color.White;
                    var ori = _innerLine.CornerRadius;
                    _isFirst = value;
                    var newCr = value switch
                    {
                        true => new CornerRadius(10, 10, ori.BottomLeft, ori.BottomRight),
                        false => new CornerRadius(0, 0, ori.BottomLeft, ori.BottomRight),
                    };
                    _innerLine.CornerRadius = newCr;
                    _innerLine.Color = color;
                }
            }

            private bool _isLast;
            public bool IsLast
            {
                get => _isLast;
                set
                {
                    if (_isLast == value) return;
                    var color = _innerLine.Color;
                    _innerLine.Color = Color.White;
                    var ori = _innerLine.CornerRadius;
                    _isLast = value;
                    var newCr = value switch
                    {
                        true => new CornerRadius(ori.TopLeft, ori.TopRight, 10, 10),
                        false => new CornerRadius(ori.TopLeft, ori.TopRight, 0, 0),
                    };
                    _innerLine.CornerRadius = newCr;
                    _innerLine.Color = color;
                }
            }
            
            private TimeLineState _lineState;
            
            /// <summary>
            /// 表示此段时间的状态
            /// </summary>
            public TimeLineState LineState
            {
                get => _lineState;
                set
                {
                    if (_lineState == value) return;
                    OnLineStateChanged(_lineState, value);
                    _lineState = value;
                    if (!IsCustomBinding)
                    {
                        BindingContext = value == TimeLineState.NotAvail ? null : _father.BindingContext;
                    }

                    _innerLine.Color = value switch
                    {
                        TimeLineState.NotAvail => Color.Black,
                        TimeLineState.Timeout => Color.FromHex("#BEBEBE"),
                        TimeLineState.Busy => Color.FromHex("#4682b4"),
                        TimeLineState.Free => Color.FromHex("#E8D2AA"),
                        _ => _innerLine.Color
                    };
                }
            }
            
            public int MinuteSpan => TimeSlot.End - TimeSlot.Start;

            private readonly BoxView _innerLine;
            private TimeSpanFrame(TimeLine father, TimeSlot slot)
            {
                CornerRadius = 0;
                HasShadow = false;
                BackgroundColor = Color.FromRgba(255, 255, 255, 0);
                _father = father;
                TimeSlot = slot;
                Content = _innerLine = new BoxView();
                Margin = Padding = new Thickness(0);
                LineState = TimeLineState.Free;
            }
            
            internal static TimeSpanFrame New(TimeLine father, TimeSlot slot)
            {
                return new TimeSpanFrame(father, slot);
            }

            private void RemoveEvents(TimeLineState state)
            {
                switch (state)
                {
                    case TimeLineState.NotAvail:
                        break;
                    case TimeLineState.Timeout:
                        Click -= _father._timeoutClicked;
                        break;
                    case TimeLineState.Busy:
                        Click -= _father._busyTimeClicked;
                        break;
                    case TimeLineState.Free:
                        Click -= _father._freeTimeClicked;
                        break;
                }
            }

            private void AddEvents(TimeLineState state)
            {
                switch (state)
                {
                    case TimeLineState.NotAvail:
                        break;
                    case TimeLineState.Timeout:
                        Click += _father._timeoutClicked;
                        break;
                    case TimeLineState.Busy:
                        Click += _father._busyTimeClicked;
                        break;
                    case TimeLineState.Free:
                        Click += _father._freeTimeClicked;
                        break;
                }
            }

            private void OnLineStateChanged(TimeLineState oldState, TimeLineState newState)
            {
                RemoveEvents(oldState);
                AddEvents(newState);
            }

            protected override void OnPropertyChanged(string propertyName = null)
            {
                if (propertyName == nameof(BindingContext))
                {
                    if (BindingContext?.GetType().IsValueType == true)
                    {
                        if (!BindingContext.Equals(_father?.BindingContext))
                        {
                            IsCustomBinding = true;
                        }
                    }
                    else
                    {
                        if (!ReferenceEquals(_father?.BindingContext, BindingContext))
                        {
                            IsCustomBinding = true;
                        }
                    }
                }

                base.OnPropertyChanged(propertyName);
            }

            private bool _isDisposed;
            public void Dispose()
            {
                if (_isDisposed) return;
                _isDisposed = true;
                BindingContext = null;
                RemoveEvents(LineState);
            }
        }
        
        public Time StartTime { get; }
        public Time EndTime { get; }

        public TimeLine(Time startTime, Time endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            Spacing = 0;
            Orientation = StackOrientation.Vertical;
            _freeTimeClicked = (s, e) => FreeTimeClicked?.Invoke(s, e);
            _timeoutClicked = (s, e) => TimeoutClicked?.Invoke(s, e);
            _busyTimeClicked = (s, e) => BusyTimeClicked?.Invoke(s, e);
            
            var line = TimeSpanFrame.New(this, new TimeSlot
            {
                Start = StartTime,
                End = EndTime
            });
            line.IsFirst = line.IsLast = true;
            Children.Add(line);
            SizeChanged += MeasureSize;
        }

        /// <summary>
        /// 在时间线中添加一段时间，供类内使用
        /// </summary>
        /// <param name="times">时间段</param>
        /// <param name="binding">时间段绑定的内容</param>
        /// <param name="taskName">时间段代表事件的描述</param>
        /// <param name="state">该段时间的状态</param>
        private void AddTime(TimeSlot times, object binding, string taskName, TimeLineState state)
        {
            var line = TimeSpanFrame.New(this, times);
            if (binding != null)
            {
                line.IsCustomBinding = true;
                line.BindingContext = binding;
            }

            line.LineState = state;
            line.TaskName = taskName;

            var start = Children.IndexOf(v =>
            {
                var ts = ((TimeSpanFrame) v).TimeSlot;
                return times.Start.CompareTo(ts.Start) >= 0 &&
                       times.Start.CompareTo(ts.End) < 0;
            });
            var end = Children.IndexOf(v =>
            {
                var ts = ((TimeSpanFrame) v).TimeSlot;
                return times.End.CompareTo(ts.Start) > 0 &&
                       times.End.CompareTo(ts.End) <= 0;
            });
            var firstFrame = (TimeSpanFrame) Children[start];
            var endFrame = (TimeSpanFrame) Children[end];
            var newFirst = TimeSpanFrame.New(this, new TimeSlot
            {
                Start = firstFrame.TimeSlot.Start, End = times.Start
            });
            FrameCopy(firstFrame, newFirst);

            var newEnd = TimeSpanFrame.New(this, new TimeSlot
            {
                Start = times.End, End = endFrame.TimeSlot.End
            });
            FrameCopy(endFrame, newEnd);

            for (var i = start; i <= end; i++)
            {
                Children.RemoveAt(i);
            }

            var index = start;
            foreach (var item in MergeFrame(newFirst, line, newEnd))
            {
                Children.Insert(index++, item);
            }

            ((TimeSpanFrame) Children[0]).IsFirst = true;
            ((TimeSpanFrame) Children[Children.Count - 1]).IsLast = true;
            MeasureSize(this, null);
        }

        /// <summary>
        /// 复制Frame的主要属性
        /// </summary>
        private static void FrameCopy(TimeSpanFrame oldFrame, TimeSpanFrame newFrame)
        {
            newFrame.IsCustomBinding = oldFrame.IsCustomBinding;
            newFrame.BindingContext = oldFrame.BindingContext;
            newFrame.LineState = oldFrame.LineState;
            newFrame.TaskName = oldFrame.TaskName;
        }
        
        /// <summary>
        /// 合并相邻Frame
        /// </summary>
        private IEnumerable<TimeSpanFrame> MergeFrame(TimeSpanFrame start, TimeSpanFrame mid, TimeSpanFrame end)
        {
            var res = new[] {true, true, true};
            if (start.MinuteSpan == 0)
            {
                res[0] = false;
            }

            if (end.MinuteSpan == 0)
            {
                res[2] = false;
            }
            if (start.LineState == mid.LineState)
            {
                mid.TimeSlot.Start = start.TimeSlot.Start;
                res[0] = false;
            }
            if (end.LineState == mid.LineState)
            {
                mid.TimeSlot.End = end.TimeSlot.End;
                res[2] = false;
            }

            if (res[0])
            {
                yield return start;
            }
            else
            {
                start.BindingContext = null;
            }
            if (res[1]) yield return mid;
            if (res[2])
            {
                yield return end;
            }
            else
            {
                end.BindingContext = null;
            }
        }
        
        /// <summary>
        /// 插入一段状态为“忙”的时间段
        /// </summary>
        public void AddBusyTime(TimeSlot times, string taskName, object binding = null)
        {
            AddTime(times, binding, taskName, TimeLineState.Busy);
        }
        
        /// <summary>
        /// 插入一段状态为“不可用”的时间段
        /// </summary>
        public void AddNotAvailTime(TimeSlot times, object binding = null)
        {
            AddTime(times, binding, null, TimeLineState.NotAvail);
        }
        
        /// <summary>
        /// 插入一段状态为“过期”的时间段
        /// </summary>
        public void AddTimeout(TimeSlot times, object binding = null)
        {
            AddTime(times, binding,null, TimeLineState.Timeout);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(BindingContext))
            {
                Children.ForEach(c =>
                {
                    if (!(c is TimeSpanFrame ts)) return;
                    if (!ts.IsCustomBinding && ts.LineState != TimeLineState.NotAvail)
                    {
                        ts.BindingContext = BindingContext;
                    }
                });
            }
        }
        
        private void MeasureSize(object sender, EventArgs args)
        {
            var height = Height;
            var time = EndTime - StartTime;
            Children.ForEach(c =>
            {
                if (!(c is TimeSpanFrame frame)) return;
                frame.HeightRequest = height * (frame.MinuteSpan / (1.0 * time));
            });
        }

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Children.ForEach(c =>
            {
                if (c is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });
            FreeTimeClicked = null;
            BusyTimeClicked = null;
            TimeoutClicked = null;
        }

        private readonly EventHandler<EventArgs> _freeTimeClicked;
        private readonly EventHandler<EventArgs> _busyTimeClicked;
        private readonly EventHandler<EventArgs> _timeoutClicked;

        public event EventHandler<EventArgs> FreeTimeClicked;
        public event EventHandler<EventArgs> BusyTimeClicked;
        public event EventHandler<EventArgs> TimeoutClicked;
    }
}