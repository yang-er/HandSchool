using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HandSchool.Models
{
    /// <summary>
    /// 保存字符串，用于展示GPA。
    /// </summary>
    public class GPAItem : IGradeItem
    {
        public string Title => "GPA统计";

        public string Score => "";
        public string Point => "";
        public string Type => "";
        public string Credit => "";
        public string Term => "";
        public bool ReSelect => false;
        public bool Pass => true;
        public NameValueCollection Attach => null;

        public DateTime Date { get; }
        public string Detail { get; }

        /// <summary>
        /// 建立新的GPA项目。
        /// </summary>
        /// <param name="to_show">将被展示的内容。</param>
        public GPAItem(string to_show)
        {
            Detail = to_show;
            Date = DateTime.Now;
        }

        public IEnumerable<Entry> GetGradeDistribute()
        {
            yield break;
        }
    }
}
