using System;
using HandSchool.Services;

namespace HandSchool.Internal
{
    /// <summary>
    /// 可用的应用设置项。在 <see cref="ISchoolSystem"/> 中给属性添加此特性，可以在应用设置中显示。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = false)]
    public sealed class SettingsAttribute : Attribute
    {
        /// <summary>
        /// 在应用设置中显示的设置项或按钮入口信息特性。
        /// </summary>
        /// <param name="title">设置项标题。</param>
        /// <param name="description">设置项描述。</param>
        /// <param name="down">设置项数字下界，-233为纯文本提示。</param>
        /// <param name="up">设置项数字上界。</param>
        public SettingsAttribute(string title, string description, int down = 0, int up = 0)
        {
            Title = title;
            Description = description;
            RangeDown = down;
            RangeUp = up;
        }

        /// <summary>
        /// 设置项标题，在对应的控件上方展示。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 设置项描述，在对应的控件下方展示。
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 如果属性为 Integer 类型，可用的数字下界。
        /// </summary>
        public int RangeDown { get; }

        /// <summary>
        /// 如果属性为 Integer 类型，可用的数字上界。
        /// </summary>
        public int RangeUp { get; }
    }
}
