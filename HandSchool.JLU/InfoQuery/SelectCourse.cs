using HandSchool.Internals;
using HandSchool.Internals.HtmlObject;
using HandSchool.Models;
using HandSchool.ViewModels;

namespace HandSchool.JLU.InfoQuery
{
    /// <summary>
    /// 实现选课的信息查询功能。
    /// </summary>
    /// <inheritdoc cref="HotfixController" />
    [Entrance("JLU", "网上选课", "对您备选的选课列表进行操作。", EntranceType.InfoEntrance)]
    [Hotfix(Loader.FileBaseUrl + "/InfoQuery/selectcourse.js.ver", "jlu_selectcourse.js")]
    internal sealed class SelectCourse : HotfixController
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
            var currentSplanName = new FirstPara("目前选课：<span id=\"splanName\">未知</span>");

            var courList = new TableResponsive(bodyId: "courList")
            {
                { "状态", 4 },
                { "课程", 14 },
                { "类型", 5 },
            };

            var schList = new TableResponsive(bodyId: "schList")
            {
                { "教师", 5 },
                { "操作", 4 },
                { "时间", 15 },
            };

            HtmlDocument = new Bootstrap
            {
                Children = { currentSplanName, courList, schList },
                JavaScript = { HotfixAttribute.ReadContent(this) }
            };
        }
    }
}
