using System;

namespace HandSchool.Internal
{
    /// <summary>
    /// 内容类型不相容错误
    /// </summary>
    public class ContentAcceptException : Exception
    {
        /// <summary>
        /// 返回的内容本身
        /// </summary>
        public string Result { get; }
        
        /// <summary>
        /// 目前的返回内容类型
        /// </summary>
        public string Current { get; }
        
        /// <summary>
        /// 本应接收的内容类型
        /// </summary>
        public string Accept { get; }
        
        /// <summary>
        /// 创建内容类型不相容错误。
        /// </summary>
        /// <param name="ret">返回的内容本身。</param>
        /// <param name="cur">目前的返回内容类型。</param>
        /// <param name="acc">本应接收的内容类型。</param>
        public ContentAcceptException(string ret, string cur, string acc)
        {
            Result = ret;
            Current = cur;
            Accept = acc;
        }
        
        /// <summary>
        /// 返回异常的文字描述。
        /// </summary>
        public override string ToString()
        {
            return $"错误的类型匹配：应接收{Accept}，实接收{Current}";
        }
    }
}
