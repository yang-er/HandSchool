using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HandSchool.JLU.Models
{
    class OutsideGradeItem : IGradeItem
    {
        private OutsideScoreValue asv;

        public OutsideGradeItem(OutsideScoreValue value)
        {
            asv = value;
            Term = value.xkkh.Substring(1, 9) + "第" + value.xkkh.Substring(11, 1) + "学期";
            Pass = double.Parse(value.gpoint) > 0.1;

            Attach = new NameValueCollection
            {
                { "选课课号", asv.xkkh }
            };
        }

        public string Title => asv.kcmc;
        public string Score => asv.zscj;
        public string Point => asv.gpoint;
        public string Credit => asv.credit;
        public bool ReSelect => asv.isReselect == "Y";
        public bool Pass { get; private set; }
        public string Term { get; private set; }
        public DateTime Date => DateTime.Now;
        public NameValueCollection Attach { get; private set; }
        public string Type => "未知";

        public string Detail => string.Format("{2}刷新；{0}通过，绩点 {1}。", Pass ? "已" : "未", Point, Date.ToString("d"));

        public IEnumerable<Entry> GetGradeDistribute()
        {
            yield break;
        }
    }
}
