﻿using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            Version = Core.Version;
            Title = "关于";
        }

        /// <summary>
        /// 打开本软件的开源项目页面。
        /// </summary>
        public void OpenSource() => Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool");

        /// <summary>
        /// 在应用商店中打开本软件详情。
        /// </summary>
        public void OpenMarket() => Core.Platform.OpenUrl(Core.Platform.StoreLink);


        [Entrance("*", "隐私政策", "提供关于本程序如何使用您的隐私的一些说明。", EntranceType.UrlEntrance)]
        public class PrivacyPolicy : BaseController, IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "privacy.html";
            public override Task Receive(string data) { return Task.CompletedTask; }
            public IUrlEntrance SubUrlRequested(string sub) { throw new InvalidOperationException(); }
        }

        [Entrance("*", "开放源代码许可", "提供关于本程序开源许可证的一些说明。", EntranceType.UrlEntrance)]
        public class LicenseInfo : BaseController, IUrlEntrance
        {
            public string HtmlUrl { get; set; } = "license.html";
            public override Task Receive(string data) { return Task.CompletedTask; }
            public IUrlEntrance SubUrlRequested(string sub) { throw new InvalidOperationException(); }
        }
    }
}
