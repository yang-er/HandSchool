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

    /// <summary>
    /// 请求输入内容的参数列表。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RequestInputWithPicArguments
    {
        /// <summary>
        /// 创建一个请求输入内容的实例，并初始化内容。
        /// </summary>
        public RequestInputWithPicArguments()
        {
            Result = new TaskCompletionSource<string>();
        }

        /// <summary>
        /// 创建一个请求输入内容的实例，并初始化内容。
        /// </summary>
        public RequestInputWithPicArguments(string tit, string msg, string no, string ok, byte[] sources) : this()
        {
            Title = tit;
            Message = msg;
            Cancel = no;
            Accept = ok;
            Sources = sources;
        }

        public byte[] Sources { get; set; }
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

    public class RequestWebDialogArguments
    {
        /// <summary>
        /// 创建一个请求输入内容的实例，并初始化内容。
        /// </summary>
        public RequestWebDialogArguments()
        {
            Result = new TaskCompletionSource<string>();
        }

        public RequestWebDialogArguments(string tit, string msg, string url, string no, string ok,bool navigation, bool hasInput, string inputHint) : this()
        {
            Title = tit;
            Message = msg;
            Cancel = no;
            Accept = ok;
            Url = url; 
            HasInput = hasInput;
            Navigation = navigation;
            InputHint = inputHint;
        }
        public bool HasInput { get; set; }
        public bool Navigation { get; set; }
        public string InputHint { get; set; }
        public string Url { get; set; }
        public string Accept { get; set; }

        public string Cancel { get; set; }

        public string Message { get; set; }

        public TaskCompletionSource<string> Result { get; }
        public string Title { get; set; }
        public void SetResult(string result)
        {
            Result.TrySetResult(result);
        }
    }

    public class WebDialogAdditionalArgs
    {
        public System.Collections.Generic.IEnumerable<System.Net.Cookie> Cookies { get; set; }
    }


}
