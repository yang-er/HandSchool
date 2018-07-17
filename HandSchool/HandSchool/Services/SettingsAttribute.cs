using System;

namespace HandSchool.Services
{
    /// <summary>
    /// 可用的应用设置项
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SettingsAttribute : Attribute
    {
        /// <summary>
        /// 可用的应用设置项
        /// </summary>
        /// <param name="title">设置项标题</param>
        /// <param name="description">设置项描述</param>
        /// <param name="down">设置项数字下界，-233为纯文本提示</param>
        /// <param name="up">设置项数字上界</param>
        public SettingsAttribute(string title, string description, int down = 0, int up = 0)
        {
            Title = title;
            Description = description;
            RangeDown = down;
            RangeUp = up;
        }

        /// <summary>
        /// 设置项标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 设置项描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 设置项数字下界
        /// </summary>
        public int RangeDown { get; private set; }

        /// <summary>
        /// 设置项数字上界
        /// </summary>
        public int RangeUp { get; private set; }
    }
}
