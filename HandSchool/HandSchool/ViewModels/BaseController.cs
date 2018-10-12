﻿using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.Internal
{
    public abstract class BaseController : BaseViewModel, IWebEntrance, IViewResponse
    {
        /// <summary>
        /// HTML浏览器的执行接口
        /// </summary>
        public Action<string> Evaluate { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        public List<InfoEntranceMenu> Menu { get; set; } = new List<InfoEntranceMenu>();

        /// <summary>
        /// 弹出
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="cancel">取消</param>
        /// <param name="destruction">删除</param>
        /// <param name="buttons">按钮</param>
        /// <returns>按下的按钮标签</returns>
        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return Core.EnsureOnMainThread(() => View.DisplayActionSheet(title, cancel, destruction, buttons));
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="data">消息</param>
        public abstract Task Receive(string data);

        /// <summary>
        /// 弹出询问框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="description">消息正文</param>
        /// <param name="cancel">取消按钮文字</param>
        /// <param name="accept">确定按钮文字</param>
        /// <returns>是否确定</returns>
        public Task<bool> ShowAskMessage(string title, string description, string cancel, string accept)
        {
            return Core.EnsureOnMainThread(() => View.ShowAskMessage(title, description, cancel, accept));
        }

        /// <summary>
        /// 弹出消息对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="message">消息正文</param>
        /// <param name="button">按钮文字</param>
        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Core.EnsureOnMainThread(() => View.ShowMessage(title, message, button));
        }
    }
}
