using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HandSchool.JLU.Models
{
    class InsideGradeItem : IGradeItem
    {
        private readonly ArchiveScoreValue asv;

        public InsideGradeItem(ArchiveScoreValue value)
        {
            asv = value;
            Attach = new NameValueCollection { { "选课课号", asv.xkkh } };

            var gradeInternal = FirstScore + (int.TryParse(FirstScore, out var sc) ? " 分" : "");
            Detail = $"{Date:d}发布\n{gradeInternal} | {Credit} 学分 | 绩点 {FirstPoint}";
        }

        public string Title => asv.course.courName;
        public string FirstScore => asv.score;
        public string HighestScore => FirstScore;
        public string FirstPoint => asv.gpoint;
        public string HightestPoint => FirstPoint;
        public string Credit => asv.credit;
        public bool ReSelect => asv.isReselect == "Y";
        public bool IsPassed => asv.isPass == "Y";
        public string Term => asv.teachingTerm.termName;
        public DateTime Date => asv.dateScore;
        public NameValueCollection Attach { get; }

        public string Type => AlreadyKnownThings.Type5Name(asv.type5);
        public Xamarin.Forms.Color TypeColor => AlreadyKnownThings.Type5Color(asv.type5);
        

        public string Detail { get; }

        static readonly string[] ChartShrooms =
        {
            "#bf6913",
            "#6913bf",
            "#13bfbf",
            "#69bf13",
            "#bf1313",
        };

        public IEnumerable<Entry> GetGradeDistribute()
        {
            if (asv.distribute is null) yield break;

            int color_id = 0;
            foreach (var entitles in asv.distribute.items)
            {
                var valueLabel = (entitles.percent / 100).ToString("#.#%");
                if (valueLabel == "%") valueLabel = "0.0%";
                var skcolor = SkiaSharp.SKColor.Parse(ChartShrooms[color_id++ % 5]);

                yield return new Entry(entitles.percent)
                {
                    Label = entitles.label.Split('(')[0],
                    ValueLabel = valueLabel,
                    Color = skcolor,
                    TextColor = skcolor
                };
            }
        }
    }
}
