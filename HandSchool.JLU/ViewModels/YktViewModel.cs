﻿using HandSchool.Internals;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.ViewModels;
using HtmlAgilityPack;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HandSchool.Models;
using JsonException = Newtonsoft.Json.JsonException;
namespace HandSchool
{
    class WebSourcesParseException : Exception
    {
        public WebSourcesParseException() : base() { }

        public WebSourcesParseException(string msg) : base(msg){}
    }
}
namespace HandSchool.JLU.ViewModels
{
    static class StringE
    {
        public static string SubStr(this string str, int startIndex, int endIndex)
        {
            if (startIndex > endIndex) return "";
            return str.Substring(startIndex, endIndex - startIndex);
        }
    }
    /// <summary>
    /// 校园一卡通的视图模型。
    /// </summary>
    internal class YktViewModel : BaseViewModel
    {
        static readonly Lazy<YktViewModel> Lazy =
            new Lazy<YktViewModel>(() => new YktViewModel());
        private bool IsFirstOpen { get; set; }

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static YktViewModel Instance => Lazy.Value;
        public static WebDialogAdditionalArgs CancelLostWebAdditionalArgs { set; private get; }

        /// <summary>
        /// 建立校园一卡通的视图模型，加载命令。
        /// </summary>
        private YktViewModel()
        {
            Title = "校园一卡通";
            RecordInfo = new ObservableCollection<RecordInfo>();
            ChargeCreditCommand = new CommandAction(ProcessCharge);
            RecordFindCommand = new CommandAction(ProcessQuery);
            SetUpLostStateCommand = new CommandAction(ProcessSetLost);
            LoadBasicInfoCommand = new CommandAction(RefreshBasicInfoAsync);
            LoadTwoInfoCommand = new CommandAction(LoadTwoAsync);
            CancelLostStateCommand = new CommandAction(ProcessCancelLost);
            IsFirstOpen = true;

            BasicInfo = new CardBasicInfo();
        }

        /// <summary>
        /// 名称信息
        /// </summary>
        public SchoolCardInfoPiece NameInfo { get; set; }

        /// <summary>
        /// 学工号
        /// </summary>
        public SchoolCardInfoPiece CardId { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public SchoolCardInfoPiece Balance { get; set; }

        /// <summary>
        /// 消费记录
        /// </summary>
        public ObservableCollection<RecordInfo> RecordInfo { get; set; }

        /// <summary>
        /// 基础信息的抽象列表
        /// </summary>
        public CardBasicInfo BasicInfo { get; set; }
        
        /// <summary>
        /// 充值校园卡的命令
        /// </summary>
        public ICommand ChargeCreditCommand { get; set; }

        /// <summary>
        /// 挂失校园卡的命令
        /// </summary>
        public ICommand SetUpLostStateCommand { get; set; }

        /// <summary>
        /// 解挂校园卡的命令
        /// </summary>
        public ICommand CancelLostStateCommand { get; set; }

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

        public static Func<Task<TaskResp>> BeforeOperatingCheck { private get; set; }

        async Task<bool> BeforeOperatingCheckAsync()
        {
            IsBusy = true;
            if (BeforeOperatingCheck != null)
            {
                var msg = await BeforeOperatingCheck();
                if (!msg.IsSuccess)
                {
                    await RequestMessageAsync("错误", msg.ToString());
                    IsBusy = false;
                    return false;
                }
            }
            IsBusy = false;
            return true;
        }

        /// <summary>
        /// 加载校园卡两个信息。
        /// </summary>
        public async Task LoadTwoAsync()
        {
            if (IsBusy) return;
            if (!await BeforeOperatingCheckAsync()) return;
            IsBusy = true;
            if (!await Loader.Ykt.CheckLogin())
            {
                IsBusy = false;
                return; 
            }
            IsBusy = false;
            await RefreshBasicInfoAsync();
            await ProcessQuery();
        }

        /// <summary>
        /// 加载校园卡基本信息。
        /// </summary>
        private async Task RefreshBasicInfoAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            string last_error = null;
            try
            {
                await Loader.Ykt.BasicInfoAsync();
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    last_error = "服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync();
                else
                    last_error = "网络似乎出了点问题呢……";
            }
            finally
            {
                IsBusy = false;
                if (last_error != null)
                    await RequestMessageAsync("查询失败", last_error, "好吧");
            }
        }

        /// <summary>
        /// 加载消费记录。
        /// </summary>
        private async Task ProcessQuery()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await Loader.Ykt.QueryCost();
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    await NoticeError($"服务器的响应未知，请检查。\n{await ex.Response.ReadAsStringAsync()}");
                else
                    await NoticeError("网络似乎出了点问题呢……");
            }
            catch (JsonException ex)
            {
                await NoticeError($"服务器的响应未知，请检查。\n{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        
        /// <summary>
        /// dsf系统有键盘验证系统，需要获取一个键盘序列。
        /// 该序列为0-9的全排列的一种，此方法尽可能提前发现错误的输入，节省时间。
        /// 该方法验证了是否含有非数字、是否为0-9的全排列。
        /// </summary>
        /// <param name="keyboard"></param>
        /// <returns></returns>
        bool CheckKeyBoard(string keyboard)
        {
            if (keyboard.Length != 10) return false;
            var frq = new int[10];
            foreach (var i in keyboard)
            {
                if (i is >= '0' and <= '9')
                {
                    frq[i - '0']++;
                }
                else return false;
            }

            foreach (var f in frq)
            {
                if (f != 1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// dsf系统的操作有两层验证，一个是常规验证码，另一个是键盘位识别
        /// 此方法提供了一个处理此流程的模板
        /// </summary>
        /// <param name="getVerification">获取验证码的方法</param>
        /// <param name="afterVerification">输入验证码之后的方法</param>
        /// <param name="successMsg">操作成功之后的提示</param>
        private async Task ProcessDsfVerification(Func<Task<TaskResp>> getVerification, Func<string, string,Task<TaskResp>> afterVerification, string successMsg)
        {
            IsBusy = true;
             //开始操作，获取验证码
            var res = await getVerification();
            if (!res.IsSuccess)
            {
                await NoticeError(res.Msg.ToString());
                return;
            }

            //接收验证码源
            var msg = ((byte[], byte[])) res.Msg;
            //要求用户输入验证码
            try
            {
                var code = await RequestInputWithPicAsync("你好", "请输入验证码", "取消", "完成", msg.Item1);
                if (code == null)
                {
                    await NoticeError("不能取消输入验证码");
                    return;
                }

                string keyboard = null;
                while (keyboard == null || !CheckKeyBoard(keyboard))
                {
                    if (keyboard != null)
                    {
                        await NoticeError("键盘位识别不正确");
                    }

                    keyboard = await RequestInputWithPicAsync("你好", "请输入图中键盘的数字布局", "取消", "完成", msg.Item2);
                    if (keyboard == null)
                    {
                        await NoticeError("不能取消识别键盘位");
                        return;
                    }
                }

                var res2 = await afterVerification(code, keyboard);
                if (!res2.IsSuccess)
                {
                    var msg2 = res2.ToString();
                    //此时处于登录状态，查询密码不可能错误（除非有人在登录期间改密码？）
                    //所以服务器返回密码错误只有一个原因：键盘位识别错误
                    if (msg2.Contains("查询密码错误"))
                    {
                        await NoticeError("键盘位识别错误");
                    }
                    else
                    {
                        await NoticeError(res2.ToString());
                    }

                    return;
                }
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    await NoticeError("服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync());
                else
                    await NoticeError("网络似乎出了点问题呢……");
                return;
            }
            catch (JsonException ex)
            {
                await NoticeError("服务器的响应未知，请检查。\n" + ex.Message);
                return;
            }
            finally
            {
                IsBusy = false;
            }

            await RequestMessageAsync("成功", successMsg);
        }
        
        /// <summary>
        /// 处理充值命令。
        /// </summary>
        public async Task ProcessCharge()
        {
            //操作前的检查，检查登录等
            if (IsBusy) return;
            if (!await BeforeOperatingCheckAsync()) return;

            IsBusy = true;

            if (!await Loader.Ykt.CheckLogin())
            {
                await NoticeError("尚未登录校园卡系统\n请下拉刷新以登录");
                return;
            }

            //用户输入金额
            if (!await RequestAnswerAsync("提示",
                "向校园卡转账成功后，所转金额都会先是在过渡余额中，" +
                "在餐厅等处的卡机上进行刷卡操作后，过渡余额即会转入校园卡。" +
                "是否继续充值？", "否", "是"))
            {
                IsBusy = false; return;
            }

            var money = await RequestInputAsync("提示", "请输入充值金额：", "取消", "继续");
            if (string.IsNullOrWhiteSpace(money))
            {
                IsBusy = false;
                return;
            }
            
            //检查金额
            try
            {
                var moneyNum = double.Parse(money);
                if (moneyNum > 200 || moneyNum < 0.01)
                    await NoticeError("单笔充值限定在0.01~200.00之间。");
            }
            catch (FormatException)
            {
                await NoticeError("充值金额的格式错误，请检查后重试。");
                return;
            }

            await ProcessDsfVerification(
                Loader.Ykt.PreChargeMoney,
                async (c, k) => await Loader.Ykt.ChargeMoney(double.Parse(money), c, k),
                $"成功充值了{money}元。"
            );
        }

        /// <summary>
        /// 处理挂失命令。
        /// </summary>
        public async Task ProcessSetLost()
        {
            if (IsBusy) return;
            
            if (!await BeforeOperatingCheckAsync()) return;

            IsBusy = true;
            if (!await Loader.Ykt.CheckLogin())
            {
                await NoticeError("尚未登录校园卡系统\n请下拉刷新以登录");
                return;
            }
            if (!BasicInfo.Lost.Description.Contains("正常"))
            {
                await NoticeError("卡片已经挂失，无需操作");
                return;
            }

            if (!await RequestAnswerAsync("提示",
                "挂失后校园卡会暂时无法使用。" +
                "如果需要解除挂失，可以登录http://xyk.jlu.edu.cn，或者前往校园卡服务中心。" +
                "确认挂失吗？", "否", "是"))
            {
                IsBusy = false;return;
            }
            
            await ProcessDsfVerification(
                Loader.Ykt.PreSetLost,
                Loader.Ykt.SetLost,
                "校园卡资金似乎暂时安全了呢（大雾"
            );
        }

        ///<summary>
        ///处理解挂
        /// </summary>
        public async Task ProcessCancelLost()
        {
            if (IsBusy) return;
            
            if (!await BeforeOperatingCheckAsync()) return;

            IsBusy = true;
            if (!await Loader.Ykt.CheckLogin())
            {
                await NoticeError("尚未登录校园卡系统\n请下拉刷新以登录");
                return;
            }
            if (BasicInfo.Lost.Description.Contains("正常"))
            {
                await RequestMessageAsync("提示", "卡片没有挂失，无需解挂");
                IsBusy = false;
                return;
            }

            if (!await RequestAnswerAsync("提示",
                "校园卡找到了？(滑稽)", "否", "是"))
            {
                IsBusy = false;
                return;
            }
            
            try
            {
                IsBusy = true;
                var vpn = Loader.Vpn is {IsLogin: true};
                var url = vpn ? "https://vpns.jlu.edu.cn/http/77726476706e69737468656265737421e8ee4ad22d3c7d1e7b0c9ce29b5b/homeLogin.action" 
                    : "http://xyk.jlu.edu.cn/homeLogin.action";

                if (Core.Platform.RuntimeName == "Android")
                {
                    if (vpn)
                    {
                        CancelLostWebAdditionalArgs.Cookies.Add(("https://vpns.jlu.edu.cn", new System.Net.Cookie("remember_token", HandSchool.JLU.Loader.Vpn.RememberToken, "/")));
                    }
                    await RequestWebDialogAsync("请在网页操作","", url, "", "退出", false, false, null, CancelLostWebAdditionalArgs);
                }
                IsBusy = false;
            }
            catch (Exception error)
            {
                await NoticeError(error.Message);
            }
        }
        
        /// <summary>
        /// 解析基础信息
        /// </summary>
        /// <param name="htmlSources">需要解析的网页</param>
        public void ParseBasicInfo(string htmlSources)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlSources);
            var ps = html.DocumentNode.SelectNodes("//div[@class='userInfoR']/p/em");
            if(ps == null)
            {
                throw new WebSourcesParseException();
            }
            var infoList = new List<string>();
            foreach (var p in ps)
            {
                infoList.Add(p.InnerHtml.Trim());
            }
            BasicInfo.NameInfo.Description = infoList[0];
            BasicInfo.CardId.Description = infoList[1];
            BasicInfo.InternalUID.Description = infoList[2];
            BasicInfo.Balance.Description = infoList[3];
            BasicInfo.CurGd.Description = infoList[4];
            BasicInfo.Lost.Description = infoList[5];
            BasicInfo.Frozen.Description = infoList[6];
        }

        /// <summary>
        /// 程序声明周期中第一次打开一卡通功能时执行。
        /// </summary>
        public async Task FirstOpen()
        {
            if (!IsFirstOpen) return;
            IsFirstOpen = false;

            if (!await BeforeOperatingCheckAsync()) return;
            
            if (await Loader.Ykt.CheckLogin())
            {
                await LoadTwoAsync();
            }
        }
    }

}