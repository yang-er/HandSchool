using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.Blank
{
    class Schedule : IScheduleEntrance
    {
        public List<CurriculumItem> Items { get; private set; }
        public string LastReport { get; private set; } = "";

        public int ClassNext => 0;
        public string Name => "课程表";
        public string ScriptFileUri => "";
        public bool IsPost => true;
        public string PostValue => "";
        public string StorageFile => "blank.kcb.json";

        public async Task Execute()
        {
            await Task.Run(() => System.Diagnostics.Debug.WriteLine("HandSchool.Blank.ScheduleEntrance->Excute()"));
            Parse();
            Save();
        }

        public void Parse()
        {
            System.Diagnostics.Debug.WriteLine("HandSchool.Blank.ScheduleEntrance->Parse()");
        }

        public void RenderWeek(int week, out List<CurriculumItem> list, bool showAll = false)
        {
            if (showAll)
                throw new NotImplementedException();

            list = Items.FindAll((item) => showAll || item.IfShow(week));
        }

        public void Save()
        {
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
            Core.WriteConfig("blank.kcb.json", Serialize(Items));
        }

        public Schedule()
        {
            LastReport = Core.ReadConfig("blank.kcb.json");
            if (LastReport != "")
                Items = JSON<List<CurriculumItem>>(LastReport);
            else
                Items = new List<CurriculumItem>();
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
        }
    }
}
