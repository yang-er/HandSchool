using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("一键教学评价", "一键教学评价，省去麻烦事。", EntranceType.InfoEntrance)]
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
        
        public Bootstrap HtmlDocument { get; set; }
        public IViewResponse Binding { get; set; }
        public Action<string> Evaluate { get; set; }
        public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();
        public string LastReport { get; private set; }
        
        public string ScriptFileUri => "action/eval/eval-with-answer.do";
        public bool IsPost => true;
        public string PostValue => null;
        public string StorageFile => "No storage";
        
        public Task Execute() { throw new InvalidOperationException(); }
        public void Parse() { throw new InvalidOperationException(); }

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
                    (RawHtml) ("<table class=\"table\" id=\"evalItemList\"><tr><th>教师</th><th>学院</th>" + (Core.RuntimePlatform == "UWP" ? "<th>教学任务</th>" : "") + "</tr></table>")
                },
                JavaScript =
                {
                    $"var studId = {Core.App.Service.AttachInfomation["studId"]}, term = {Core.App.Service.AttachInfomation["term"]}; var list = []; var i = 0, len = 0; var uwp = {(Core.RuntimePlatform == "UWP" ? "true" : "false")};",
                    HotfixAttribute.ReadContent(this) ?? "invokeCSharpAction('msg;模块热更新出现问题，请重启应用尝试。')"
                }
            };

            Menu.Add(new InfoEntranceMenu("开始", new Command(() => Evaluate("solve()")), "\uE8B0"));
        }
    }
}
