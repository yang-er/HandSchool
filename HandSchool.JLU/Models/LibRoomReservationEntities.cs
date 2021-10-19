using System;
using System.Collections.Generic;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Views;

namespace HandSchool.JLU.Models
{
    //相对时间，相对于当天0:00的时间
    public class Time : IComparable<Time>
    {
        public int Hour { get; private set; }
        public int Min { get; private set; }
        private int _addedDays;

        public Time(string time)
        {
            var t = time.Split(':');
            Hour = int.Parse(t[0]);
            Min = int.Parse(t[1]);
        }
        public Time(DateTime time)
        {
            Hour = time.Hour;
            Min = time.Minute;
        }

        private Time() { }
        public int CompareTo(Time other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            if (_addedDays > other._addedDays) return 1;
            if (_addedDays < other._addedDays) return -1;
            if (Hour > other.Hour) return 1;
            if (Hour < other.Hour) return -1;
            if (Min > other.Min) return 1;
            if (Min < other.Min) return -1;
            return 0;
        }

        public static int operator -(Time b, Time a)
        {
            return b.Min - a.Min + (b.Hour - a.Hour) * 60 + (b._addedDays - a._addedDays) * 24 * 60;
        }
        public override string ToString()
        {
            return $"{Hour:00}:{Min:00}";
        }

        public static Time operator -(Time a, int minute)
        {
            var min = a.Min - minute;
            var hour = a.Hour;
            var addDays = 0;
            if (min < 0)
            {
                var d = -min;
                var n = d / 60;
                hour -= n + 1;
                min = 60 - d + n;
                while (hour < 0)
                {
                    addDays--;
                    hour += 24;
                }
            }

            return new Time
            {
                Hour = hour,
                Min = min,
                _addedDays = addDays
            };
        }

        public static Time operator +(Time a, int minute)
        {
            var min = a.Min + minute;
            var hour = a.Hour + min / 60;
            var addDays = 0;
            while (hour >= 24)
            {
                addDays++;
                hour -= 24;
            }
            return new Time { Hour = hour % 24, Min = min % 60, _addedDays = addDays };
        }
    }
    public class LibRoomRequestParams
    {
        public LibRoomResultPage ResultPage { get; set; }
        public NearDays Date { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public LibRoom LibRoom { get; set; }
    }
    public class LibRoom
    {
        public enum LibRoomState { Close, NotClose }
        public LibRoomState State { get; set; }
        public string Name { get; set; }
        public string Floor { get; set; }
        public int? MinMins { get; set; }
        public int? MaxMins { get; set; }
        public int? MinUser { get; set; }
        public int? MaxUser { get; set; }
        public string LabId { get; set; }
        public string RoomId { get; set; }
        public string DevId { get; set; }
        public string KindId { get; set; }
        public List<string> OpenTimes { get; set; }
        public List<RoomUsageInfo> Ts { get; set; }
        public List<RoomUsageInfo> Cls { get; set; }
        public List<RoomUsageInfo> Ops { get; set; }
        public List<TimeSlot> Times { get; set; }
    }
    public class GetRoomUsageParams
    {
        public NearDays Date { get; set; }
        public int RoomType { get; set; }
    }
    public class LibRoomResultPageParams
    {
        public GetRoomUsageParams GetRoomUsageParams { get; set; }
        public string Title { get; set; }
        public NearDays Date { get; set; }
        public IList<LibRoom> Rooms { get; set; }
    }
    public class TimeSlot
    {
        public Time Start { get; set; }
        public Time End { get; set; }
        public string Msg { get; set; }
    }
    public class RoomUsageInfo
    {
        public string id { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
        public string state { get; set; }
        public string date { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string owner { get; set; }
        public string accno { get; set; }
        public string member { get; set; }
        public string limit { get; set; }
        public bool? occupy { get; set; }
    }

    public class ReservationInfo
    {
        public string Start { get; set; }
        public string End { get; set; }
        public DateTime? ResvTime { get; set; }
        public string Kind { get; set; }
        public string People { get; set; }
        public string RoomId { get; set; }
        public string State { get; set; }
        public string Owner { get; set; }
        public string ResvType { get; set; }
        public string RoomInfo => $"{Kind} {RoomId}";
        public string ResvInnerId { get; set; }
        public string Description => $"申请人：{Owner}\n预约时间：{ResvTime:MM-dd hh:mm}\n开始时间：{Start}\n结束时间：{End}\n成员：\n{People}\n状态：\n{State}";
    }
    public class Irregularities
    {
        public DateTime? DateTime { get; set; }
        public string Tips { get; set; }
        public string Place { get; set; }
        public string State { get; set; }
        public int? Score { get; set; }
        public string Description => $"时间：{DateTime}\n类型：{Tips}\n状态：{State}\n扣分：{Score}";
    }
}