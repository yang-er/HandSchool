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
            Const,
            Boolean,
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
            public SettingsAttribute AttributeData => attrData;
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
                if (attrData.RangeDown == -233) Type = SettingTypes.Const;
                else if (pinfo.PropertyType == typeof(int)) Type = SettingTypes.Integer;
                else if (pinfo.PropertyType == typeof(string)) Type = SettingTypes.String;
                else if (pinfo.PropertyType == typeof(bool)) Type = SettingTypes.Boolean;
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
            readonly int _rangeDown;
            readonly int _rangeUp;

            public SettingsAttribute(string title, string description, int down = 0, int up = 0)
            {
                _title = title;
                _description = description;
                _rangeDown = down;
                _rangeUp = up;
            }

            public string Title => _title;
            public string Description => _description;
            public int RangeDown => _rangeDown;
            public int RangeUp => _rangeUp;
        }
    }
}
