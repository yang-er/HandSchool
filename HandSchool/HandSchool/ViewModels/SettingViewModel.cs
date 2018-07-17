using HandSchool.Models;
using HandSchool.Services;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Command = Xamarin.Forms.Command;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 系统设置的 ViewModel
    /// </summary>
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
        public Command SaveConfigures { get; }

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
            SaveConfigures = new Command(Core.App.Service.SaveSettings);
        }
    }
}
