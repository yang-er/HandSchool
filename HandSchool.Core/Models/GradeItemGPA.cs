using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Color = Xamarin.Forms.Color;
using Entry = Microcharts.Entry;

namespace HandSchool.Models
{
    /// <summary>
    /// 保存字符串，用于展示GPA。
    /// </summary>
    public class GPAItem : IGradeItem
    {
        public string Title => "GPA统计";
        public string FirstScore => "";
        public string HighestScore => "";
        public string FirstPoint => "";
        public string HightestPoint => "";
        public string Type => "";
        public Color TypeColor => Color.Transparent;
        public string Credit => "";
        public string Term => "";
        public bool ReSelect => false;
        public bool IsPassed => true;
        public NameValueCollection Attach => null;

        public DateTime Date { get; }
        public string Detail { get; }

        /// <summary>
        /// 建立新的GPA项目。
        /// </summary>
        /// <param name="toShow">将被展示的内容。</param>
        public GPAItem(string toShow)
        {
            Detail = toShow;
            Date = DateTime.Now;
        }

        public IEnumerable<Entry> GetGradeDistribute()
        {
            yield break;
        }
    }
}