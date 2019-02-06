namespace HandSchool.Internals
{
    public enum WebStatus
    {
        /// <summary>
        /// 请求一切正常
        /// </summary>
        Success = 0,
        
        /// <summary>
        /// 域名解析失败
        /// </summary>
        NameResolutionFailure = 1,
        
        /// <summary>
        /// 连接服务器失败
        /// </summary>
        ConnectFailure = 2,
        
        /// <summary>
        /// 接收数据失败
        /// </summary>
        ReceiveFailure = 3,

        /// <summary>
        /// 协议出错***
        /// </summary>
        ProtocolError = 7,

        /// <summary>
        /// SSL连接错误
        /// </summary>
        SecureChannelFailure = 10,

        /// <summary>
        /// 网络沟通错误
        /// </summary>
        ServerProtocolViolation = 11,

        /// <summary>
        /// 连接超时
        /// </summary>
        Timeout = 14,

        /// <summary>
        /// 未知错误
        /// </summary>
        UnknownError = 16,

        /// <summary>
        /// 返回类型不匹配
        /// </summary>
        MimeNotMatch = 21,
    }
}