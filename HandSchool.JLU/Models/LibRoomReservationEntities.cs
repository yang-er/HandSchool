using System;
using System.Collections.Generic;
using HandSchool.JLU.JsonObject;

namespace HandSchool.JLU.Models
{
    //相对时间，相对于当天0:00的时间
    public struct Time: IComparable<Time>
    {
        private TimeSpan _inner;
        public int Hour => _inner.Hours;
        public int Min => _inner.Minutes;
        public Time(string time)
        {
            var t = time.Split(':');
            _inner = new TimeSpan(int.Parse(t[0]), int.Parse(t[1]), 0);
        }
        
        public Time(DateTime time)
        {
            _inner = new TimeSpan(time.Hour, time.Minute, 0);
        }
        
        public int CompareTo(Time other)
        {
            return _inner.CompareTo(other._inner);
        }
        
        public static int operator -(Time b, Time a)
        {
            return (int)(b._inner - a._inner).TotalMinutes;
        }
        
        public override string ToString()
        {
            return $"{Hour:00}:{Min:00}";
        }
        
        public static Time operator -(Time a, int minute)
        {
            return new Time {_inner = a._inner.Add(new TimeSpan(0, -minute, 0))};
        }

        public static Time operator +(Time a, int minute)
        {
            return new Time {_inner = a._inner.Add(new TimeSpan(0, minute, 0))};
        }
    }

    public class LibRoomRequestParams
    {
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
        public Time OpenStart { get; set; }
        public Time OpenEnd { get; set; }
        public List<RoomUsageInfo> Ts { get; set; }
        public List<RoomUsageInfo> Cls { get; set; }
        public List<RoomUsageInfo> Ops { get; set; }
        public List<RoomUsingSpan> Times { get; set; }
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
    public class TimeSlot : ICloneable
    {
        public Time Start { get; set; }
        public Time End { get; set; }
        public object Clone()
        {
            return new TimeSlot {Start = Start, End = End};
        }
    }

    public struct RoomUsingSpan
    {
        public TimeSlot TimeSlot { get; set; }
        public string UserInfo { get; set; }
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
        public bool IsUsing { get; set; }
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