using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 课程表的视图模型，提供了增删改查功能。
    /// </summary>
    /// <inheritdoc cref="ScheduleViewModelBase" />
    public sealed class ScheduleViewModel : ScheduleViewModelBase
    {
        private int _week;
        private const string JsonName = "core.kcb";

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
            _itemsLoader = new Lazy<List<CurriculumItem>>(LoadFromFile);
            RefreshCommand = new CommandAction(Refresh);
            AddCommand = new CommandAction(Create);
            ChangeWeekCommand = new CommandAction(ChangeWeek);
            QuickChangeWeekCommand = new CommandAction(QuickSwitchWeek);
            Title = "课程表";
        }

        public override bool IsComposed => false;

        public override int Week
        {
            get => _week;
            set => SetProperty(ref _week, value, nameof(CurrentWeek));
        }

        private SchoolState _schoolState;
        public override SchoolState SchoolState
        {
            get => _schoolState;
            set => SetProperty(ref _schoolState, value, nameof(CurrentWeek));
        }
        
        public int TotalWeek { get; set; }

        /// <summary>
        /// 当前周的文字描述
        /// </summary>
        public string CurrentWeek => SchoolState != SchoolState.Normal ? "假期" : (_week == 0 ? "所有周" : $"第{_week}周");

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
            Week = sys.CurrentWeek;
            SchoolState = sys.SchoolState;
            TotalWeek = sys.TotalWeek;
        }
        
        public static Func<Task<TaskResp>> BeforeOperatingCheck { private get; set; }


        #region 增删改查命令

        /// <summary>
        /// 刷新课程表，修改当前周，并通知视图重新绘制。
        /// </summary>
        public async Task Refresh()
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
            await Core.App.Schedule.Execute();
            SendRefreshComplete();
            IsBusy = false;
        }

        /// <summary>
        /// 快速切换课程表显示周。
        /// </summary>
        private async Task QuickSwitchWeek()
        {
            if (SchoolState != SchoolState.Normal)
            {
                return;
            }
            var now = Core.App.Service.CurrentWeek;
            var va = Core.App.Service.SchoolState != SchoolState.Normal;
            int week2;
            if (_week == 0)
            {
                week2 = va ? 0 : now;
            }
            else
            {
                week2 = _week;
            }
            
            var @params = new List<string> {"所有周"};
            var pre = week2 - 1;
            if (pre > 0)
            {
                @params.Add($"上一周 ({pre})");
            }
            @params.Add($"当前周 ({(va ? "假期" : now.ToString())})");
            var po = week2 + 1;
            if (po <= TotalWeek)
            {
                @params.Add($"下一周 ({po})");
            }
            
            var ret = await RequestActionAsync("跳转周", "取消", null, @params.ToArray());
            if (ret == "取消" || ret == null) return;

            if (ret.StartsWith("当前周"))
            {
                SchoolState = Core.App.Service.SchoolState;
                Week = Core.App.Service.CurrentWeek;
            }
            else
            {
                SchoolState = SchoolState.Normal;
                if (ret == "所有周")
                {
                    Week = 0;
                }
                else if (ret.StartsWith("上一周"))
                {
                    Week = pre;
                }
                else if (ret.StartsWith("下一周"))
                {
                    Week = po;
                }
            }

            SendRefreshComplete();
        }

        /// <summary>
        /// 修改当前周，并通知视图重新绘制。
        /// </summary>
        private async Task ChangeWeek()
        {
            var paramList = new string[TotalWeek + 1];
            paramList[0] = "所有周";
            for (var i = 1; i <= TotalWeek; i++)
                paramList[i] = $"第{i}周";

            var ret = await RequestActionAsync("跳转周", "取消", null, paramList);
            if (ret == "取消") return;

            var index = paramList.IndexOf(ret);
            SchoolState = SchoolState.Normal;
            SetProperty(ref _week, index, nameof(CurrentWeek));
            
            SendRefreshComplete();
        }

        /// <summary>
        /// 将添加课程的页面加载。
        /// </summary>
        private async Task Create()
        {
            await RequestMessageAsync("提示", "刷新课程表不会影响自定义课程；课程时间冲突时，显示可能不正常。");
            var item = new CurriculumItem
            {
                IsCustom = true,
                CourseId = "CUSTOM-" + DateTime.Now.ToString("s")
            };

            var page = Core.New<ICurriculumPage>();
            page.SetNavigationArguments(item, true);

            
            if (await page.IsSuccess())
                SendRefreshComplete();
        }

        public void SendRefreshComplete()
        {
            RefreshComplete?.Invoke();
        }

        #endregion

        #region 数据源及操作

        private readonly Lazy<List<CurriculumItem>> _itemsLoader;

        /// <summary>
        /// 是否已经完成加载
        /// </summary>
        public bool ItemsLoaded => _itemsLoader.IsValueCreated;

        /// <summary>
        /// 所有的课程表内容
        /// </summary>
        private List<CurriculumItem> Items => _itemsLoader.Value;

        /// <summary>
        /// 课程表合并状态内容
        /// </summary>
        private IEnumerable<CurriculumSet> ItemsSet { get; set; }

        public override void RenderWeek(int week, SchoolState state, out IEnumerable<CurriculumItemBase> list)
        {
            if (state != SchoolState.Normal)
            {
                list = Array.Empty<CurriculumItemBase>();
                return;
            }
            if (week == 0)
            {
                ItemsSet ??= FetchItemsSet(Items);
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

        public IList<CurriculumItem> FindItems(Predicate<CurriculumItem> pred)
        {
            return
                (from item in Items
                where pred(item)
                select item).ToList();
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
            Core.App.Loader.JsonManager.InsertOrUpdateTable(new ServerJson
            {
                JsonName = JsonName,
                Json = Items.Serialize()
            });
        }

        /// <summary>
        /// 从文件加载课程表列表。
        /// </summary>
        /// <returns>课程表内容</returns>
        private static List<CurriculumItem> LoadFromFile()
        {
            return Core.App.Loader.JsonManager
                       .GetItemWithPrimaryKey(JsonName)
                       ?.ToObject<List<CurriculumItem>>()
                   ?? new List<CurriculumItem>();
        }

        #endregion
    }
}
