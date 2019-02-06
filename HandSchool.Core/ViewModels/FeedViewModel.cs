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
    /// 学校通知的视图模型，提供了刷新和数据源的功能。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    /// <inheritdoc cref="ICollection{T}" />
    public sealed class FeedViewModel : BaseViewModel, ICollection<FeedItem>
    {
        static readonly Lazy<FeedViewModel> Lazy = 
            new Lazy<FeedViewModel>(() => new FeedViewModel());

        /// <summary>
        /// 消息内容列表
        /// </summary>
        public ObservableCollection<FeedItem> Items { get; set; }

        /// <summary>
        /// 加载消息的命令
        /// </summary>
        public ICommand LoadItemsCommand { get; set; }

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static FeedViewModel Instance => Lazy.Value;

        /// <summary>
        /// 将学校通知的数据源和刷新操作组织起来。
        /// </summary>
        private FeedViewModel()
        {
            Title = "学校通知";
            Items = new ObservableCollection<FeedItem>();
            LoadItemsCommand = new CommandAction(ExecuteLoadItemsCommand);
        }

        /// <summary>
        /// 加载消息的方法。
        /// </summary>
        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return; IsBusy = true;

            try
            {
                await Core.App.Feed.Execute();
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
        
        #region ICollection<T> Implements

        public int Count => Items.Count;
        public void Add(FeedItem item) => Items.Add(item);
        public void Clear() => Items.Clear();
        public bool Remove(FeedItem item) => Items.Remove(item);

        bool ICollection<FeedItem>.IsReadOnly => ((ICollection<FeedItem>)Items).IsReadOnly;
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        IEnumerator<FeedItem> IEnumerable<FeedItem>.GetEnumerator() => Items.GetEnumerator();
        bool ICollection<FeedItem>.Contains(FeedItem item) => Items.Contains(item);
        void ICollection<FeedItem>.CopyTo(FeedItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public void AddRange(IEnumerable<FeedItem> toAdd)
        {
            foreach (var item in toAdd) Items.Add(item);
        }

        #endregion
    }
}