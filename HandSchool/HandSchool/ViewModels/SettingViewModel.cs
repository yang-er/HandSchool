using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HandSchool.Internal;
using HandSchool.Models;

namespace HandSchool.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        private static SettingViewModel instance = null;
        public static SettingViewModel Instance
        {
            get
            {
                if (instance is null) instance = new SettingViewModel();
                return instance;
            }
        }

        public List<SettingWrapper> Items { get; }

        private SettingViewModel()
        {
            var type = Core.App.Service.GetType();
            var props = type.GetProperties();
            Items = (
                from n in props
                where n.GetCustomAttribute(typeof(SettingsAttribute)) != null
                select new SettingWrapper(n)
            ).ToList();
        }
    }
}
