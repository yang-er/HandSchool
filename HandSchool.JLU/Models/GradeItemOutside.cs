using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Color = Xamarin.Forms.Color;
using Entry = Microcharts.Entry;

namespace HandSchool.JLU.Models
{
    internal sealed class CJCXGradeItem : IGradeItem
    {
        private readonly CJCXCJ.Item Item;

        public CJCXGradeItem(CJCXCJ.Item item)
        {
            Item = item;
            Attach = new NameValueCollection { { "选课课号", item.xkkh } };

            var gradeInternal = Score + (int.TryParse(Score, out int sc) ? " 分" : "");
            Detail = string.Format("{1}，{2} 学分，绩点 {0}。", Point, gradeInternal, Credit);
            Pass = Item.gpoint > 0.4m;
            Term = "";
        }

        public string Title => Item.kcmc;
        public string Score => Item.cj;
        public string Point => string.Format("{0:1}", Item.gpoint);
        public string Type => "";
        public string Credit => string.Format("{0:1}", Item.credit);
        public bool ReSelect => Item.isReselect == "Y";
        public bool Pass { get; }
        public string Term { get; }
        public DateTime Date => DateTime.Now;
        public NameValueCollection Attach { get; }
        public string Detail { get; }
        public Color TypeColor => Color.Transparent;

        public IEnumerable<Entry> GetGradeDistribute()
        {
            yield break;
        }
    }
}
