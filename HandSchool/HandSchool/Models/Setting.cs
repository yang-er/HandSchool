using HandSchool.Internal;
using System;
using System.ComponentModel;
using System.Reflection;

namespace HandSchool
{
    namespace Models
    {
        public enum SettingTypes
        {
            Unkown,
            Integer,
            String,
        }

        public sealed class SettingWrapper<T> : SettingWrapper
        {
            public new T Value
            {
                get => (T) pInfo.GetValue(Core.App.Service);
                set => pInfo.SetValue(Core.App.Service, value);
            }

            public SettingWrapper(PropertyInfo pinfo) : base(pinfo)
            {
                if (typeof(T) == typeof(int)) Type = SettingTypes.Integer;
                else if (typeof(T) == typeof(string)) Type = SettingTypes.String;
                else Type = SettingTypes.Unkown;
            }
        }

        public class SettingWrapper
        {
            protected SettingsAttribute attrData;
            protected PropertyInfo pInfo;

            public PropertyInfo Infomation => pInfo;
            public string Title => attrData.Title;
            public string Description => attrData.Description;
            public SettingTypes Type { get; protected set; }

            public object Value
            {
                get => pInfo.GetValue(Core.App.Service);
                set => pInfo.SetValue(Core.App.Service, value);
            }

            public SettingWrapper(PropertyInfo pinfo)
            {
                pInfo = pinfo;
                attrData = pinfo.GetCustomAttribute(typeof(SettingsAttribute)) as SettingsAttribute;
                if (pinfo.PropertyType == typeof(int)) Type = SettingTypes.Integer;
                else if (pinfo.PropertyType == typeof(string)) Type = SettingTypes.String;
                else Type = SettingTypes.Unkown;
            }
        }
    }

    namespace Internal
    {
        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public sealed class SettingsAttribute : Attribute
        {
            readonly string _title;
            readonly string _description;

            public SettingsAttribute(string title, string description)
            {
                _title = title;
                _description = description;
            }

            public string Title => _title;
            public string Description => _description;
        }
    }
}
