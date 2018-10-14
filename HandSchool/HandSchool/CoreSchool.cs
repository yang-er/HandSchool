using HandSchool.Models;
using HandSchool.Services;
using System.Collections.Generic;

namespace HandSchool
{
    public sealed partial class Core
    {
        /// <summary>
        /// 学校的教务中心服务
        /// </summary>
        public ISchoolSystem Service;

        /// <summary>
        /// 获取绩点的入口点
        /// </summary>
        public IGradeEntrance GradePoint;

        /// <summary>
        /// 获取课程表的入口点
        /// </summary>
        public IScheduleEntrance Schedule;

        /// <summary>
        /// 获取系统消息的入口点
        /// </summary>
        public IMessageEntrance Message;

        /// <summary>
        /// 获取消息更新的入口点
        /// </summary>
        public IFeedEntrance Feed;

        /// <summary>
        /// 每天有多少节标准课时
        /// </summary>
        public int DailyClassCount;

        /// <summary>
        /// 信息查询入口点工厂类列表
        /// </summary>
        public List<InfoEntranceGroup> InfoEntrances = new List<InfoEntranceGroup>();
    }
}
