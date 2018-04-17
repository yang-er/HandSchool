using System;
using System.ComponentModel;

namespace HandSchool
{
    namespace Models
    {
        public interface ISettingEntrance : INotifyPropertyChanged
        {
            string StorageFile { get; }
            void Load();
            void Save();
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
