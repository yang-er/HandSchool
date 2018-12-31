using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HandSchool.Models
{
    /// <summary>
    /// 基础的登录表单，提供了准备接口和登录接口。
    /// </summary>
    public interface ILoginField : INotifyPropertyChanged
    {
        /// <summary>
        /// 登录表单的用户名
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// 登录表单的密码
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// 登录表单的提示（例如默认密码）
        /// </summary>
        string Tips { get; }

        /// <summary>
        /// 是否已登录
        /// </summary>
        bool IsLogin { get; }

        /// <summary>
        /// 是否需要登录
        /// </summary>
        bool NeedLogin { get; }

        /// <summary>
        /// 表单的名称
        /// </summary>
        string FormName { get; }

        /// <summary>
        /// 登录操作，通过网络进行登录操作。
        /// </summary>
        /// <returns>是否登录成功。</returns>
        Task<bool> Login();

        /// <summary>
        /// 登录之前进行准备工作，例如加载验证码等内容。
        /// </summary>
        /// <returns>是否准备成功。</returns>
        Task<bool> PrepareLogin();

        /// <summary>
        /// 验证码内容
        /// </summary>
        string CaptchaCode { get; set; }

        /// <summary>
        /// 验证码源 image/jpeg
        /// </summary>
        byte[] CaptchaSource { get; }

        /// <summary>
        /// 是否自动登录
        /// </summary>
        bool AutoLogin { get; set; }

        /// <summary>
        /// 是否保存密码
        /// </summary>
        bool SavePassword { get; set; }
        
        /// <summary>
        /// 登录状态改变的事件
        /// </summary>
        event EventHandler<LoginStateEventArgs> LoginStateChanged;
    }
}
