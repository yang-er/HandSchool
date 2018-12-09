using HandSchool.Internal;
using HandSchool.JLU.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using JsonException = Newtonsoft.Json.JsonException;
using WebException = System.Net.WebException;

namespace HandSchool.JLU.ViewModels
{
    class YktViewModel : BaseViewModel
    {
        static YktViewModel _instance = null;

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static YktViewModel Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new YktViewModel();
                return _instance;
            }
        }

        /// <summary>
        /// 建立校园一卡通的视图模型，加载命令。
        /// </summary>
        private YktViewModel()
        {
            Title = "校园一卡通";
            BasicInfo = new SchoolCardInfo();
            PickCardInfo = new ObservableCollection<PickCardInfo>();
            RecordInfo = new ObservableCollection<RecordInfo>();
            LoadPickCardInfoCommand = new Command(async() => await GetPickCardInfo());
            ChargeCreditCommand = new Command(async (obj) => await ProcessCharge(obj));
            RecordFindCommand = new Command(async () => await ProcessQuery());
            SetUpLostStateCommand = new Command(async () => await ProcessSetLost());
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
        /// 基础信息
        /// </summary>
        public SchoolCardInfo BasicInfo { get; set; }

        /// <summary>
        /// 加载拾卡信息的命令
        /// </summary>
        public Command LoadPickCardInfoCommand { get; set; }

        /// <summary>
        /// 充值校园卡的命令
        /// </summary>
        public Command ChargeCreditCommand { get; set; }

        /// <summary>
        /// 挂失校园卡的命令
        /// </summary>
        public Command SetUpLostStateCommand { get; set; }

        /// <summary>
        /// 加载消费记录的命令
        /// </summary>
        public Command RecordFindCommand { get; set; }

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
            catch (WebException)
            {
                last_error = "网络似乎出了点问题呢……";
            }
            catch (JsonException ex)
            {
                last_error = "服务器的响应未知，请检查。\n" + ex.Message;
            }
            catch (ContentAcceptException ex)
            {
                last_error = "服务器的响应未知，请检查。\n" + ex.Current;
            }
            finally
            {
                IsBusy = false;
                if (last_error != null)
                    await ShowMessage("查询失败", last_error);
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
            catch (WebException)
            {
                await ShowMessage("拾卡信息", "拾卡信息加载失败，请检查网络连接。");
            }
            catch (ContentAcceptException)
            {
                await ShowMessage("拾卡信息", "拾卡信息加载失败，可能是软件过老。");
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
        public async Task ProcessCharge(object money)
        {
            if (IsBusy) return;
            if (!await ShowAskMessage("提示", 
                "向校园卡转账成功后，所转金额都会先是在过渡余额中，" +
                "在餐厅等处的卡机上进行刷卡操作后，过渡余额即会转入校园卡。" +
                "是否继续充值？", "否", "是")) return;

            IsBusy = true;
            string last_error = null;

            try
            {
                if (!await Loader.Ykt.ChargeMoney(money as string))
                {
                    last_error = Loader.Ykt.LastReport;
                }
            }
            catch (WebException)
            {
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
            catch (ContentAcceptException ex)
            {
                last_error = "服务器的响应未知，请检查。\n" + ex.Current;
            }
            finally
            {
                IsBusy = false;
                if (last_error != null)
                    await ShowMessage("充值失败", last_error);
                else
                    await ShowMessage("充值成功", "成功充值了" + money as string + "元。");
            }
        }

        /// <summary>
        /// 处理挂失命令。
        /// </summary>
        public async Task ProcessSetLost()
        {
            if (IsBusy) return;
            if (!await ShowAskMessage("提示",
                "挂失后，您的校园卡会暂时无法使用。" +
                "如果需要接触挂失，可以登录xyk.jlu.edu.cn，或者前往校园卡服务中心。" +
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
            catch (WebException)
            {
                last_error = "网络似乎出了点问题呢……";
            }
            catch (JsonException ex)
            {
                last_error = "服务器的响应未知，请检查。\n" + ex.Message;
            }
            catch (ContentAcceptException ex)
            {
                last_error = "服务器的响应未知，请检查。\n" + ex.Current;
            }
            finally
            {
                IsBusy = false;
                if (last_error != null)
                    await ShowMessage("挂失失败", last_error);
                else
                    await ShowMessage("挂失成功", "校园卡资金似乎暂时安全了呢（大雾");
            }
        }
    }
}
