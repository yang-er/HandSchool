﻿using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 课程表的视图模型，提供了增删改查功能。
    /// </summary>
    public class ScheduleViewModel : BaseViewModel
    {
        private int week;
        static ScheduleViewModel instance = null;
        const string storageFile = "jlu.kcb2.json";

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static ScheduleViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new ScheduleViewModel();
                return instance;
            }
        }

        /// <summary>
        /// 将当前周、增删改查等操作加载。
        /// </summary>
        private ScheduleViewModel()
        {
            Core.App.LoginStateChanged += SyncData;
            ItemsLoader = new Lazy<List<CurriculumItem>>(LoadFromFile);
            RefreshCommand = new Command(Refresh);
            AddCommand = new Command(Create);
            ChangeWeekCommand = new Command(ChangeWeek);
            Title = "课程表";
        }

        /// <summary>
        /// 当前周
        /// </summary>
        public int Week => week;

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
            SetProperty(ref week, sys.CurrentWeek, nameof(CurrentWeek));
        }

        #region 增删改查命令

        /// <summary>
        /// 刷新课程表的命令
        /// </summary>
        public Command RefreshCommand { get; set; }

        /// <summary>
        /// 刷新课程表，修改当前周，并通知视图重新绘制。
        /// </summary>
        private async void Refresh()
        {
            if (IsBusy) return;
            IsBusy = true;
            await Core.App.Schedule.Execute();
            RefreshComplete?.Invoke();
            IsBusy = false;
        }

        /// <summary>
        /// 修改当前周的命令
        /// </summary>
        public Command ChangeWeekCommand { get; }

        /// <summary>
        /// 修改当前周，并通知视图重新绘制。
        /// </summary>
        private async void ChangeWeek()
        {
            var paramlist = new string[25];
            paramlist[0] = "所有周";
            for (int i = 1; i < 25; i++)
                paramlist[i] = $"第{i}周";

            var ret = await DisplayActionSheet("显示周", "取消", null, paramlist);

            if (ret != "取消")
            {
                for (int i = 0; i < 25; i++)
                {
                    if (ret == paramlist[i])
                    {
                        SetProperty(ref week, i, nameof(CurrentWeek));
                        break;
                    }
                }

                RefreshComplete?.Invoke();
            }
        }

        /// <summary>
        /// 添加课程的命令
        /// </summary>
        public Command AddCommand { get; set; }

#if __UWP__
        /// <summary>
        /// 将添加课程的页面加载。
        /// </summary>
        async void Create()
        {
            var item = new CurriculumItem
            {
                IsCustom = true,
                CourseID = "CUSTOM-" + DateTime.Now.ToString("s")
            };

            var box = new CurriculumDialog(item, true);
            var result = await box.ShowAsync();
            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                RefreshComplete?.Invoke();
        }
#else
        /// <summary>
        /// 将添加课程的页面加载。
        /// </summary>
        /// <param name="param">系统的导航服务。</param>
        async void Create(object param)
        {
            var item = new CurriculumItem
            {
                IsCustom = true,
                CourseID = "CUSTOM-" + DateTime.Now.ToString("s")
            };

            var page = new CurriculumPage(item, true);
            await page.ShowAsync(param as INavigation);
        }
#endif

        #endregion

        #region 数据源及操作

        private Lazy<List<CurriculumItem>> ItemsLoader;

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

        /// <summary>
        /// 从周的条件渲染课程表。
        /// </summary>
        /// <param name="week">第几周。</param>
        /// <param name="list">输出列表的迭代器。</param>
        public void RenderWeek(int week, out IEnumerable<CurriculumItemBase> list)
        {
            if (week == 0)
            {
                if (ItemsSet is null) FetchItemsSet();
                list = ItemsSet;
            }
            else
            {
                list = Items.FindAll((item) => item.IfShow(week));
            }
        }

        /// <summary>
        /// 获取多节课的课程表列表。
        /// </summary>
        private void FetchItemsSet()
        {
            var controller = new CurriculumSet.MergeAlgorithm();
            var list = Items;
            foreach (var i in list)
                controller.AddClass(i);
            ItemsSet = controller.ToList();
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

        /// <summary>
        /// 保存课程表项目
        /// </summary>
        public void SaveToFile()
        {
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
            Core.WriteConfig(storageFile, Items.Serialize());
        }

        /// <summary>
        /// 从文件加载课程表列表。
        /// </summary>
        /// <returns>课程表内容</returns>
        private List<CurriculumItem> LoadFromFile()
        {
            var LastReport = Core.ReadConfig(storageFile);

            if (LastReport != "")
            {
                return LastReport.ParseJSON<List<CurriculumItem>>();
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
