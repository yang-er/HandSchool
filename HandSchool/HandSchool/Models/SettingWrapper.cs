using HandSchool.Services;
using System.Reflection;

namespace HandSchool.Models
{
    /// <summary>
    /// 设置类型
    /// </summary>
    public enum SettingTypes
    {
        /// <summary>
        /// 未知类型
        /// </summary>
        Unkown,

        /// <summary>
        /// 数字类型
        /// </summary>
        Integer,

        /// <summary>
        /// 文本类型
        /// </summary>
        String,

        /// <summary>
        /// 常量说明
        /// </summary>
        Const,

        /// <summary>
        /// 开关类型
        /// </summary>
        Boolean,

        /// <summary>
        /// 行为类型
        /// </summary>
        Action,
    }
    
    /// <summary>
    /// 设置包装
    /// </summary>
    public sealed class SettingWrapper
    {
        /// <summary>
        /// 在动作模式时占位
        /// </summary>
        public static string ActionPlaceHolder { get; set; } = "";

        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo Infomation { get; }

        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// 设置元数据
        /// </summary>
        public SettingsAttribute AttributeData { get; }

        /// <summary>
        /// 设置项标题
        /// </summary>
        public string Title => AttributeData.Title;

        /// <summary>
        /// 设置项描述
        /// </summary>
        public string Description => AttributeData.Description;

        /// <summary>
        /// 设置项类型
        /// </summary>
        public SettingTypes Type { get; }

        /// <summary>
        /// 设置项值
        /// </summary>
        public object Value
        {
            get => Infomation.GetValue(Core.App.Service);
            set => Infomation.SetValue(Core.App.Service, value);
        }

        /// <summary>
        /// 设置包装
        /// </summary>
        /// <param name="pinfo">属性信息</param>
        public SettingWrapper(PropertyInfo pinfo)
        {
            Infomation = pinfo;
            AttributeData = pinfo.GetCustomAttribute(typeof(SettingsAttribute)) as SettingsAttribute;
            System.Diagnostics.Debug.Assert(AttributeData != null, "Error setting here.");

            if (AttributeData.RangeDown == -233)
                Type = SettingTypes.Const;
            else if (pinfo.PropertyType == typeof(int))
                Type = SettingTypes.Integer;
            else if (pinfo.PropertyType == typeof(string))
                Type = SettingTypes.String;
            else if (pinfo.PropertyType == typeof(bool))
                Type = SettingTypes.Boolean;
            else
                Type = SettingTypes.Unkown;
        }

        /// <summary>
        /// 设置入口点包装
        /// </summary>
        /// <param name="mInfo">方法信息</param>
        public SettingWrapper(MethodInfo mInfo)
        {
            Infomation = typeof(SettingWrapper).GetProperty(nameof(ActionPlaceHolder));
            MethodInfo = mInfo;
            AttributeData = mInfo.GetCustomAttribute(typeof(SettingsAttribute)) as SettingsAttribute;
            System.Diagnostics.Debug.Assert(AttributeData != null, "Error setting here.");
            Type = SettingTypes.Action;
        }
    }
}
