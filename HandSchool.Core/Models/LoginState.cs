namespace HandSchool.Models
{
    /// <summary>
    /// 表示目前登录状态的枚举。
    /// </summary>
    public enum LoginState
    {
        /// <summary>
        /// 正在登录
        /// </summary>
        Processing,

        /// <summary>
        /// 登录成功
        /// </summary>
        Succeeded,

        /// <summary>
        /// 登录失败
        /// </summary>
        Failed
    }
}
