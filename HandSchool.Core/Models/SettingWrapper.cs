using HandSchool.Internal;
using System.Reflection;

namespace HandSchool.Models
{
    /// <summary>
    /// 设置条目的包装，提供了调用和属性设置。
    /// </summary>
    public sealed class SettingWrapper
    {
        /// <summary>
        /// 在 XAML 中提供，绑定自己使用。
        /// </summary>
        public SettingWrapper Self => this;

        /// <summary>
        /// 在动作模式时占位的一个属性值。
        /// </summary>
        public static string ActionPlaceHolder { get; set; } = "";

        /// <summary>
        /// 如果设置的类型为 Action，其对应的设置的运行时信息
        /// </summary>
        public PropertyInfo Information { get; }

        /// <summary>
        /// 如果设置的类型为 Action，其调用的方法的运行时信息
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// 可用设置项的元数据，指示了设置项标题、描述与限制等
        /// </summary>
        public SettingsAttribute AttributeData { get; }

        /// <summary>
        /// 设置项的标题
        /// </summary>
        public string Title => AttributeData.Title;

        /// <summary>
        /// 设置项的描述
        /// </summary>
        public string Description => AttributeData.Description;

        /// <summary>
        /// 设置项的类型
        /// </summary>
        public SettingTypes Type { get; }

        /// <summary>
        /// 设置项的值设置，通过反射进行获取和修改
        /// </summary>
        public object Value
        {
            get => Information.GetValue(Core.App.Service);
            set => Information.SetValue(Core.App.Service, value);
        }

        /// <summary>
        /// 是否为静态成员
        /// </summary>
        public bool IsStatic { get; }

        /// <summary>
        /// 设置的值内容修改包装。
        /// </summary>
        /// <param name="pinfo">属性的信息。</param>
        public SettingWrapper(PropertyInfo pInfo)
        {
            Information = pInfo;
            AttributeData = pInfo.Get<SettingsAttribute>();
            IsStatic = pInfo.GetMethod.IsStatic;

            if (!Information.CanWrite)
                Type = SettingTypes.Const;
            else if (pInfo.PropertyType == typeof(int))
                Type = SettingTypes.Integer;
            else if (pInfo.PropertyType == typeof(string))
                Type = SettingTypes.String;
            else if (pInfo.PropertyType == typeof(bool))
                Type = SettingTypes.Boolean;
            else
                Type = SettingTypes.Unknown;
        }

        /// <summary>
        /// 设置的点击入口点包装。
        /// </summary>
        /// <param name="mInfo">方法的信息。</param>
        public SettingWrapper(MethodInfo mInfo)
        {
            Information = typeof(SettingWrapper).GetProperty(nameof(ActionPlaceHolder));
            MethodInfo = mInfo;
            IsStatic = mInfo.IsStatic;
            AttributeData = mInfo.Get<SettingsAttribute>();
            Type = SettingTypes.Action;

            ExcuteAction = new CommandAction(() =>
            {
                MethodInfo.Invoke(Core.App.Service, new object[] { });
            });
        }

        public CommandAction ExcuteAction { get; }
    }
}
