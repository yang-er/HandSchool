using System;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.JLU.Models
{
    public class WeekInfo
    {
        public int week;
        public string last_update;
        public int delta;
        public WeekInfo() { }
        public WeekInfo(int week,string last_update, int delta)
        {
            this.week = week;
            this.last_update = last_update;
            this.delta = delta;
        }
    }
}
