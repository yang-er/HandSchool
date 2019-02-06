using HandSchool.Internals;
using HandSchool.Internals.HtmlObject;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;

namespace HandSchool.JLU.InfoQuery
{
    /// <summary>
    /// 实现一键教学评价的入口。
    /// </summary>
    /// <inheritdoc cref="HotfixController" />
    [Entrance("JLU", "一键教学评价", "一键教学评价，省去麻烦事。", EntranceType.InfoEntrance)]
    [Hotfix(Loader.FileBaseUrl + "/InfoQuery/teacheval.js.ver", "jlu_teacheval.js")]
    internal class TeachEvaluate : HotfixController
    {
        protected override void HandlePostReturnValue(string[] ops, ref string ret)
        {
            if (ret == "") ret = "{\"error\":\"null\"}";
            base.HandlePostReturnValue(ops, ref ret);
        }

        public TeachEvaluate()
        {
            var usageDescription = new FirstPara("本功能可以帮助你完成评教。" +
                "蓝色代表可以评价，黄色代表需要手动登录网页评价，绿色代表评价完成。");
            
            var table = new Table(false, "evalItemList") { "教师", "学院" };
            if (Core.Platform.RuntimeName == "UWP") table.Add("教学任务");

            HtmlDocument = new Bootstrap
            {
                Children = { usageDescription, table },
                JavaScript =
                {
                    $"var list = []; var i = 0, len = 0; " +
                    $"var uwp = {(Core.Platform.RuntimeName == "UWP" ? "true" : "false")};",
                    HotfixAttribute.ReadContent(this) ?? "invokeCSharpAction('msg;模块热更新出现问题，请重启应用尝试。')"
                }
            };

            Menu.Add(new MenuEntry
            {
                Title = "开始",
                UWPIcon = "\uE8B0",
                Command = new CommandAction(() => Evaluate("solve()")),
            });
        }
    }
}