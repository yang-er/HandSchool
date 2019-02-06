using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.Internals
{
    public interface IWebClient : IDisposable
    {
        /// <summary>
        /// Cookie的容器
        /// </summary>
        CookieContainer Cookie { get; }

        /// <summary>
        /// 是否允许自动导航
        /// </summary>
        bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// 默认字符编码
        /// </summary>
        Encoding Encoding { get; set; }
        
        /// <summary>
        /// 网站基础地址
        /// </summary>
        string BaseAddress { get; set; }

        /// <summary>
        /// 超时时长
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// 以form-urlencoded格式进行POST。
        /// </summary>
        /// <param name="req">网络请求</param>
        /// <param name="value">发送的值</param>
        /// <returns>返回结果</returns>
        Task<IWebResponse> PostAsync(WebRequestMeta req, KeyValueDict value);

        /// <summary>
        /// 以要求的格式POST发送数据。
        /// </summary>
        /// <param name="req">网络请求</param>
        /// <param name="value">发送的值</param>
        /// <param name="contentType">值的类型</param>
        /// <returns>返回结果</returns>
        Task<IWebResponse> PostAsync(WebRequestMeta req, string value, string contentType);

        /// <summary>
        /// 以GET形式发送数据。
        /// </summary>
        /// <param name="script">网络请求</param>
        /// <returns>返回结果</returns>
        Task<IWebResponse> GetAsync(WebRequestMeta req);
    }
}