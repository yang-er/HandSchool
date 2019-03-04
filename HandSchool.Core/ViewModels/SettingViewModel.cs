using HandSchool.Internals;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <summary>
        /// 所有设置入口点的包装
        /// </summary>
        public ObservableCollection<HeadedList<SettingWrapper>> Items { get; }

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
            /*var type = Core.App.Service.GetType();
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
            // Items.Add(new SettingWrapper(GetType().GetMethod(nameof(TestBindingCounts))));*/
            Items = new ObservableCollection<HeadedList<SettingWrapper>>();
            SaveConfigures = new CommandAction(SaveConfiguresAsync);
            throw new NotImplementedException();
        }

        private async Task SaveConfiguresAsync()
        {
            Core.App.Loader.SaveSettings(Core.App.Service);
            await RequestMessageAsync("设置中心", "保存成功！", "好的");
        }
        
        /// <summary>
        /// 清除数据的功能
        /// </summary>
        [Settings("清除数据", "将应用数据清空，恢复到默认状态。")]
        public async void ResetSettings()
        {
            if (!await RequestAnswerAsync("清除数据", "确定要清除数据吗？", "取消", "确认")) return;

            foreach (var fileName in Core.App.Loader.RegisteredFiles)
                Core.Configure.Remove(fileName);
            Core.Configure.Remove("hs.school.bin");
            Core.App.Service.ResetSettings();

            await RequestMessageAsync("清除数据", "重置应用成功！重启应用后生效。", "好的");
        }

#if DEBUG
        /// <summary>
        /// 测试功能
        /// </summary>
        [Settings("测试", "检测绑定数目。")]
        public async void TestBindingCounts()
        {
            await RequestMessageAsync("绑定数目", Instance.GetEventAttached() + "个", "好的");
        }
#endif
    }
}
