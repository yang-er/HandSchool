using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 关于界面的视图模型。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    public class AboutViewModel : BaseViewModel
    {
        static readonly Lazy<AboutViewModel> Lazy =
            new Lazy<AboutViewModel>(() => new AboutViewModel());

        /// <summary>
        /// 信息入口点的列表
        /// </summary>
        public List<InfoEntranceGroup> InfoEntrances { get; }

        /// <summary>
        /// 目前程序的版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 视图模型的单个实例
        /// </summary>
        public static AboutViewModel Instance => Lazy.Value;

        /// <summary>
        /// 创建一个关于界面的视图模型。
        /// </summary>
        private AboutViewModel()
        {
            var ent = new InfoEntranceGroup { GroupTitle = "关于" };
            ent.Add(new TapEntranceWrapper("开源项目", "", (nav) => Task.Run(() => OpenSource())));
            if (Core.Platform.RuntimeName != "iOS")
                ent.Add(new TapEntranceWrapper("检查更新", "", (nav) => Task.Run(() => CheckUpdate())));
            ent.Add(new TapEntranceWrapper("软件评分", "", (nav) => Task.Run(() => OpenMarket())));
            ent.Add(new InfoEntranceWrapper(typeof(PrivacyPolicy)));
            ent.Add(new InfoEntranceWrapper(typeof(LicenseInfo)));

            InfoEntrances = new List<InfoEntranceGroup> { ent };
            Version = Core.Version;
            Title = "关于";
        }

        /// <summary>
        /// 打开本软件的开源项目页面。
        /// </summary>
        private void OpenSource() => Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool");

        /// <summary>
        /// 在应用商店中打开本软件详情。
        /// </summary>
        private void OpenMarket() => Core.Platform.OpenUrl(Core.Platform.StoreLink);

        /// <summary>
        /// 检查软件更新。
        /// </summary>
        private void CheckUpdate() => Core.Platform.CheckUpdate();

        [Entrance("*", "隐私政策", "提供关于本程序如何使用您的隐私的一些说明。", EntranceType.UrlEntrance)]
        public class PrivacyPolicy : BaseController, IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "privacy.html";
            public override Task Receive(string data) { return Task.Run(() => { }); }
            public byte[] OpenWithPost => null;
            public List<string> Cookie => null;
            public IUrlEntrance SubUrlRequested(string sub) { throw new InvalidOperationException(); }
        }

        [Entrance("*", "开放源代码许可", "提供关于本程序开源许可证的一些说明。", EntranceType.UrlEntrance)]
        public class LicenseInfo : BaseController, IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "license.html";
            public override Task Receive(string data) { return Task.Run(() => { }); }
            public byte[] OpenWithPost => null;
            public List<string> Cookie => null;
            public IUrlEntrance SubUrlRequested(string sub) { throw new InvalidOperationException(); }
        }
    }
}
