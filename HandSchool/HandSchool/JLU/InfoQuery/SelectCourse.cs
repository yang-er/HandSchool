using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;

namespace HandSchool.JLU.InfoQuery
{
    [Entrance("jlu", "网上选课", "对您备选的选课列表进行操作。", EntranceType.InfoEntrance)]
    [Hotfix("https://raw.githubusercontent.com/yang-er/HandSchool/master/HandSchool/HandSchool/JLU/InfoQuery/selectcourse.js.ver", "jlu_selectcourse.js")]
    class SelectCourse : HotfixController
    {
        protected override void HandlePostReturnValue(string[] ops, ref string ret)
        {
            if (ops[1] == "action/select/select-lesson.do")
                ret = "{\"id\":\"selectlesson\",\"send\":" + ops[2] + ",\"value\":" + ret + "}";
            if (ret == "") ret = "{\"error\":\"null\"}";
            base.HandlePostReturnValue(ops, ref ret);
        }

        public SelectCourse()
        {
            HtmlDocument = new Bootstrap
            {
                Children =
                {
                    "<p class=\"mt-3\">目前选课：<span id=\"splanName\">未知</span></p>" +

                    "<table class=\"table table-responsive\">" +
                    "<thead><tr><th style=\"min-width:4em\">状态</th><th style=\"min-width:14em\">课程</th>" +
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
