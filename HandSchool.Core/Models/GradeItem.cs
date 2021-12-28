using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HandSchool.Models
{
    public interface IBasicGradeItem
    {
        /// <summary>
        /// 成绩名称
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 成绩分数
        /// </summary>
        string FirstScore { get; }
        string HighestScore { get; }
        
        /// <summary>
        /// 成绩点
        /// </summary>
        string FirstPoint { get; }
        
        string HightestPoint { get; }
        
        /// <summary>
        /// 成绩类型
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 学分
        /// </summary>
        string Credit { get; }
        /// <summary>
        /// 是否通过
        /// </summary>
        bool IsPassed { get; }
        
        /// <summary>
        /// 选课学期
        /// </summary>
        string Term { get; }
        
        /// <summary>
        /// 成绩类型的颜色
        /// </summary>
        Xamarin.Forms.Color TypeColor { get; }
        /// <summary>
        /// 获得展示的字符串
        /// </summary>
        string Detail { get; }
    }
    
    /// <summary>
    /// 实现成绩信息展示的储存类接口。
    /// </summary>
    public interface IGradeItem : IBasicGradeItem
    {
        /// <summary>
        /// 是否重修
        /// </summary>
        bool ReSelect { get; }

        /// <summary>
        /// 出分日期
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        /// 附加信息
        /// </summary>
        NameValueCollection Attach { get; }

        /// <summary>
        /// 全班的成绩分布
        /// </summary>
        IEnumerable<Entry> GetGradeDistribute();
        
    }
}