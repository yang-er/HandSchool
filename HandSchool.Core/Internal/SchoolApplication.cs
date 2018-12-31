using System;
using HandSchool.Models;
using HandSchool.Services;
using System.Collections.Generic;

namespace HandSchool.Internal
{
    public class SchoolApplication
    {
        /// <summary>
        /// 最终使用的加载器
        /// </summary>
        public ISchoolWrapper Loader { get; set; }

        /// <summary>
        /// 学校的教务中心服务
        /// </summary>
        public ISchoolSystem Service => Loader.Service.Value;

        /// <summary>
        /// 获取绩点的入口点
        /// </summary>
        public IGradeEntrance GradePoint => Loader.GradePoint?.Value;

        /// <summary>
        /// 获取课程表的入口点
        /// </summary>
        public IScheduleEntrance Schedule => Loader.Schedule.Value;

        /// <summary>
        /// 获取系统消息的入口点
        /// </summary>
        public IMessageEntrance Message => Loader.Message?.Value;

        /// <summary>
        /// 获取消息更新的入口点
        /// </summary>
        public IFeedEntrance Feed => Loader.Feed?.Value;

        /// <summary>
        /// 每天有多少节标准课时
        /// </summary>
        public int DailyClassCount;

        /// <summary>
        /// 信息查询入口点工厂类列表
        /// </summary>
        public List<InfoEntranceGroup> InfoEntrances = new List<InfoEntranceGroup>();
        
        /// <summary>
        /// 登录状态发生改变
        /// </summary>
        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        /// <summary>
        /// 将加载器作为依赖服务注入。
        /// </summary>
        /// <param name="wrapper">加载器</param>
        public void InjectService(ISchoolWrapper wrapper)
        {
            Loader = wrapper;
            Loader.NoticeChange = (s, e) => LoginStateChanged?.Invoke(s, e);
        }
    }
}
