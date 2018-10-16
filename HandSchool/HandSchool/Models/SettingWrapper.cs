using HandSchool.Internal;
using HandSchool.Services;
using System.Reflection;

namespace HandSchool.Models
{
    /// <summary>
    /// 设置中心的条目类型。
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
    /// 设置条目的包装，提供了调用和属性设置。
    /// </summary>
    public sealed class SettingWrapper
    {
        /// <summary>
        /// 绑定自己使用
        /// </summary>
        public SettingWrapper Self => this;

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
        /// 设置的值内容修改包装。
        /// </summary>
        /// <param name="pinfo">属性的信息。</param>
        public SettingWrapper(PropertyInfo pinfo)
        {
            Infomation = pinfo;
            AttributeData = pinfo.GetSettingsAttribute();

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
        /// 设置的点击入口点包装。
        /// </summary>
        /// <param name="mInfo">方法的信息。</param>
        public SettingWrapper(MethodInfo mInfo)
        {
            Infomation = typeof(SettingWrapper).GetProperty(nameof(ActionPlaceHolder));
            MethodInfo = mInfo;
            AttributeData = mInfo.GetSettingsAttribute();
            Type = SettingTypes.Action;
        }
    }
}
