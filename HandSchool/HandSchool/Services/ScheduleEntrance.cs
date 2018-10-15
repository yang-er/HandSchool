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
    }
}
