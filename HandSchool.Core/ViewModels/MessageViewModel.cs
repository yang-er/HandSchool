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
    /// 站内消息的视图模型，提供了读取删除等功能。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    /// <inheritdoc cref="ICollection{T}" />
    public sealed class MessageViewModel : BaseViewModel, ICollection<IMessageItem>
    {
        private bool IsFirstOpen { get; set; }
        private IMessageEntrance Service { get; }

        /// <summary>
        /// 目前的所有站内消息
        /// </summary>
        public ObservableCollection<IMessageItem> Items { get; set; }

        /// <summary>
        /// 加载消息的命令
        /// </summary>
        public ICommand LoadItemsCommand { get; set; }

        /// <summary>
        /// 删除所有的命令
        /// </summary>
        public ICommand DeleteAllCommand { get; set; }

        /// <summary>
        /// 全部设置已读的命令
        /// </summary>
        public ICommand ReadAllCommand { get; set; }
        
        /// <summary>
        /// 将视图模型的操作加载。
        /// </summary>
        public MessageViewModel(IMessageEntrance service, ILogger<MessageViewModel> logger)
        {
            Title = "站内消息";
            Items = new ObservableCollection<IMessageItem>();
            Items.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Count));
            LoadItemsCommand = new CommandAction(ExecuteLoadItemsCommand);
            DeleteAllCommand = new CommandAction(ExecuteDeleteAllCommand);
            ReadAllCommand = new CommandAction(ExecuteReadAllCommand);
            IsFirstOpen = true;

            Service = service;
            Logger = logger;
        }

        /// <summary>
        /// 删除所有站内消息。
        /// </summary>
        [ToFix("并发删除所有")]
        private async Task ExecuteDeleteAllCommand()
        {
            foreach (var item in Items)
            {
                await Service.Delete(item.Id);
            }

            Items.Clear();
        }

        /// <summary>
        /// 将所有站内消息设置为已读状态。
        /// </summary>
        [ToFix("并发已读所有")]
        private async Task ExecuteReadAllCommand()
        {
            foreach (var item in Items)
            {
                await Service.SetReadState(item.Id, true);
            }
        }

        /// <summary>
        /// 加载所有的站内消息内容。
        /// </summary>
        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var values = await Service.ExecuteAsync();
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

        /// <summary>
        /// 第一次启动时调用。
        /// </summary>
        public async void FirstOpen()
        {
            if (!IsFirstOpen) return;
            IsFirstOpen = false;

            await ExecuteLoadItemsCommand();
        }

        #region ICollection<T> Implements

        public int Count => Items.Count;
        public void Add(IMessageItem item) => Items.Add(item);
        public void Clear() => Items.Clear();
        public bool Remove(IMessageItem item) => Items.Remove(item);

        bool ICollection<IMessageItem>.IsReadOnly => ((ICollection<IMessageItem>)Items).IsReadOnly;
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        IEnumerator<IMessageItem> IEnumerable<IMessageItem>.GetEnumerator() => Items.GetEnumerator();
        bool ICollection<IMessageItem>.Contains(IMessageItem item) => Items.Contains(item);
        void ICollection<IMessageItem>.CopyTo(IMessageItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public void AddRange(IEnumerable<IMessageItem> toAdd)
        {
            foreach (var item in toAdd) Items.Add(item);
        }

        #endregion
    }
}
