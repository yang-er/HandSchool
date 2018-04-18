using HandSchool.Internal;
using HandSchool.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            Title = "设置中心";
            var type = Core.App.Service.GetType();
            var props = type.GetProperties();
            Items = (
                from prop in props
                where prop.GetCustomAttribute(typeof(SettingsAttribute)) != null
                select new SettingWrapper(prop)
            ).ToList();
        }
    }
}
