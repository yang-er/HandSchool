using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Threading.Tasks;

namespace HandSchool.JLU.Services
{
    [Entrance("JLU", "成绩查询")]
    class CJCXGrade : IGradeEntrance
    {
        internal const string config_grade = "jlu.grade3.json";
        
        public string ScriptFileUri => "service_res.php";
        public bool IsPost => true;
        public string PostValue => "{\"tag\":\"lessonSelectResult@oldStudScore\",\"params\":{\"xh\":\"00000000\"}}";
        public string GPAPostValue { get; }
        public string StorageFile => config_grade;
        public string LastReport { get; private set; }

        public async Task Execute()
        {
            try
            {
                LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
                var ro = LastReport.ParseJSON<CJCXCJ>();
                Core.Configure.Write(config_grade, LastReport);

                GradePointViewModel.Instance.Items.Clear();

                foreach (var asv in ro.items)
                {
                    GradePointViewModel.Instance.Items.Add(new CJCXGradeItem(asv));
                }
            }
            catch (WebsException ex)
            {
                if (ex.Status != WebStatus.Timeout) throw;
                await GradePointViewModel.Instance.RequestMessageAsync("错误", "连接超时，请重试。");
            }
        }

        public CJCXGrade()
        {
            Task.Run(async () =>
            {
                await Task.Yield();
                LastReport = Core.Configure.Read(config_grade);
                if (LastReport != "") Parse();
            });
        }

        public void Parse()
        {
            var ro = LastReport.ParseJSON<CJCXCJ>();

            GradePointViewModel.Instance.Items.Clear();

            foreach (var asv in ro.items)
            {
                GradePointViewModel.Instance.Items.Add(new CJCXGradeItem(asv));
            }
        }

        public Task GatherGPA() => Task.CompletedTask;

        public void ParseGPA() { }
    }
}
