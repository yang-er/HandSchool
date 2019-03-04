﻿using HandSchool.Internals;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using JsonException = Newtonsoft.Json.JsonException;

namespace HandSchool.JLU.ViewModels
{
    /// <summary>
    /// 校园一卡通的视图模型。
    /// </summary>
    internal class YktViewModel : BaseViewModel
    {
        private bool IsFirstOpen { get; set; }
        private SchoolCard Service { get; }

        /// <summary>
        /// 建立校园一卡通的视图模型，加载命令。
        /// </summary>
        private YktViewModel(SchoolCard service)
        {
            Service = service;

            Title = "校园一卡通";
            BasicInfo = new CardBasicInfo();
            PickCardInfo = new ObservableCollection<PickCardInfo>();
            RecordInfo = new ObservableCollection<RecordInfo>();
            LoadPickCardInfoCommand = new CommandAction(GetPickCardInfo);
            ChargeCreditCommand = new CommandAction(ProcessCharge);
            RecordFindCommand = new CommandAction(ProcessQuery);
            SetUpLostStateCommand = new CommandAction(ProcessSetLost);
            LoadBasicInfoCommand = new CommandAction(RefreshBasicInfoAsync);
            LoadTwoInfoCommand = new CommandAction(LoadTwoAsync);
            IsFirstOpen = true;
        }

        /// <summary>
        /// 拾卡信息
        /// </summary>
        public ObservableCollection<PickCardInfo> PickCardInfo { get; set; }
        
        /// <summary>
        /// 消费记录
        /// </summary>
        public ObservableCollection<RecordInfo> RecordInfo { get; set; }
        
        /// <summary>
        /// 基础信息的抽象列表
        /// </summary>
        public CardBasicInfo BasicInfo { get; set; }

        /// <summary>
        /// 加载拾卡信息的命令
        /// </summary>
        public ICommand LoadPickCardInfoCommand { get; set; }

        /// <summary>
        /// 充值校园卡的命令
        /// </summary>
        public ICommand ChargeCreditCommand { get; set; }

        /// <summary>
        /// 挂失校园卡的命令
        /// </summary>
        public ICommand SetUpLostStateCommand { get; set; }

        /// <summary>
        /// 加载消费记录的命令
        /// </summary>
        public ICommand RecordFindCommand { get; set; }

        /// <summary>
        /// 加载基本信息的命令
        /// </summary>
        public ICommand LoadBasicInfoCommand { get; set; }

        /// <summary>
        /// 加载基本信息的命令
        /// </summary>
        public ICommand LoadTwoInfoCommand { get; set; }

        /// <summary>
        /// 加载校园卡两个信息。
        /// </summary>
        public async Task LoadTwoAsync()
        {
            await RefreshBasicInfoAsync();
            await ProcessQuery();
        }

        /// <summary>
        /// 加载校园卡基本信息。
        /// </summary>
        public async Task RefreshBasicInfoAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            string lastError = null;

            try
            {
                await Service.BasicInfoAsync(BasicInfo);
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    lastError = "服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync();
                else
                    lastError = "网络似乎出了点问题呢……";
            }
            finally
            {
                IsBusy = false;
                if (lastError != null)
                    await RequestMessageAsync("查询失败", lastError, "好吧");
            }
        }

        /// <summary>
        /// 加载消费记录。
        /// </summary>
        public async Task ProcessQuery()
        {
            if (IsBusy) return;
            IsBusy = true;
            string lastError = null;

            try
            {
                var enumerable = await Service.QueryCost();
                RecordInfo.Clear();
                foreach (var e in enumerable)
                    RecordInfo.Add(e);
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    lastError = "服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync();
                else
                    lastError = "网络似乎出了点问题呢……";
            }
            catch (JsonException ex)
            {
                lastError = "服务器的响应未知，请检查。\n" + ex.Message;
            }
            finally
            {
                IsBusy = false;
                if (lastError != null)
                    await RequestMessageAsync("查询失败", lastError, "好吧");
            }
        }

        /// <summary>
        /// 获取拾卡信息。
        /// </summary>
        public async Task GetPickCardInfo()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var enumerable = await Service.GetPickCardInfo();
                PickCardInfo.Clear();
                foreach (var e in enumerable)
                    PickCardInfo.Add(e);
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    await RequestMessageAsync("拾卡信息", "拾卡信息加载失败，可能是软件过老。");
                else
                    await RequestMessageAsync("拾卡信息", "拾卡信息加载失败，请检查网络连接。");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// 处理充值命令。
        /// </summary>
        public async Task ProcessCharge()
        {
            if (IsBusy) return;
            if (!await RequestAnswerAsync("提示", 
                "向校园卡转账成功后，所转金额都会先是在过渡余额中，" +
                "在餐厅等处的卡机上进行刷卡操作后，过渡余额即会转入校园卡。" +
                "是否继续充值？", "否", "是")) return;
            string money = await RequestInputAsync("提示", "请输入充值金额：", "取消", "继续");
            if (money is null) return;

            IsBusy = true;
            string lastError = null;

            try
            {
                if (!await Service.ChargeMoney(money))
                {
                    lastError = Service.LastReport;
                }
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    lastError = "服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync();
                else
                    lastError = "网络似乎出了点问题呢……";
            }
            catch (OverflowException)
            {
                lastError = "单笔充值限定在0.01~200.00之间。";
            }
            catch (FormatException)
            {
                lastError = "充值金额的格式错误，请检查后重试。";
            }
            catch (JsonException ex)
            {
                lastError = "服务器的响应未知，请检查。\n" + ex.Message;
            }
            finally
            {
                IsBusy = false;
                if (lastError != null)
                    await RequestMessageAsync("充值失败", lastError);
                else
                    await RequestMessageAsync("充值成功", "成功充值了" + money + "元。");
            }
        }

        /// <summary>
        /// 处理挂失命令。
        /// </summary>
        public async Task ProcessSetLost()
        {
            if (IsBusy) return;
            if (!await RequestAnswerAsync("提示",
                "挂失后，您的校园卡会暂时无法使用。" +
                "如果需要解除挂失，可以登录xyk.jlu.edu.cn，或者前往校园卡服务中心。" +
                "确认挂失吗？", "否", "是")) return;

            IsBusy = true;
            string lastError = null;

            try
            {
                if (!await Service.SetLost())
                {
                    lastError = Service.LastReport;
                }
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    lastError = "服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync();
                else
                    lastError = "网络似乎出了点问题呢……";
            }
            catch (JsonException ex)
            {
                lastError = "服务器的响应未知，请检查。\n" + ex.Message;
            }
            finally
            {
                IsBusy = false;
                if (lastError != null)
                    await RequestMessageAsync("挂失失败", lastError);
                else
                    await RequestMessageAsync("挂失成功", "校园卡资金似乎暂时安全了呢（大雾");
            }
        }
        
        /// <summary>
        /// 程序声明周期中第一次打开一卡通功能时执行。
        /// </summary>
        public async Task FirstOpen()
        {
            if (!IsFirstOpen) return;
            IsFirstOpen = false;

            if (await Service.RequestLogin())
            {
                await RefreshBasicInfoAsync();
                await ProcessQuery();
                // await GetPickCardInfo();
            }
        }
    }
}