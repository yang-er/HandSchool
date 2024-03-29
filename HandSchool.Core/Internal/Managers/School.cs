﻿using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HandSchool.Internals
{
    public delegate Task<TaskResp> ActioningDelegate(object sender, ActioningEventArgs args);
    public class SchoolApplication
    {
        public static event ActioningDelegate Actioning;

        public static async Task<List<TaskResp>> SendActioning(object sender, ActioningEventArgs args)
        {
            if (Actioning is null) return new List<TaskResp>();
            var res = new List<TaskResp>();
            foreach (var task in Actioning.GetInvocationList().OfType<ActioningDelegate>())
            {
                res.Add(await task(sender, args));
            }

            return res;
        }
        
        /// <summary>
        /// 加载开始时执行
        /// </summary>
        public static event EventHandler<EventArgs> Loading;
        
        /// <summary>
        /// 加载结束后执行
        /// </summary>
        public static event EventHandler<EventArgs> Loaded;
        
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
        public ObservableCollection<InfoEntranceGroup> InfoEntrances = new ObservableCollection<InfoEntranceGroup>();
        
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
            Registry.RegisterTypes();
        }

        public static void OnLoading(ISchoolWrapper loader, EventArgs args) => Loading?.Invoke(loader, args);
        public static void OnLoaded(ISchoolWrapper loader, EventArgs args) => Loaded?.Invoke(loader, args);
    }
}