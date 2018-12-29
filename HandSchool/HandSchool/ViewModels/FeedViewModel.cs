using HandSchool.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 学校通知的视图模型，提供了刷新和数据源的功能。
    /// </summary>
    public class FeedViewModel : BaseViewModel, ICollection<FeedItem>
    {
        static FeedViewModel instance = null;

        /// <summary>
        /// 消息内容列表
        /// </summary>
        public ObservableCollection<FeedItem> Items { get; set; }

        /// <summary>
        /// 加载消息的命令
        /// </summary>
        public Command LoadItemsCommand { get; set; }

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static FeedViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new FeedViewModel();
                return instance;
            }
        }

        /// <summary>
        /// 将学校通知的数据源和刷新操作组织起来。
        /// </summary>
        private FeedViewModel()
        {
            Title = "学校通知";
            Items = new ObservableCollection<FeedItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
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
                Core.Log(ex);
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
