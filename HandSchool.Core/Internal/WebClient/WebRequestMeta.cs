using System.Collections.Generic;

namespace HandSchool.Internals
{
    /// <summary>
    /// 网络消息的请求。
    /// </summary>
    public class WebRequestMeta
    {
        public const string Json = "application/json";
        public const string Plain = "text/plain";
        public const string Xml = "text/xml";
        public const string All = "*/*";

        /// <summary>
        /// 创建请求的基本信息。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="accept">接受类型</param>
        public WebRequestMeta(string url, string accept)
        {
            Url = url;
            Accept = accept;
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// 使用的头信息
        /// </summary>
        internal IDictionary<string, string> Headers { get; }

        /// <summary>
        /// 请求的地址
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// 接受的MIME类型
        /// </summary>
        public string Accept { get; }

        /// <summary>
        /// 设置请求头。
        /// </summary>
        /// <param name="name">键名</param>
        /// <param name="value">键值</param>
        public void SetHeader(string name, string value)
        {
            if (Headers.ContainsKey(name))
                Headers.Remove(name);
            Headers.Add(name, value);
        }
    }
}