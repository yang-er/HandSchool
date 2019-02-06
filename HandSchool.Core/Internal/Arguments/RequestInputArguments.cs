using System.ComponentModel;
using System.Threading.Tasks;

namespace HandSchool.Internals
{
    /// <summary>
    /// 请求输入内容的参数列表。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RequestInputArguments
    {
        /// <summary>
        /// 创建一个请求输入内容的实例，并初始化内容。
        /// </summary>
        public RequestInputArguments()
        {
            Result = new TaskCompletionSource<string>();
        }
        
        /// <summary>
        /// 创建一个请求输入内容的实例，并初始化内容。
        /// </summary>
        public RequestInputArguments(string tit, string msg, string no, string ok) : this()
        {
            Title = tit;
            Message = msg;
            Cancel = no;
            Accept = ok;
        }

        /// <summary>
        /// 接受按钮
        /// </summary>
        public string Accept { get; set; }
        
        /// <summary>
        /// 取消按钮
        /// </summary>
        public string Cancel { get; set; }
        
        /// <summary>
        /// 消息内容，表示要输入的内容的描述
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 表示结果的异步源
        /// </summary>
        public TaskCompletionSource<string> Result { get; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 设置结果，并返回RequestInputAsync。
        /// </summary>
        /// <param name="result">内容</param>
        public void SetResult(string result)
        {
            Result.TrySetResult(result);
        }
    }
}
