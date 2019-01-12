using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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

        static readonly Lazy<ScheduleViewModel> Lazy =
            new Lazy<ScheduleViewModel>(() => new ScheduleViewModel());

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static ScheduleViewModel Instance => Lazy.Value;

        /// <summary>
        /// 将当前周、增删改查等操作加载。
        /// </summary>
        private ScheduleViewModel()
        {
            Core.App.LoginStateChanged += SyncData;
            ItemsLoader = new Lazy<List<CurriculumItem>>(LoadFromFile);
            RefreshCommand = new CommandAction(Refresh);
            AddCommand = new CommandAction(Create);
            ChangeWeekCommand = new CommandAction(ChangeWeek);
            Title = "课程表";
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
            await Core.App.Schedule.Execute();
            RefreshComplete?.Invoke();
            IsBusy = false;
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

            RefreshComplete?.Invoke();
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

            if (await Core.Platform.ShowNewCurriculumPageAsync(item, null))
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
            Core.Configure.Write(storageFile, Items.Serialize());
        }

        /// <summary>
        /// 从文件加载课程表列表。
        /// </summary>
        /// <returns>课程表内容</returns>
        private static List<CurriculumItem> LoadFromFile()
        {
            var LastReport = Core.Configure.Read(storageFile);
            return LastReport != "" ? LastReport.ParseJSON<List<CurriculumItem>>() : new List<CurriculumItem>();
        }

        #endregion
    }
}
