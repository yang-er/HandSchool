using HandSchool.Models;
using System;
using System.Collections.Generic;

namespace HandSchool.Services
{
    /// <summary>
    /// 加载学校的接口
    /// </summary>
    public interface ISchoolWrapper
    {
        /// <summary>
        /// 学校的教务中心服务
        /// </summary>
        Lazy<ISchoolSystem> Service { get; }

        /// <summary>
        /// 获取绩点的入口点
        /// </summary>
        Lazy<IGradeEntrance> GradePoint { get; }

        /// <summary>
        /// 获取课程表的入口点
        /// </summary>
        Lazy<IScheduleEntrance> Schedule { get; }

        /// <summary>
        /// 获取系统消息的入口点
        /// </summary>
        Lazy<IMessageEntrance> Message { get; }

        /// <summary>
        /// 获取消息更新的入口点
        /// </summary>
        Lazy<IFeedEntrance> Feed { get; }

        /// <summary>
        /// 登录状态更改的事件传递
        /// </summary>
        EventHandler<LoginStateEventArgs> NoticeChange { get; set; }

        /// <summary>
        /// 在加载之前运行
        /// </summary>
        void PreLoad();

        /// <summary>
        /// 加载完后运行
        /// </summary>
        void PostLoad();

        /// <summary>
        /// 学校的名称
        /// </summary>
        string SchoolName { get; }
        
        /// <summary>
        /// 学校的简写字母
        /// </summary>
        string SchoolId { get; }

        /// <summary>
        /// 保存设置
        /// </summary>
        void SaveSettings(ISchoolSystem system);

        /// <summary>
        /// 使用的储存文件。将在重置时清除。
        /// </summary>
        List<string> RegisteredFiles { get; }

        /// <summary>
        /// 学校欢迎界面的类型。
        /// </summary>
        Type HelloPage { get; }
    }
}
