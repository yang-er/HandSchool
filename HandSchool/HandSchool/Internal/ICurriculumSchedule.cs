using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HandSchool.Internal
{
    public class CurriculumSchedule
    {
        public static List<ICurriculumItem> Table = new List<ICurriculumItem>();
        public static ObservableCollection<ScheduleViewItem> List = new ObservableCollection<ScheduleViewItem>();

        public static event EventHandler OnTableRefresh;

        public static void RenderWeek(int week, bool showAll = false)
        {
            if (showAll)
                throw new NotImplementedException();

            List.Clear();
            foreach (ICurriculumItem item in Table)
            {
                if (showAll || item.IfShow(week))
                    List.Add(new ScheduleViewItem(item));
            }
        }
    }

    public class ScheduleViewItem
    {
        public ICurriculumItem Source { get; }

        public ScheduleViewItem(ICurriculumItem source)
        {
            Source = source;
        }
    }
}
