using HandSchool.Internal;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.Services
{
    /// <summary>
    /// 课程表获取
    /// </summary>
    public interface IScheduleEntrance : ISystemEntrance
    {
        /// <summary>
        /// 下一节课
        /// </summary>
        int ClassNext { get; }

        /// <summary>
        /// 添加课程
        /// </summary>
        /// <param name="item">课程</param>
        void AddItem(CurriculumItem item);

        /// <summary>
        /// 删除课程
        /// </summary>
        /// <param name="item">课程</param>
        void RemoveItem(CurriculumItem item);

        /// <summary>
        /// 渲染课程表
        /// </summary>
        /// <param name="week">第几周</param>
        /// <param name="list">输出列表</param>
        void RenderWeek(int week, out IEnumerable<CurriculumItemBase> list);

        /// <summary>
        /// 保存课程表
        /// </summary>
        void Save();
    }

    /// <summary>
    /// 实现 <see cref="IScheduleEntrance"/> 大部分方法的基类
    /// </summary>
    public abstract class ScheduleEntranceBase : IScheduleEntrance
    {
        public abstract string ScriptFileUri { get; }
        public abstract bool IsPost { get; }
        public abstract string PostValue { get; }
        public abstract string StorageFile { get; }
        public string LastReport { get; set; }
        public abstract Task Execute();
        public abstract void Parse();
        
        public abstract int ClassNext { get; }
        public List<CurriculumItem> Items { get; }
        public List<CurriculumItemSet2> ItemsSet { get; protected set; }
        
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

        protected virtual void FetchItemsSet()
        {
            throw new NotImplementedException();
        }
        
        public void Save()
        {
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
            Core.WriteConfig(StorageFile, Items.Serialize());
        }

        public void AddItem(CurriculumItem item)
        {
            Items.Add(item);
        }

        public void RemoveItem(CurriculumItem item)
        {
            Items.Remove(item);
        }

        public ScheduleEntranceBase()
        {
            LastReport = Core.ReadConfig(StorageFile);
            if (LastReport != "")
                Items = LastReport.ParseJSON<List<CurriculumItem>>();
            else
                Items = new List<CurriculumItem>();
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
        }
    }
}
