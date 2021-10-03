﻿using HandSchool.Internals;
using HandSchool.Views;
using Microcharts;
using System;
using System.Threading.Tasks;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 提供了视图模型的基类。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    /// <inheritdoc cref="IViewResponse" />
    public class BaseViewModel : NotifyPropertyChanged, IViewResponse, IBusySignal
    {
        bool isBusy = false;
        string title = string.Empty;
        
        /// <summary>
        /// 视图模型是否处于忙碌状态。
        /// </summary>
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value, nameof(IsBusy));
        }

        /// <summary>
        /// 视图显示的窗口标题。
        /// </summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        #region IViewResponse 实现
        
        public IViewResponse View { get; set; }
        
        public Task<string> RequestActionAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            if (View is null) return Task.FromResult<string>(null);
            return Core.Platform.EnsureOnMainThread(() => View.RequestActionAsync(title, cancel, destruction, buttons));
        }
        
        public Task<bool> RequestAnswerAsync(string title, string description, string cancel, string accept)
        {
            if (View is null) return Task.FromResult(false);
            return Core.Platform.EnsureOnMainThread(() => View.RequestAnswerAsync(title, description, cancel, accept));
        }

        public Task<string> RequestInputAsync(string title, string description, string cancel, string accept)
        {
            if (View is null) return Task.FromResult<string>(null);
            return Core.Platform.EnsureOnMainThread(() => View.RequestInputAsync(title, description, cancel, accept));
        }
        public Task<string> RequestInputWithPicAsync(string title, string description, string cancel, string accept,byte[]sources)
        {
            if (View is null) return Task.FromResult<string>(null);
            return Core.Platform.EnsureOnMainThread(() => View.RequestInputWithPicAsync(title, description, cancel, accept,sources));
        }

        public Task RequestMessageAsync(string title, string message, string button = "知道了")
        {
            if (View is null) return Task.CompletedTask;
            return Core.Platform.EnsureOnMainThread(() => View.RequestMessageAsync(title, message, button));
        }

        public Task RequestChartAsync(Chart chart, string title = "", string close = "关闭")
        {
            if (View is null) return Task.CompletedTask;
            return Core.Platform.EnsureOnMainThread(() => View.RequestChartAsync(chart, title, close));
        }
        public Task<string> RequestWebDialogAsync(string title, string description, string url, string cancel, string accept, bool navigation, bool hasInput, string inputHint, WebDialogAdditionalArgs additionalArgs)
        {
            if (View is null) return Task.FromResult<string>(null);
            return Core.Platform.EnsureOnMainThread(() => View.RequestWebDialogAsync(title, description, url, cancel, accept, navigation, hasInput, inputHint, additionalArgs));
        }

        /// <summary>
        /// 通知错误信息，并且将IsBusy属性置为false
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public Task NoticeError(string error)
        {
            var res = RequestMessageAsync("错误", error, "好");
            IsBusy = false;
            return res;
        }

        #endregion
    }
}