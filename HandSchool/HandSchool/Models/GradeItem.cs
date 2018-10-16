using System;
using System.Collections.Specialized;

namespace HandSchool.Models
{
    /// <summary>
    /// 实现成绩信息展示的储存类接口。
    /// </summary>
    public interface IGradeItem
    {
        /// <summary>
        /// 成绩名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 成绩分数
        /// </summary>
        string Score { get; }

        /// <summary>
        /// 成绩点
        /// </summary>
        string Point { get; }

        /// <summary>
        /// 成绩类型
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 学分
        /// </summary>
        string Credit { get; }

        /// <summary>
        /// 是否重修
        /// </summary>
        bool ReSelect { get; }

        /// <summary>
        /// 是否通过
        /// </summary>
        bool Pass { get; }

        /// <summary>
        /// 选课学期
        /// </summary>
        string Term { get; }

        /// <summary>
        /// 出分日期
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        /// 附加信息
        /// </summary>
        NameValueCollection Attach { get; }

        /// <summary>
        /// 获得展示的字符串
        /// </summary>
        string Show { get; }
    }

    /// <summary>
    /// 保存字符串，用于展示GPA。
    /// </summary>
    public class GPAItem : IGradeItem
    {
        public string Name => "GPA统计";

        public string Score => "";
        public string Point => "";
        public string Type => "";
        public string Credit => "";
        public string Term => "";
        public bool ReSelect => false;
        public bool Pass => true;
        public NameValueCollection Attach => null;

        public DateTime Date { get; }
        public string Show { get; }

        /// <summary>
        /// 建立新的GPA项目。
        /// </summary>
        /// <param name="to_show">将被展示的内容。</param>
        public GPAItem(string to_show)
        {
            Show = to_show;
            Date = DateTime.Now;
        }
    }
}
