using System.Collections.Generic;
using HandSchool.Internals;
using System.Linq;
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
        /// 调用源
        /// </summary>
        private object CallSite { get; }
        
        /// <summary>
        /// 如果设置的类型为 Value，其对应的设置的运行时信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// 如果设置的类型为 Action，其调用的方法的运行时信息
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// 设置项的值设置，通过反射进行获取和修改
        /// </summary>
        public object Value
        {
            get => PropertyInfo.GetValue(CallSite);
            set => PropertyInfo.SetValue(CallSite, value);
        }

        /// <summary>
        /// 执行操作。
        /// </summary>
        public void ExecuteAction()
        {
            MethodInfo.Invoke(CallSite, new object[] { });
        }

        /// <summary>
        /// 从某个具体对象中寻找设置信息。
        /// </summary>
        /// <param name="header">组标题</param>
        /// <param name="obj">具体对象</param>
        /// <returns>设置集合</returns>
        public static IEnumerable<SettingWrapper> From(object obj)
        {
            var type = obj.GetType();
            var props = type.GetProperties(BindingFlags.Public);
            var voids = type.GetMethods(BindingFlags.Public);

            return (
                from prop in props
                where prop.Has<SettingsAttribute>()
                select new SettingWrapper(obj, prop)
            ).Union(
                from @void in voids
                where @void.Has<SettingsAttribute>()
                select new SettingWrapper(obj, @void)
            );
        }

        /// <summary>
        /// 从某个抽象类型中寻找设置信息。
        /// </summary>
        /// <param name="header">组标题</param>
        /// <returns>设置集合</returns>
        public static IEnumerable<SettingWrapper> From<T>()
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Static);
            var voids = type.GetMethods(BindingFlags.Static);

            return (
                from prop in props
                where prop.Has<SettingsAttribute>()
                select new SettingWrapper(null, prop)
            ).Union(
                from @void in voids
                where @void.Has<SettingsAttribute>()
                select new SettingWrapper(null, @void)
            );
        }

        /// <summary>
        /// 是否为静态成员
        /// </summary>
        public bool IsStatic { get; }

        /// <summary>
        /// 设置的值内容修改包装。
        /// </summary>
        /// <param name="src">数据源</param>
        /// <param name="pInfo">属性的信息。</param>
        public SettingWrapper(object src, PropertyInfo pInfo)
        {
            CallSite = src;
            PropertyInfo = pInfo;
            AttributeData = pInfo.Get<SettingsAttribute>();
            IsStatic = pInfo.GetMethod.IsStatic;

            if (!PropertyInfo.CanWrite)
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
        /// <param name="src">数据源</param>
        /// <param name="mInfo">方法的信息。</param>
        public SettingWrapper(object src, MethodInfo mInfo)
        {
            CallSite = src;
            MethodInfo = mInfo;
            IsStatic = mInfo.IsStatic;
            AttributeData = mInfo.Get<SettingsAttribute>();
            Type = SettingTypes.Action;
        }
    }
}
