using HandSchool.Models;
using HandSchool.Services;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Command = Xamarin.Forms.Command;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 系统设置的视图模型，提供了教务系统服务的设置。
    /// </summary>
    public class SettingViewModel : BaseViewModel
    {
        private static SettingViewModel instance = null;

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static SettingViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new SettingViewModel();
                return instance;
            }
        }

        /// <summary>
        /// 所有设置入口点的包装
        /// </summary>
        public List<SettingWrapper> Items { get; }

        /// <summary>
        /// 保存设置的命令
        /// </summary>
        public Command SaveConfigures { get; }

        /// <summary>
        /// 通过反射从教务系统服务中读取所有的设置属性和方法。
        /// </summary>
        private SettingViewModel()
        {
            Title = "设置中心";
            var type = Core.App.Service.GetType();
            var props = type.GetProperties();
            var voids = type.GetMethods();

            Items = (
                from prop in props
                where prop.GetCustomAttribute(typeof(SettingsAttribute)) != null
                select new SettingWrapper(prop)
            ).Union(
                from @void in voids
                where @void.GetCustomAttribute(typeof(SettingsAttribute)) != null
                select new SettingWrapper(@void)
            ).ToList();
            
            SaveConfigures = new Command(async () =>
            {
                Core.App.Service.SaveSettings();
                await ShowMessage("设置中心", "保存成功！");
            });
        }
    }
}
