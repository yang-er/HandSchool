﻿using HandSchool.Internals;
using HandSchool.Views;
using Microcharts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HandSchool.Models;

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

        protected virtual async Task<TaskResp> CheckEnv(string actionName)
        {
            var responses = await SchoolApplication.SendActioning(this, new ActioningEventArgs {ActionName = actionName});
            if(responses.Count == 0)return TaskResp.True;
            return new TaskResp(responses.All(r => r.IsSuccess), responses.FirstOrDefault(r => !r.IsSuccess).Msg);
        }
        
        #region IViewResponse 实现

        private List<IViewResponse> _views = new List<IViewResponse>();
        public IViewResponse View => _views.Count > 0 ? _views[_views.Count - 1] : null;

        public void AddView(IViewResponse v)
        {
            var index = _views.IndexOf(v);
            if (index >= 0)
            {
                if (index == _views.Count - 1) return;
                _views.RemoveAt(index);
            }
            _views.Add(v);
        }

        public void PopView() => _views.RemoveAt(_views.Count - 1);
        public void RemoveView(IViewResponse v) => _views.Remove(v);
        
        public Task<string> RequestActionAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return Core.Platform.EnsureOnMainThread(() => 
                View?.RequestActionAsync(title, cancel, destruction, buttons) ?? Task.FromResult<string>(null));
        }
        
        public Task<bool> RequestAnswerAsync(string title, string description, string cancel, string accept)
        {
            return Core.Platform.EnsureOnMainThread(() => 
                View?.RequestAnswerAsync(title, description, cancel, accept) ?? Task.FromResult(false));
        }

        public Task<string> RequestInputAsync(string title, string description, string cancel, string accept)
        {
            return Core.Platform.EnsureOnMainThread(() => 
                View?.RequestInputAsync(title, description, cancel, accept) ?? Task.FromResult<string>(null));
        }
        public Task<string> RequestInputWithPicAsync(string title, string description, string cancel, string accept,byte[]sources)
        {
            return Core.Platform.EnsureOnMainThread(() => 
                View?.RequestInputWithPicAsync(title, description, cancel, accept,sources) ?? Task.FromResult<string>(null));
        }

        public Task RequestMessageAsync(string title, string message, string button = "知道了")
        {
            return Core.Platform.EnsureOnMainThread(() => 
                View?.RequestMessageAsync(title, message, button) ?? Task.CompletedTask);
        }

        public Task RequestChartAsync(Chart chart, string title = "", string close = "关闭")
        {
            return Core.Platform.EnsureOnMainThread(() => 
                View?.RequestChartAsync(chart, title, close) ?? Task.CompletedTask);
        }
        public Task<string> RequestWebDialogAsync(string title, string description, string url, string cancel, string accept, bool navigation, bool hasInput, string inputHint, WebDialogAdditionalArgs additionalArgs)
        {
            return Core.Platform.EnsureOnMainThread(() => 
                View?.RequestWebDialogAsync(title, description, url, cancel, accept, navigation, hasInput, inputHint, additionalArgs) ?? Task.FromResult<string>(null));
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