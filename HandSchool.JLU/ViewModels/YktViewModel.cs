using HandSchool.Internals;
using HandSchool.JLU.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
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
        static readonly Lazy<YktViewModel> Lazy =
            new Lazy<YktViewModel>(() => new YktViewModel());
        private bool IsFirstOpen { get; set; }

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static YktViewModel Instance => Lazy.Value;

        /// <summary>
        /// 建立校园一卡通的视图模型，加载命令。
        /// </summary>
        private YktViewModel()
        {
            Title = "校园一卡通";
            PickCardInfo = new ObservableCollection<PickCardInfo>();
            RecordInfo = new ObservableCollection<RecordInfo>();
            LoadPickCardInfoCommand = new CommandAction(GetPickCardInfo);
            ChargeCreditCommand = new CommandAction(ProcessCharge);
            RecordFindCommand = new CommandAction(ProcessQuery);
            SetUpLostStateCommand = new CommandAction(ProcessSetLost);
            LoadBasicInfoCommand = new CommandAction(RefreshBasicInfoAsync);
            IsFirstOpen = true;
            
            BasicInfo = new List<SchoolCardInfoPiece>
            {
                new SchoolCardInfoPiece("姓名", "未知"),
                new SchoolCardInfoPiece("学工号", "不知道"),
                new SchoolCardInfoPiece("校园卡余额", "不清楚"),
                new SchoolCardInfoPiece("银行卡号", "不知晓"),
                new SchoolCardInfoPiece("当前过渡余额", "听不清"),
                new SchoolCardInfoPiece("上次过渡余额", "看不见"),
                new SchoolCardInfoPiece("挂失状态", "可能正确吧"),
                new SchoolCardInfoPiece("冻结状态", "大概正常吧"),
                new SchoolCardInfoPiece("身份类型", "你先登录"),
                new SchoolCardInfoPiece("部门名称", "猜不透"),
            };
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
        public List<SchoolCardInfoPiece> BasicInfo { get; set; }

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
        /// 加载校园卡基本信息。
        /// </summary>
        public async Task RefreshBasicInfoAsync()
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
        public async Task ProcessQuery()
        {
            if (IsBusy) return;
            IsBusy = true;
            string last_error = null;

            try
            {
                await Loader.Ykt.QueryCost();
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    last_error = "服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync();
                else
                    last_error = "网络似乎出了点问题呢……";
            }
            catch (JsonException ex)
            {
                last_error = "服务器的响应未知，请检查。\n" + ex.Message;
            }
            finally
            {
                IsBusy = false;
                if (last_error != null)
                    await RequestMessageAsync("查询失败", last_error, "好吧");
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
                await Loader.Ykt.GetPickCardInfo();
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
        /// <param name="money">充值金额</param>
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
            string last_error = null;

            try
            {
                if (!await Loader.Ykt.ChargeMoney(money))
                {
                    last_error = Loader.Ykt.LastReport;
                }
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    last_error = "服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync();
                else
                    last_error = "网络似乎出了点问题呢……";
            }
            catch (OverflowException)
            {
                last_error = "单笔充值限定在0.01~200.00之间。";
            }
            catch (FormatException)
            {
                last_error = "充值金额的格式错误，请检查后重试。";
            }
            catch (JsonException ex)
            {
                last_error = "服务器的响应未知，请检查。\n" + ex.Message;
            }
            finally
            {
                IsBusy = false;
                if (last_error != null)
                    await RequestMessageAsync("充值失败", last_error);
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
            string last_error = null;

            try
            {
                if (!await Loader.Ykt.SetLost())
                {
                    last_error = Loader.Ykt.LastReport;
                }
            }
            catch (WebsException ex)
            {
                if (ex.Status == WebStatus.MimeNotMatch)
                    last_error = "服务器的响应未知，请检查。\n" + await ex.Response.ReadAsStringAsync();
                else
                    last_error = "网络似乎出了点问题呢……";
            }
            catch (JsonException ex)
            {
                last_error = "服务器的响应未知，请检查。\n" + ex.Message;
            }
            finally
            {
                IsBusy = false;
                if (last_error != null)
                    await RequestMessageAsync("挂失失败", last_error);
                else
                    await RequestMessageAsync("挂失成功", "校园卡资金似乎暂时安全了呢（大雾");
            }
        }

        /// <summary>
        /// 解析基础信息
        /// </summary>
        /// <param name="html">需要解析的网页</param>
        public void ParseBasicInfo(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");

            foreach (var info in BasicInfo)
            {
                if (info.Command != null) continue;
                info.Description = Regex.Match(html, @"<td class=""first"">"
                    + info.Title + @"</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            }
        }

        /// <summary>
        /// 程序声明周期中第一次打开一卡通功能时执行。
        /// </summary>
        public async Task FirstOpen()
        {
            if (!IsFirstOpen) return;
            IsFirstOpen = false;

            if (await Loader.Ykt.RequestLogin())
            {
                await Core.Platform.EnsureOnMainThread(async () =>
                {
                    await RefreshBasicInfoAsync();
                    await ProcessQuery();
                    await GetPickCardInfo();
                });
            }
        }
    }
}