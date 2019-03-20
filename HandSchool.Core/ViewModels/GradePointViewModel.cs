using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 绩点成绩的视图模型，提供了加载绩点的命令和数据源。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    /// <inheritdoc cref="ICollection{T}" />
    public sealed class GradePointViewModel : BaseViewModel, ICollection<IGradeItem>
    {
        /// <summary>
        /// 绩点成绩列表
        /// </summary>
        public ObservableCollection<IGradeItem> Items { get; set; }

        /// <summary>
        /// 加载绩点的命令
        /// </summary>
        public ICommand LoadItemsCommand { get; set; }
        
        /// <summary>
        /// 视图模型使用的服务
        /// </summary>
        private IGradeEntrance Service { get; }
        
        /// <summary>
        /// 建立绩点视图模型的数据源和刷新操作。
        /// </summary>
        public GradePointViewModel(IGradeEntrance service, ILogger<GradePointViewModel> logger)
        {
            Title = "学分成绩";
            Items = new ObservableCollection<IGradeItem>();
            LoadItemsCommand = new CommandAction(ExecuteLoadItemsCommand);
            Service = service;
            Logger = logger;
        }

        /// <summary>
        /// 启动后预加载数据。
        /// </summary>
        /// <returns></returns>
        public async Task LoadCacheAsync()
        {
            await Task.Yield();

            try
            {
                var values = await Service.OfflineAsync();
                Clear();
                AddRange(values);
            }
            catch (ServiceException ex)
            {
                Logger.Warn(ex);
            }
        }

        /// <summary>
        /// 加载绩点的具体函数。
        /// </summary>
        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var values = await Service.OnlineAsync();
                Clear();
                AddRange(values);
            }
            catch (ServiceException ex)
            {
                await RequestMessageAsync("出错", ex.Message);
                Logger.Warn(ex);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        #region ICollection<T> Implements

        public int Count => Items.Count;
        public void Add(IGradeItem item) => Items.Add(item);
        public void Clear() => Items.Clear();

        bool ICollection<IGradeItem>.IsReadOnly => ((ICollection<IGradeItem>)Items).IsReadOnly;
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        IEnumerator<IGradeItem> IEnumerable<IGradeItem>.GetEnumerator() => Items.GetEnumerator();
        bool ICollection<IGradeItem>.Contains(IGradeItem item) => Items.Contains(item);
        void ICollection<IGradeItem>.CopyTo(IGradeItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
        bool ICollection<IGradeItem>.Remove(IGradeItem item) => Items.Remove(item);
        
        public void AddRange(IEnumerable<IGradeItem> toAdd)
        {
            foreach (var item in toAdd) Items.Add(item);
        }
        
        #endregion
    }
}
