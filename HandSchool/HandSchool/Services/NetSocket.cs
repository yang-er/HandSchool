using System;
using System.Collections.Generic;

namespace HandSchool.Services
{
    /// <summary>
    /// 网络协议
    /// </summary>
    public interface INetSocket
    {
        /// <summary>
        /// 设置认证信息
        /// </summary>
        /// <param name="infomation">数据</param>
        void SaveCredential(Dictionary<string, string> infomation);

        /// <summary>
        /// 登录并保持在线（在新线程中）
        /// </summary>
        void LoginAndKeepAlive();

        /// <summary>
        /// 发送登出信号
        /// </summary>
        void Logout();
    }
    
    /// <summary>
    /// 套接字日志委托
    /// </summary>
    /// <param name="str">字符串</param>
    public delegate void NetSocketLogDelegate(string str);

    /// <summary>
    /// 套接字状态更改委托
    /// </summary>
    /// <param name="isLogin">是否已登录</param>
    /// <param name="tip">提示</param>
    public delegate void NetSocketStateDelegate(bool isLogin, string tip = "");

    /// <summary>
    /// 网络套接字异常
    /// </summary>
    public class NetSocketException : Exception { }
}
