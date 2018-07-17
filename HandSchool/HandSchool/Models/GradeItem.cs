using System;
using System.Collections.Specialized;

namespace HandSchool.Models
{
    /// <summary>
    /// 成绩信息
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
}
