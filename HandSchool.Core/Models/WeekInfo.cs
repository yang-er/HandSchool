using System;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Models
{
    public class WeekInfo
    {
        public int week;
        public string last_update;
        public WeekInfo() { }
        public WeekInfo(int week,string last_update)
        {
            this.week = week;
            this.last_update = last_update;
        }
    }
}
