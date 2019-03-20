using System;

namespace HandSchool.Internals
{
    /// <summary>
    /// 由 <see cref="IWebClient"/> 抛出的异常。
    /// </summary>
    public class WebsException : Exception
    {
        /// <summary>
        /// 创建一个网络异常实例。
        /// </summary>
        /// <param name="response">响应信息</param>
        /// <param name="innerException">内部异常信息</param>
        public WebsException(IWebResponse response, Exception innerException)
            : base(response.Status.ToDescription(), innerException)
        {
            Response = response;
            Request = response.Request;
            Status = response.Status;
        }

        /// <summary>
        /// 创建一个网络异常实例。
        /// </summary>
        /// <param name="response">响应信息</param>
        public WebsException(IWebResponse response) : base(response.Status.ToDescription())
        {
            Response = response;
            Request = response.Request;
            Status = response.Status;
        }

        /// <summary>
        /// 创建一个网络异常实例。
        /// </summary>
        /// <param name="status">状态信息</param>
        public WebsException(WebStatus status) : base(status.ToDescription())
        {
            Status = status;
        }

        /// <summary>
        /// 创建一个网络异常实例。
        /// </summary>
        /// <param name="status">状态信息</param>
        public WebsException(IWebResponse response, WebStatus status) : base(status.ToDescription())
        {
            Response = response;
            Request = response.Request;
            Status = status;
        }

        /// <summary>
        /// 创建一个网络异常实例。
        /// </summary>
        /// <param name="status">状态信息</param>
        public WebsException(string info, WebStatus status) : base(info)
        {
            Status = status;
        }

        /// <summary>
        /// 消息回应
        /// </summary>
        public IWebResponse Response { get; }

        /// <summary>
        /// 消息请求
        /// </summary>
        public WebRequestMeta Request { get; }

        /// <summary>
        /// 消息状态
        /// </summary>
        public WebStatus Status { get; }
    }
}