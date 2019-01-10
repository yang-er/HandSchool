using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Views;
using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 学校教务系统接口
    /// </summary>
    /// <inheritdoc cref="ILoginField"/>
    public interface ISchoolSystem : ILoginField
    {
        /// <summary>
        /// 服务器基地址
        /// </summary>
        string ServerUri { get; }

        /// <summary>
        /// 内部WebClient
        /// </summary>
        AwaredWebClient WebClient { get; set; }
        
        /// <summary>
        /// 当前周
        /// </summary>
        int CurrentWeek { get; set; }

        /// <summary>
        /// 欢迎信息
        /// </summary>
        string WelcomeMessage { get; }

        /// <summary>
        /// 当前周信息
        /// </summary>
        string CurrentMessage { get; }

        /// <summary>
        /// 天气位置
        /// </summary>
        string WeatherLocation { get; }

        /// <summary>
        /// POST发送数据包
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="value">发送的数据</param>
        /// <returns>接收的数据</returns>
        /// <exception cref="System.Net.WebException" />
        Task<string> Post(string url, string value);

        /// <summary>
        /// GET发送数据包
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>接收的数据</returns>
        /// <exception cref="System.Net.WebException" />
        Task<string> Get(string url);
        
        /// <summary>
        /// 用附加信息格式化字符串
        /// </summary>
        /// <param name="args">元字符串</param>
        /// <returns>格式化后的字符串</returns>
        string FormatArguments(string args);

        /// <summary>
        /// 重置设置，将应用恢复为初始状态。
        /// </summary>
        void ResetSettings();
    }
}
