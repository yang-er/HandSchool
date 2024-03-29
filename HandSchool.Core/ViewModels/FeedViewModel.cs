﻿using HandSchool.Internals;
using HandSchool.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandSchool.Internal;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public enum FeedMode
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

        private int _curPageIndex;

        public bool IsFeedOutOfTime => _timeoutManager.NotInit || _timeoutManager.IsTimeout();

        private readonly TimeoutManager _timeoutManager
            = new TimeoutManager(900);

        public (FeedMode, string) WorkState = (FeedMode.Normal, null);

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

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            set => SetProperty(ref _isRefreshing, value, nameof(IsRefreshing));
            get => _isRefreshing;
        }
        
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
            get => _curPageIndex;
            set => SetProperty(ref _curPageIndex, value, onChanged: _leftPageCountChanged);
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

        /// <summary>
        /// 由于iOS平台中RefreshView的呈现特性(显示加载的转圈需要退回到最起始位置)，
        /// "加载更多"功能，需要保持当前进度，所以不能直接使IsRefreshing绑定IsBusy。
        /// Android平台无此限制，为了更加醒目，可以直接全部用小圈呈现
        /// </summary>
        void SetBusy(bool more, bool value)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    IsRefreshing = value; 
                    break;
                case Device.iOS:
                    if (more) IsBusy = value;
                    else IsRefreshing = value;
                    break;
            }
        }
        bool GetBusy() => IsRefreshing || IsBusy;

        /// <summary>
        /// 加载消息的方法。
        /// </summary>
        public async Task LoadItems(bool more)
        {
            if (GetBusy()) return;
            WorkState = (FeedMode.Normal, null);
            SetBusy(more, true);
            var msg = await CheckEnv("LoadItems");
            if (!msg)
            {
                await RequestMessageAsync("错误", msg.ToString());
                SetBusy(more, false);
                return;
            }

            SetBusy(more, true);
            int newCnt = 0;
            try
            {
                newCnt = await Core.App.Feed.Execute(more ? CurPageIndex + 1 : 1);
                if (!more)
                {
                    _timeoutManager.Refresh();
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex);
            }
            finally
            {
                SetBusy(more, false);
            }

            CurPageIndex = newCnt;
        }

        public async Task SearchWord(bool more, string word = null)
        {
            if (GetBusy()) return;
            SetBusy(more, true);
            var msg = await CheckEnv("SearchWord");
            if (!msg)
            {
                await RequestMessageAsync("错误", msg.ToString());
                SetBusy(more, false);
                return;
            }

            var newCnt = 0;
            try
            {
                string str;
                if (string.IsNullOrWhiteSpace(word))
                    str = await RequestInputAsync("输入要搜索的关键词", "", "取消", "完成");
                else str = word;

                if (string.IsNullOrWhiteSpace(str))
                {
                    SetBusy(more, false);
                    return;
                }
                else
                {
                    newCnt = await Core.App.Feed.Search(str, more ? CurPageIndex + 1 : 1);
                    WorkState = (FeedMode.Search, str);
                }

                if (!more)
                {
                    _timeoutManager.Refresh();
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(ex);
            }
            finally
            {
                SetBusy(more, false);
            }

            _curPageIndex = 0;
            CurPageIndex = newCnt;
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