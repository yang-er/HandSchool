using HandSchool.Internals;
using HandSchool.Models;
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
        static readonly Lazy<MessageViewModel> Lazy = 
            new Lazy<MessageViewModel>(() => new MessageViewModel());
        private bool IsFirstOpen { get; set; }

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
        /// 视图模型的实例
        /// </summary>
        public static MessageViewModel Instance => Lazy.Value;

        /// <summary>
        /// 将视图模型的操作加载。
        /// </summary>
        private MessageViewModel()
        {
            Title = "站内消息";
            Items = new ObservableCollection<IMessageItem>();
            LoadItemsCommand = new CommandAction(ExecuteLoadItemsCommand);
            DeleteAllCommand = new CommandAction(ExecuteDeleteAllCommand);
            ReadAllCommand = new CommandAction(ExecuteReadAllCommand);
            IsFirstOpen = true;
        }

        /// <summary>
        /// 删除所有站内消息。
        /// </summary>
        private async Task ExecuteDeleteAllCommand()
        {
            foreach (var item in Instance.Items)
            {
                await Core.App.Message.Delete(item.Id);
            }

            Items.Clear();
        }

        /// <summary>
        /// 将所有站内消息设置为已读状态。
        /// </summary>
        private async Task ExecuteReadAllCommand()
        {
            foreach (var item in Items)
            {
                await Core.App.Message.SetReadState(item.Id, true);
            }
        }

        /// <summary>
        /// 加载所有的站内消息内容。
        /// </summary>
        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return; IsBusy = true;

            try
            {
                await Core.App.Message.Execute();
            }
            catch (Exception ex)
            {
                this.WriteLog(ex);
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
