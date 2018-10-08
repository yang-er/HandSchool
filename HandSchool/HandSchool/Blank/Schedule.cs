using HandSchool.Services;
using System.Threading.Tasks;

namespace HandSchool.Blank
{
    [Entrance("课程表")]
    class Schedule : ScheduleEntranceBase
    {
        public override int ClassNext => 0;
        public override string ScriptFileUri => "";
        public override bool IsPost => true;
        public override string PostValue => "";
        public override string StorageFile => "blank.kcb.json";

        public override async Task Execute()
        {
            Core.Log("Blank.ScheduleEntrance->Excute()");
            await Task.Run(() => { });
            Parse();
            Save();
        }

        public override void Parse()
        {
            Core.Log("Blank.ScheduleEntrance->Parse()");
        }
        
        public Schedule() : base() { }
    }
}
