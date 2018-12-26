﻿using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace HandSchool.JLU
{
    class GradeItem : IGradeItem
    {
        private ArchiveScoreValue asv;

        public GradeItem(ArchiveScoreValue value)
        {
            asv = value;
            Attach = new NameValueCollection
            {
                { "选课课号", asv.xkkh }
            };
        }

        public string Name => asv.course.courName;
        public string Score => asv.score;
        public string Point => asv.gpoint;
        public string Credit => asv.credit;
        public bool ReSelect => asv.isReselect != "N";
        public bool Pass => asv.isPass == "Y";
        public string Term => asv.teachingTerm.termName;
        public DateTime Date => asv.dateScore;
        public NameValueCollection Attach { get; private set; }

        public string Type => AlreadyKnownThings.Type5Name(asv.type5);

        public string Show => string.Format("{2}发布；{0}通过，绩点 {1}。", Pass ? "已" : "未", Point, Date.ToShortDateString());

        static readonly string[] ChartShrooms = new[]
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

        public string Name => asv.kcmc;
        public string Score => asv.zscj;
        public string Point => asv.gpoint;
        public string Credit => asv.credit;
        public bool ReSelect => asv.isReselect != "N";
        public bool Pass { get; private set; }
        public string Term { get; private set; }
        public DateTime Date => DateTime.Now;
        public NameValueCollection Attach { get; private set; }
        public string Type => "未知";

        public string Show => string.Format("{2}刷新；{0}通过，绩点 {1}。", Pass ? "已" : "未", Point, Date.ToShortDateString());

        public IEnumerable<Entry> GetGradeDistribute()
        {
            yield break;
        }
    }

    [Entrance("成绩查询")]
    class GradeEntrance : IGradeEntrance
    {
        internal const string config_grade = "jlu.grade.json";
        internal const string config_gpa = "jlu.gpa.json";
        internal const string grade_distribute = "score/course-score-stat.do";

        public int RowLimit { get; set; } = 25;
        
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string PostValue => "{\"tag\":\"archiveScore@queryCourseScore\",\"branch\":\"latest\",\"params\":{},\"rowLimit\":" + RowLimit + "}";
        public string GPAPostValue => "{\"type\":\"query\",\"res\":\"stat-avg-gpoint\",\"params\":{\"studId\":`studId`}}";
        public string StorageFile => config_grade;
        public string LastReport { get; private set; }
        public string LastReportGPA { get; private set; }

        public async Task Execute()
        {
            try
            {
                LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
                var ro = LastReport.ParseJSON<RootObject<ArchiveScoreValue>>();

                foreach (var asv in ro.value)
                {
                    var LastDetail = await Core.App.Service.Post(grade_distribute, $"{{\"asId\":\"{asv.asId}\"}}");
                    //LastDetail = Encoding.UTF8.
                    asv.distribute = LastDetail.ParseJSON<GradeDetails>();
                }

                Core.WriteConfig(config_grade, ro.Serialize());

                LastReportGPA = await Core.App.Service.Post(ScriptFileUri, GPAPostValue);
                Core.WriteConfig(config_gpa, LastReportGPA);
                ParseGPA();

                foreach (var asv in ro.value)
                {
                    GradePointViewModel.Instance.Items.Add(new GradeItem(asv));
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    await GradePointViewModel.Instance.ShowMessage("错误", "连接超时，请重试。");
                    return;
                }

                throw ex;
            }
        }
        
        public GradeEntrance()
        {
            Task.Run(async () =>
            {
                await Task.Yield();
                LastReportGPA = Core.ReadConfig(config_gpa);
                if (LastReportGPA != "") ParseGPA();
                LastReport = Core.ReadConfig(config_grade);
                if (LastReport != "") Parse();
            });
        }

        public void Parse()
        {
            var ro = LastReport.ParseJSON<RootObject<ArchiveScoreValue>>();
            
            foreach (var asv in ro.value)
            {
                GradePointViewModel.Instance.Items.Add(new GradeItem(asv));
            }
        }

        public void ParseGPA()
        {
            var ro = LastReportGPA.ParseJSON<RootObject<GPAValue>>();
            var str = string.Format("按首次成绩\n学分平均绩点 {0:N6}\n学分平均成绩 {1:N6}\n\n按最好成绩\n学分平均绩点 {2:N6}\n学分平均成绩 {3:N6}",
                ro.value[0].gpaFirst, ro.value[0].avgScoreFirst, ro.value[0].gpaBest, ro.value[0].avgScoreBest);
            GradePointViewModel.Instance.Items.Clear();
            GradePointViewModel.Instance.Items.Add(new GPAItem(str));
        }

        public async Task GatherGPA()
        {
            try
            {
                LastReportGPA = await Core.App.Service.Post(ScriptFileUri, GPAPostValue);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    await GradePointViewModel.Instance.ShowMessage("错误", "连接超时，请重试。");
                    return;
                }

                throw ex;
            }

            Core.WriteConfig(config_gpa, LastReportGPA);
            ParseGPA();
        }
    }
}
