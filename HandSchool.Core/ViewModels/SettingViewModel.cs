﻿using HandSchool.Internals;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 系统设置的视图模型，提供了教务系统服务的设置。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    public sealed class SettingViewModel : BaseViewModel
    {
        static readonly Lazy<SettingViewModel> Lazy = 
            new Lazy<SettingViewModel>(() => new SettingViewModel());
        public static event Func<Task> OnResetSettings;

        public static event Func<Task> OnSaveSettings;
        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static SettingViewModel Instance => Lazy.Value;

        /// <summary>
        /// 关于应用的视图模型
        /// </summary>
        public AboutViewModel AboutViewModel => AboutViewModel.Instance;

        /// <summary>
        /// 所有设置入口点的包装
        /// </summary>
        public List<SettingWrapper> Items { get; }

        /// <summary>
        /// 保存设置的命令
        /// </summary>
        public ICommand SaveConfigures { get; }

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
                where prop.Has<SettingsAttribute>()
                select new SettingWrapper(prop)
            ).Union(
                from @void in voids
                where @void.Has<SettingsAttribute>()
                select new SettingWrapper(@void)
            ).ToList();
            Items.Add(new SettingWrapper(GetType().GetMethod(nameof(ResetSettings))));
           
            //Items.Add(new SettingWrapper(GetType().GetMethod(nameof(TestBindingCounts))));

            SaveConfigures = new CommandAction(async () =>
            {
                Core.App.Loader.SaveSettings(Core.App.Service);
                await (OnSaveSettings?.Invoke() ?? Task.CompletedTask);
                await RequestMessageAsync("设置中心", "保存成功\n但需要结束当前进程重启后生效！", "好的");
            });
        }
        
        /// <summary>
        /// 清除数据的功能
        /// </summary>
        [Settings("清除数据", "将应用数据清空，恢复到默认状态。")]
        public static async void ResetSettings()
        {
            if (!await Instance.RequestAnswerAsync("清除数据", "确定要清除数据吗？", "取消", "确认")) return;

            foreach (var fileName in Core.App.Loader.RegisteredFiles)
                Core.Configure.Remove(fileName);
            Core.Configure.Remove(Core.ConfigSchool);
            Core.Configure.Remove(ConfigurationManager.ConfigPath);
            Core.App.Service.ResetSettings();
            await (OnResetSettings?.Invoke() ?? Task.CompletedTask);
            await Instance.RequestMessageAsync("清除数据", "重置应用成功！重启应用后生效。", "好的");
        }

        /// <summary>
        /// 测试功能
        /// </summary>
        [Settings("测试", "检测绑定数目。")]
        public static async void TestBindingCounts()
        {
            await Instance.RequestMessageAsync("绑定数目", Instance.GetEventAttached() + "个", "好的");
        }
    }
}
