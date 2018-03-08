using System.Collections.Generic;

namespace HandSchool.Internal
{

    public interface ICurriculumParser<T>
    {
        void ParseTable(List<T> list, string inputData = "", bool append = false);
        CurriculumSmall SmallInfo(T curriculum);
        CurriculumDetail DetailInfo(T curriculum);
        bool IfShow(int week, T curriculum);
    }

    public class CurriculumSchedule<T>
    {

        private ICurriculumParser<T> Parser;
        public List<T> Table;

        public CurriculumDetail Detail(T item)
        {
            return Parser.DetailInfo(item);
        }

        public List<CurriculumSmall> RenderWeek(int week, bool showAll)
        {
            List<CurriculumSmall> ret = new List<CurriculumSmall>();
            foreach (T item in Table)
            {
                if (showAll || Parser.IfShow(week, item))
                    ret.Add(Parser.SmallInfo(item));
            }
            return ret;
        }

    }

    /// <summary>
    /// 是否显示此课程。
    /// </summary>
    /// <param name="week">当前周</param>
    /// <param name="curriculum">课程</param>
    public delegate bool IfShow<T>(int week, T curriculum);

    /// <summary>
    /// 课程简略信息，显示在周课程表上。
    /// </summary>
    public struct CurriculumSmall
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public int Weekday;
        public int From;
        public int To;
    }

    /// <summary>
    /// 课程详细信息，显示在课程详情处。
    /// </summary>
    public struct CurriculumDetail
    {
        public string Name;
        public string Position;
        public string Teacher;
        public int Weekday;
        public int From;
        public int To;
    }

}
