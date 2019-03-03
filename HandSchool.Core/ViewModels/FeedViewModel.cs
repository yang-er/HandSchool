using HandSchool.Internals;
using HandSchool.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandSchool.Design;
using HandSchool.Services;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 学校通知的视图模型，提供了刷新和数据源的功能。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    /// <inheritdoc cref="ICollection{T}" />
    public sealed class FeedViewModel : BaseViewModel, ICollection<FeedItem>
    {
        private int leftPageCount;
        private IFeedEntrance Service { get; }
        private ILogger<FeedViewModel> Logger { get; }

        /// <summary>
        /// 消息内容列表
        /// </summary>
        public ObservableCollection<FeedItem> Items { get; set; }

        /// <summary>
        /// 加载消息的命令
        /// </summary>
        public ICommand LoadItemsCommand { get; set; }
        
        /// <summary>
        /// 将学校通知的数据源和刷新操作组织起来。
        /// </summary>
        public FeedViewModel(IFeedEntrance service, ILogger<FeedViewModel> logger)
        {
            Title = "学校通知";
            Items = new ObservableCollection<FeedItem>();
            Items.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Count));
            LoadItemsCommand = new CommandAction(async () => await LoadItems(false));

            Service = service;
            Logger = logger;
            Task.Run(LoadCacheAsync);
        }

        /// <summary>
        /// 剩余页面数
        /// </summary>
        public int LeftPageCount
        {
            get => leftPageCount;
            set => SetProperty(ref leftPageCount, value, onChanged: _leftPageCountChanged);
        }

        void _leftPageCountChanged()
        {
            OnPropertyChanged(nameof(FooterTip));
        }

        /// <summary>
        /// 列表底部提示
        /// </summary>
        public string FooterTip
        {
            get
            {
                if (IsBusy)
                    return "正在加载中~";
                else if (leftPageCount == 0)
                    return "已经到底啦QAQ";
                return "下拉加载更多……";
            }
        }

        /// <summary>
        /// 从缓存中加载数据。如果不存在，那么更新数据。
        /// </summary>
        private async Task LoadCacheAsync()
        {
            await Task.Yield();
            var items = await Service.FromCacheAsync();
            if (items == null) await LoadItems(true);
            else AddRange(items);
        }

        /// <summary>
        /// 加载消息的方法。
        /// </summary>
        private async Task LoadItems(bool more)
        {
            if (IsBusy) return;
            IsBusy = true;
            int newcnt = 0;

            try
            {
                var res = await Service.FetchAsync(more ? LeftPageCount : 1);
                newcnt = res.Item1;
                if (!more) Clear();
                AddRange(res.Item2);
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

            LeftPageCount = newcnt;
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