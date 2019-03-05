﻿using HandSchool.Design;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MessagingCenter = Xamarin.Forms.MessagingCenter;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 课程表的视图模型，提供了增删改查功能。
    /// </summary>
    /// <inheritdoc cref="ScheduleViewModelBase" />
    public sealed class ScheduleViewModel : ScheduleViewModelBase
    {
        private int week;
        const string storageFile = "jlu.kcb2.json";
        
        private IConfiguration Configure { get; }
        private IScheduleEntrance Service { get; }

        /// <summary>
        /// 将当前周、增删改查等操作加载。
        /// </summary>
        public ScheduleViewModel(IScheduleEntrance service, IConfiguration configure, ILogger<ScheduleViewModel> logger)
        {
            Service = service;
            Configure = configure;
            Logger = logger;

            Title = "课程表";
            RefreshCommand = new CommandAction(Refresh);
            AddCommand = new CommandAction(Create);
            ChangeWeekCommand = new CommandAction(ChangeWeek);

            MessagingCenter.Subscribe<object, LoginStateEventArgs>(this, Core.LoginStateChangedSignal, SyncData);

            Core.App.LoginStateChanged += SyncData;
            ItemsLoader = new Lazy<List<CurriculumItem>>(Service.FromCache);
        }
        
        public override bool IsComposed => false;
        
        public override int Week
        {
            get => week;
            set => SetProperty(ref week, value, nameof(CurrentWeek));
        }

        /// <summary>
        /// 当前周的文字描述
        /// </summary>
        public string CurrentWeek => week == 0 ? "所有周" : $"第{week}周";

        /// <summary>
        /// 课程表刷新完成的事件
        /// </summary>
        public event Action RefreshComplete;

        /// <summary>
        /// 当教务系统登录时，同步目前周。
        /// </summary>
        private void SyncData(object sender, LoginStateEventArgs e)
        {
            if (e.State != LoginState.Succeeded) return;
            var sys = sender as ISchoolSystem;
            Debug.Assert(sys != null);
            SetProperty(ref week, sys.CurrentWeek, nameof(CurrentWeek));
        }

        #region 增删改查命令

        /// <summary>
        /// 刷新课程表，修改当前周，并通知视图重新绘制。
        /// </summary>
        private async Task Refresh()
        {
            if (IsBusy) return;
            IsBusy = true;
            bool updated = false;

            try
            {
                var values = await Service.ExecuteAsync();
                RemoveAllItem(item => !item.IsCustom);
                foreach (var item in values)
                    AddItem(item);
                updated = true;
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

            if (updated)
                SendRefreshComplete();
        }

        /// <summary>
        /// 修改当前周，并通知视图重新绘制。
        /// </summary>
        private async Task ChangeWeek()
        {
            var paramList = new string[25];
            paramList[0] = "所有周";
            for (int i = 1; i < 25; i++)
                paramList[i] = $"第{i}周";

            var ret = await RequestActionAsync("显示周", "取消", null, paramList);
            if (ret == "取消") return;

            for (int i = 0; i < 25; i++)
            {
                if (ret != paramList[i]) continue;
                SetProperty(ref week, i, nameof(CurrentWeek));
                break;
            }

            SendRefreshComplete();
        }
        
        /// <summary>
        /// 将添加课程的页面加载。
        /// </summary>
        private async Task Create()
        {
            var item = new CurriculumItem
            {
                IsCustom = true,
                CourseID = "CUSTOM-" + DateTime.Now.ToString("s")
            };

            var page = Core.New<ICurriculumPage>();
            page.SetNavigationArguments(item, true);

            if (await page.ShowAsync())
                SendRefreshComplete();
        }

        public void SendRefreshComplete()
        {
            RefreshComplete?.Invoke();
        }

        #endregion

        #region 数据源及操作

        private readonly Lazy<List<CurriculumItem>> ItemsLoader;

        /// <summary>
        /// 是否已经完成加载
        /// </summary>
        public bool ItemsLoaded => ItemsLoader.IsValueCreated;

        /// <summary>
        /// 所有的课程表内容
        /// </summary>
        private List<CurriculumItem> Items => ItemsLoader.Value;

        /// <summary>
        /// 课程表合并状态内容
        /// </summary>
        private IEnumerable<CurriculumSet> ItemsSet { get; set; }
        
        public override void RenderWeek(int week, out IEnumerable<CurriculumItemBase> list)
        {
            if (week == 0)
            {
                if (ItemsSet is null) ItemsSet = FetchItemsSet(Items);
                list = ItemsSet;
            }
            else
            {
                list = Items.FindAll((item) => item.IfShow(week));
            }
        }
        
        /// <summary>
        /// 添加课程。
        /// </summary>
        /// <param name="item">新的课程表项目。</param>
        [DebuggerStepThrough]
        public void AddItem(CurriculumItem item)
        {
            Items.Add(item);
            ItemsSet = null;
        }

        /// <summary>
        /// 查询是否包含课程。
        /// </summary>
        /// <param name="context">课程</param>
        /// <returns>是否包含</returns>
        public bool Contains(CurriculumItem context)
        {
            return Items.Contains(context);
        }

        /// <summary>
        /// 删除课程。
        /// </summary>
        /// <param name="item">已有的课程表项目。</param>
        [DebuggerStepThrough]
        public void RemoveItem(CurriculumItem item)
        {
            Items.Remove(item);
            ItemsSet = null;
        }

        /// <summary>
        /// 寻找第一个满足谓词的课程表项目。
        /// </summary>
        /// <param name="pred">判断课程表项目的谓词。</param>
        /// <returns>课程表项目的内容。</returns>
        [DebuggerStepThrough]
        public CurriculumItem FindItem(Predicate<CurriculumItem> pred)
        {
            return Items.Find(pred);
        }

        /// <summary>
        /// 寻找最后一个满足谓词的课程表项目。
        /// </summary>
        /// <param name="pred">判断课程表项目的谓词。</param>
        /// <returns>课程表项目的内容。</returns>
        [DebuggerStepThrough]
        public CurriculumItem FindLastItem(Predicate<CurriculumItem> pred)
        {
            return Items.FindLast(pred);
        }

        /// <summary>
        /// 删除所有满足谓词的项目
        /// </summary>
        /// <param name="pred">判断课程表项目的谓词。</param>
        [DebuggerStepThrough]
        public void RemoveAllItem(Predicate<CurriculumItem> pred)
        {
            Items.RemoveAll(pred);
            ItemsSet = null;
        }
        
        #endregion

        /// <summary>
        /// 保存课程表项目
        /// </summary>
        public async void SaveToFile()
        {
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
            await Configure.SaveAsAsync(storageFile, Items);
        }
    }
}
