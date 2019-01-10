using HandSchool.Internal.HtmlObject;
using HandSchool.Services;
using System.Net;
using System.Threading.Tasks;
using HandSchool.Internal;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 将数据和逻辑处理交给热更新的 JavaScript 进行处理。
    /// </summary>
    /// <example>
    /// 在这里我们对 JavaScript 做几个约定：
    /// 
    ///   [1] 网页发送的数据将被传送至 Receive(data) 中处理。
    ///     (1) finished
    ///     (2) begin
    ///     (3) post;发送的地址;发送的数据
    ///     (4) msg;显示的消息
    ///     (4) ask;询问的消息;询问回调内容
    /// 
    ///   [2] 发送给网页的数据通过 Evaluate(data) 传递。
    ///     (1) 推荐回调函数名：te_callback
    ///     (2) 可以使用其他的 JavaScript 函数，例如 jQuery 或者其他内容。
    /// 
    ///   [3] 回调函数可以适当重写，达到更好的条件控制。
    ///     将 await base.Receive(data); 放在自己重写内容的后面。
    /// </example>
    /// <inheritdoc cref="BaseController" />
    /// <inheritdoc cref="IInfoEntrance" />
    public abstract class HotfixController : BaseController, IInfoEntrance
    {
        public Bootstrap HtmlDocument { get; set; }
        
        /// <summary>
        /// JavaScript 发送的内容的接收函数。
        /// </summary>
        /// <param name="data">发送的数据内容。</param>
        public override async Task Receive(string data)
        {
            if (data == "finished")
            {
                HandleFinished();
                IsBusy = false;
            }
            else if (data == "begin")
            {
                HandleStart();
                IsBusy = true;
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
                        IsBusy = false;
                        await this.ShowTimeoutMessage();
                        return;
                    }
                    else
                    {
                        throw;
                    }
                }

                HandlePostReturnValue(ops, ref ret);
                Evaluate("te_callback(" + ret + ")");
            }
            else if (data.StartsWith("msg;"))
            {
                var ops = data.Split(new char[] { ';' }, 2);
                HandleMessageValue(ref ops[1]);
                await RequestMessageAsync("消息", ops[1], "好的");
            }
            else if (data.StartsWith("ask;"))
            {
                var ops = data.Split(new char[] { ';' }, 3);
                HandleMessageValue(ref ops[1]);
                if (await RequestAnswerAsync("消息", ops[1], "取消", "确定"))
                    Evaluate("te_callback(" + ops[2] + ")");
            }
            else
            {
                this.WriteLog("Unknown response: <<<EOF\n" + data + "\nEOF; " +
                    "You can rewrite Receive(string) as you needed.");
            }
        }

        /// <summary>
        /// 读取热更新的脚本内容。
        /// </summary>
        public virtual string GetContent()
        {
            return HotfixAttribute.ReadContent(this) ?? "invokeCSharpAction('msg;模块热更新出现问题，请重启应用尝试。')";
        }

        /// <summary>
        /// 处理 POST 操作的返回值。
        /// </summary>
        /// <param name="ops">JavaScript 提供的数据。</param>
        /// <param name="ans">POST 操作返回的具体值。</param>
        protected virtual void HandlePostReturnValue(string[] ops, ref string ans) { }

        /// <summary>
        /// 处理要显示的消息。
        /// </summary>
        /// <param name="msg">要显示的消息内容。</param>
        protected virtual void HandleMessageValue(ref string msg) { }

        /// <summary>
        /// 当操作进程结束时执行自定义操作。
        /// </summary>
        protected virtual void HandleFinished() { }

        /// <summary>
        /// 当操作进程开始时执行自定义操作。
        /// </summary>
        protected virtual void HandleStart() { }
    }
}
