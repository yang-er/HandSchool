using System;

namespace HandSchool.Internals
{
    /// <summary>
    /// 注册服务，表示此服务可能会被用到。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class RegisterServiceAttribute : Attribute
    {
        /// <summary>
        /// 注册的服务类型
        /// </summary>
        public Type RegisterType { get; }

        /// <summary>
        /// 注册服务，表示此服务可能会被用到。
        /// </summary>
        /// <param name="registerType">注册的服务类型。</param>
        public RegisterServiceAttribute(Type registerType)
        {
            RegisterType = registerType;
        }
    }
}
