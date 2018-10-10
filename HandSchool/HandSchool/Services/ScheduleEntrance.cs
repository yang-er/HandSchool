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
        /// 查找课程
        /// </summary>
        /// <param name="pred">判断条件</param>
        CurriculumItem FindItem(Predicate<CurriculumItem> pred);

        /// <summary>
        /// 查找最后一个课程
        /// </summary>
        /// <param name="pred">判断条件</param>
        CurriculumItem FindLastItem(Predicate<CurriculumItem> pred);

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
            ClassTableController Controller = new ClassTableController();

            var list = Items;
            foreach (var i in list)
                Controller.AddClass(i);
            ItemsSet=Controller.ToList();
        }
        
        public void Save()
        {
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
            Core.WriteConfig(StorageFile, Items.Serialize());
        }

        public void AddItem(CurriculumItem item)
        {
            Items.Add(item);
            ItemsSet = null;
        }

        public void RemoveItem(CurriculumItem item)
        {
            Items.Remove(item);
            ItemsSet = null;
        }

        public CurriculumItem FindItem(Predicate<CurriculumItem> pred)
        {
            return Items.Find(pred);
        }

        public CurriculumItem FindLastItem(Predicate<CurriculumItem> pred)
        {
            return Items.FindLast(pred);
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
