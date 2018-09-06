using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("网上选课", "对您备选的选课列表进行操作。", EntranceType.InfoEntrance)]
    [Hotfix("https://raw.githubusercontent.com/yang-er/HandSchool/master/HandSchool/HandSchool/JLU/InfoQuery/selectcourse.js.ver", "jlu_selectcourse.js")]
    class SelectCourse : IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }
        public IViewResponse Binding { get; set; }
        public Action<string> Evaluate { get; set; }
        public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();
        public string LastReport { get; private set; }

        public string ScriptFileUri => "service/res.do";
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
                if (ops[1] == "action/select/select-lesson.do")
                    ret = "{\"id\":\"selectlesson\",\"send\":" + ops[2] + ",\"value\":" + ret + "}";
                System.Diagnostics.Debug.WriteLine(ret);
                Evaluate("te_callback(" + ret + ")");
            }
            else if (data.StartsWith("msg;"))
            {
                var ops = data.Split(new char[] { ';' }, 2);
                await Binding.ShowMessage("消息", ops[1]);
            }
        }

        public SelectCourse()
        {
            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    "<p class=\"mt-3\">目前选课：<span id=\"splanName\">未知</span></p>" +

                    "<table class=\"table table-responsive\">" +
                    "<thead><tr><th style=\"min-width:3.5em\">状态</th><th style=\"min-width:14em\">课程</th>" +
                    "<th style=\"min-width:5em\">类型</th></tr></thead><tbody id=\"courList\">" +
                    "</tbody></table>" +

                    "<table class=\"table table-responsive\">" +
                    "<thead><tr><th style=\"min-width:5em\">教师</th><th style=\"min-width:4em\">操作</th>" +
                    "<th style=\"min-width:15em\">时间</th></tr></thead><tbody id=\"schList\">" +
                    "</tbody></table>".ToRawHtml()
                },
                JavaScript =
                {
                    HotfixAttribute.ReadContent(this) ?? "invokeCSharpAction('msg;模块热更新出现问题，请重启应用尝试。')"
                }
            };
        }
    }
}
