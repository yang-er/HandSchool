using HandSchool.Services;
using HandSchool.ViewModels;
using System.Threading.Tasks;
using HandSchool.Internal;

namespace HandSchool.Blank
{
    [Entrance("blank", "课程表")]
    class Schedule : IScheduleEntrance
    {
        public int ClassNext => 0;
        public string ScriptFileUri => "";
        public bool IsPost => true;
        public string PostValue => "";
        public string StorageFile => "blank.kcb.json";
        public string LastReport { get; set; }

        public async Task Execute()
        {
            Core.Log("Blank.ScheduleEntrance->Excute()");
            await Task.Run(() => { });
            Parse();
            ScheduleViewModel.Instance.SaveToFile();
        }

        public void Parse()
        {
            Core.Log("Blank.ScheduleEntrance->Parse()");
        }
        
        public Schedule() : base() { }
    }
}
