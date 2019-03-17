using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HandSchool.Internals
{
    /// <summary>
    /// 网络消息的响应。
    /// </summary>
    public interface IWebResponse : IDisposable
    {
        /// <summary>
        /// 发送的请求
        /// </summary>
        WebRequestMeta Request { get; }

        /// <summary>
        /// 状态码
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// 302的跳转地址
        /// </summary>
        string Location { get; }

        /// <summary>
        /// 内容类型
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// 请求的状态
        /// </summary>
        WebStatus Status { get; }

        /// <summary>
        /// 将结果作为字符串读取。
        /// </summary>
        /// <returns>结果字符串</returns>
        Task<string> ReadAsStringAsync();

        /// <summary>
        /// 将结果作为字节数组读取。
        /// </summary>
        /// <returns>结果字节数组</returns>
        Task<byte[]> ReadAsByteArrayAsync();

        /// <summary>
        /// 将结果作为流对象写入文件。
        /// </summary>
        /// <param name="path">文件路径</param>
        Task WriteToFileAsync(string path);

        /// <summary>
        /// 获得响应头。
        /// </summary>
        /// <returns>响应头</returns>
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetHeaders();
    }
}