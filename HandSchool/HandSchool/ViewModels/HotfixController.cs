using HandSchool.Internal.HtmlObject;
using HandSchool.Services;
using System.Net;
using System.Threading.Tasks;

namespace HandSchool.Internal
{
    /// <summary>
    /// 我们将数据和逻辑处理全部交给JavaScript。
    /// </summary>
    /// <example>
    /// 在这里我们对JavaScript做几个约定：
    ///   四个状态：
    ///     加载中 = -1，
    ///     加载完成 = 0，
    ///     正在发送某一个 = 1，
    ///     发送完某一个发送回调 = 2
    ///   几个常量：
    ///     studId, term
    ///   发送来的数据只有：
    ///     post;...;value=...
    ///     finished
    ///     begin
    ///   发回的数据是te_callback(原json)，不做特殊处理
    /// 回调函数可以适当重写
    /// </example>
    public abstract class HotfixController : BaseController, IInfoEntrance
    {
        /// <summary>
        /// HTML文档
        /// </summary>
        public Bootstrap HtmlDocument { get; set; }
        
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="data"></param>
        public override async Task Receive(string data)
        {
            if (data == "finished")
            {
                HandleFinished();
                SetIsBusy(false);
            }
            else if (data == "begin")
            {
                HandleStart();
                SetIsBusy(true, "信息查询中……");
            }
            else if (data.StartsWith("post;"))
            {
                var ops = data.Split(new char[] { ';' }, 3);
                string ret;

                try
                {
                    ret = await Core.App.Service.Post(ops[1], ops[2]);
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.Timeout)
                    {
                        SetIsBusy(false);
                        await ShowMessage("错误", "连接超时，请重试。");
                        return;
                    }
                    else
                    {
                        throw ex;
                    }
                }

                HandlePostReturnValue(ops, ref ret);
                Evaluate("te_callback(" + ret + ")");
            }
            else if (data.StartsWith("msg;"))
            {
                var ops = data.Split(new char[] { ';' }, 2);
                HandleMessageValue(ops, ref ops[1]);
                await ShowMessage("消息", ops[1]);
            }
            else
            {
                Core.Log("Unknown response: " + data);
                Core.Log("You can rewrite Receive(string) as you needed.");
            }
        }

        protected virtual void HandlePostReturnValue(string[] ops, ref string ans) { }
        protected virtual void HandleMessageValue(string[] ops, ref string ans) { }
        protected virtual void HandleFinished() { }
        protected virtual void HandleStart() { }
    }
}
