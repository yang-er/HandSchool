using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU.InfoQuery
{
    [Hotfix("https://raw.githubusercontent.com/miasakachenmo/store/master/teacheval.js.ver", "jlu_teacheval.js")]
    class TeachEvaluate : IInfoEntrance
    {
        /**
         * 由于这个功能有点点难写，因为学校可能随时调整接口，
         * 我们将数据和逻辑处理全部交给JavaScript。
         * 
         * 在这里我们对JavaScript做几个约定：
         *   四个状态：
         *     加载中 = -1，
         *     加载完成 = 0，
         *     正在发送某一个 = 1，
         *     发送完某一个发送回调 = 2
         *   几个常量：
         *     studId, term
         *   发送来的数据只有：
         *     post;...;value=...
         *     finished
         *     begin
         *   发回的数据是te_callback(原json)，不做特殊处理
         */

        const string evalList = "{\"tag\":\"student@evalItem\",\"branch\":\"self\",\"params\":{\"blank\":\"Y\"}}";
        const string evalPost = "{\"guidelineId\":\"120\",\"evalItemId\":\"`evalItemId`\",\"answers\":{\"prob11\":\"A\",\"prob12\":\"A\",\"prob13\":\"N\",\"prob14\":\"A\",\"prob15\":\"A\",\"prob21\":\"A\",\"prob22\":\"A\",\"prob23\":\"A\",\"prob31\":\"A\",\"prob32\":\"A\",\"prob33\":\"A\",\"prob41\":\"A\",\"prob42\":\"A\",\"prob43\":\"A\",\"prob51\":\"A\",\"prob52\":\"A\",\"sat6\":\"A\",\"mulsel71\":\"K\",\"advice72\":\"good\",\"prob73\":\"Y\"},\"clicks\":{\"mulsel71\":176187,\"prob11\":143759,\"prob12\":146540,\"prob13\":148790,\"prob14\":150583,\"prob15\":152233,\"prob21\":153748,\"prob22\":155383,\"prob23\":156628,\"prob31\":158877,\"prob32\":161367,\"prob33\":164157,\"prob41\":165950,\"prob42\":167053,\"prob43\":168304,\"prob51\":169768,\"prob52\":170968,\"prob73\":177732,\"sat6\":172791,\"_boot_\":0}}";

        public string Description => throw new NotImplementedException();
        public List<string> TableHeader { get; set; }
        public Bootstrap HtmlDocument { get; set; }
        public IViewResponse Binding { get; set; }
        public Action<string> Evaluate { get; set; }
        public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();
        public string LastReport { get; private set; }

        public string Name => "一键教学评价";
        public string ScriptFileUri => "action/eval/eval-with-answer.do";
        public bool IsPost => true;
        public string PostValue => evalPost;
        public string StorageFile => "No storage";

        [Obsolete]
        public async Task Execute()
        {
            LastReport = await Core.App.Service.Post("service/res.do", evalList);
            var fetched_list = JSON<RootObject<StudEval>>(LastReport);
            foreach (var op in fetched_list.value)
            {
                if (op.evalActTime.evalGuideline.evalGuidelineId == "120")
                {
                    await Core.App.Service.Post("action/eval/eval-with-answer.do", evalPost.Replace("`evalItemId`", op.evalItemId));
                }
            }

            throw new NotImplementedException();
        }

        public void Parse() { }

        public async void Receive(string data)
        {
            if (data == "finished")
                Binding.SetIsBusy(false);
            else if (data == "begin")
                Binding.SetIsBusy(true, "信息查询中……");
            else if (data.StartsWith("post;"))
            {
                var ops = data.Split(new char[] { ';' }, 3);
                var ret = await Core.App.Service.Post(ops[1], ops[2]);
                System.Diagnostics.Debug.WriteLine(ret);
                Evaluate("te_callback(" + ret + ")");
            }
            else if (data.StartsWith("msg;"))
            {
                var ops = data.Split(new char[] { ';' }, 2);
                await Binding.ShowMessage("消息", ops[1]);
            }
        }

        public TeachEvaluate()
        {
            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    (RawHtml) "<p>本功能可以帮助你完成评教。蓝色代表可以评价，黄色代表需要手动登录网页评价，绿色代表评价完成。</p>",
                    (RawHtml) ("<table class=\"table\" id=\"evalItemList\"><tr><th>教师</th><th>学院</th>" + (Device.RuntimePlatform == "UWP" ? "<th>教学任务</th>" : "") + "</tr></table>")
                },
                JavaScript =
                {
                    $"var studId = {Core.App.Service.AttachInfomation["studId"]}, term = {Core.App.Service.AttachInfomation["term"]}; var list = []; var i = 0, len = 0; var uwp = {(Device.RuntimePlatform == "UWP" ? "true" : "false")};",
                    ReadConfFile("jlu_teacheval.js") ?? "invokeCSharpAction('msg;模块热更新出现问题，请重启应用尝试。')"
                }
            };

            Menu.Add(new InfoEntranceMenu("开始", new Command(() => Evaluate("solve()")), "\uE8B0"));
        }
    }
}
