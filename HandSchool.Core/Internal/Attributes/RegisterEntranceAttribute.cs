using System;

namespace HandSchool.Internal
{
    /// <summary>
    /// 注册服务，表示此服务可能会被用到。
    /// </summary>
    [Obsolete]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class RegisterEntranceAttribute : Attribute
    {
        /// <summary>
        /// 注册的入口类型
        /// </summary>
        public Type RegisterType { get; }

        /// <summary>
        /// 注册入口，表示此服务可能会被用到。
        /// </summary>
        /// <param name="registerType">注册的入口类型。</param>
        public RegisterEntranceAttribute(Type registerType)
        {
            RegisterType = registerType;
        }
    }
}
