﻿using HandSchool.Internals;
using HandSchool.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public enum FeedState
    {
        Normal, Search
    }
    /// <summary>
    /// 学校通知的视图模型，提供了刷新和数据源的功能。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    /// <inheritdoc cref="ICollection{T}" />
    public sealed class FeedViewModel : BaseViewModel, ICollection<FeedItem>
    {
        static readonly Lazy<FeedViewModel> Lazy = 
            new Lazy<FeedViewModel>(() => new FeedViewModel());

        private int curPageIndex;
        

        public DateTime? LastReload = null;
        public (FeedState, string) WorkState = (FeedState.Normal, null);

        /// <summary>
        /// 消息内容列表
        /// </summary>
        public ObservableCollection<FeedItem> Items { get; set; }

        /// <summary>
        /// 加载消息的命令
        /// </summary>
        public ICommand LoadItemsCommand { get; set; }

        public ICommand SearchByKeyWordCommand { get; set; }

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
            Items.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Count));
            LoadItemsCommand = new CommandAction(ExecuteLoadItemsCommand);
            SearchByKeyWordCommand = new CommandAction(SearchByKeyWord);
        }

        /// <summary>
        /// 剩余页面数
        /// </summary>
        public int CurPageIndex
        {
            get => curPageIndex;
            set => SetProperty(ref curPageIndex, value, onChanged: _leftPageCountChanged);
        }

        public int TotalPageCount = 0;

        public int LeftPage => TotalPageCount - CurPageIndex;

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
                else if (LeftPage <= 0)
                    return "已经到底啦QAQ";
                return "上拉加载更多……";
            }
        }

        /// <summary>
        /// 加载消息的方法。
        /// </summary>
        private Task ExecuteLoadItemsCommand() => LoadItems(false);

        private Task SearchByKeyWord() => SearchWord(false);
        public static Func<Task<TaskResp>> BeforeOperatingCheck { set; private get; }

        /// <summary>
        /// 加载消息的方法。
        /// </summary>
        public async Task LoadItems(bool more)
        {
            if (IsBusy) return;
            WorkState = (FeedState.Normal, null);
            IsBusy = true;
            if (BeforeOperatingCheck != null)
            {
                var msg = await BeforeOperatingCheck();
                if (!msg.IsSuccess)
                {
                    await RequestMessageAsync("错误", msg.ToString());
                    IsBusy = false;
                    return;
                }
            }
            IsBusy = false;

            IsBusy = true;
            int newcnt = 0;
            try
            {
                newcnt = await Core.App.Feed.Execute(more ? CurPageIndex + 1 : 1);
                if (!more)
                {
                    LastReload = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex);
            }
            finally
            {
                IsBusy = false;
            }

            CurPageIndex = newcnt;
        }
        
        public async Task SearchWord(bool more, string word = null)
        {
            if (IsBusy) return;
            IsBusy = true;
            if (BeforeOperatingCheck != null)
            {
                var msg = await BeforeOperatingCheck();
                if (!msg.IsSuccess)
                {
                    await RequestMessageAsync("错误", msg.ToString());
                    IsBusy = false;
                    return;
                }
            }
            IsBusy = false;

            IsBusy = true;
            int newcnt = 0;

            try
            {
                string str;
                if (string.IsNullOrWhiteSpace(word))
                    str = await RequestInputAsync("输入要搜索的关键词", "", "取消", "完成");
                else str = word;

                if (string.IsNullOrWhiteSpace(str))
                {
                    IsBusy = false;
                    return;
                }
                else
                {
                    newcnt = await Core.App.Feed.Search(str, more ? CurPageIndex + 1 : 1);
                    WorkState = (FeedState.Search, str);
                }

                if (!more)
                {
                    LastReload = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex);
            }
            finally
            {
                IsBusy = false;
            }
            curPageIndex = 0;
            CurPageIndex = newcnt;
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