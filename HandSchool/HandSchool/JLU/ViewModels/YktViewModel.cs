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
        public static YktViewModel Instance { get; private set; }
        public ObservableCollection<PickCardInfo> PickCardInfo { get; set; }
        public ObservableCollection<RecordInfo> RecordInfo { get; set; }
        public SchoolCardInfo BasicInfo { get; set; }

        public Command LoadPickCardInfoCommand { get; set; }
        public Command ChargeCreditCommand { get; set; }
        public Command RecordFindCommand { get; set; }

        public YktViewModel()
        {
            System.Diagnostics.Debug.Assert(Instance is null);
            Instance = this;
            Title = "校园一卡通";
            BasicInfo = new SchoolCardInfo();
            PickCardInfo = new ObservableCollection<PickCardInfo>();
            RecordInfo = new ObservableCollection<RecordInfo>();
            LoadPickCardInfoCommand = new Command(async() => await GetPickCardInfo());
            ChargeCreditCommand = new Command(async (obj) => await ProcessCharge(obj));
            RecordFindCommand = new Command(async () => await ProcessQuery());
        }

        public async Task ProcessQuery()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载消费信息……");
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
                SetIsBusy(false);
                if (last_error != null)
                    await View.ShowMessage("查询失败", last_error);
            }
        }

        public async Task GetPickCardInfo()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载拾卡信息……");

            try
            {
                await Loader.Ykt.GetPickCardInfo();
            }
            catch (WebException)
            {
                await View.ShowMessage("拾卡信息", "拾卡信息加载失败，请检查网络连接。");
            }
            catch (ContentAcceptException)
            {
                await View.ShowMessage("拾卡信息", "拾卡信息加载失败，可能是软件过老。");
            }
            finally
            {
                SetIsBusy(false);
            }
        }

        public async Task ProcessCharge(object money)
        {
            if (IsBusy) return;
            if (!await View.ShowAskMessage("提示", "向校园卡转账成功后，所转金额都会先是在过渡余额中，在餐厅等处的卡机上进行刷卡操作后，过渡余额即会转入校园卡。是否继续充值？", "否", "是")) return;

            SetIsBusy(true, "正在充值……");
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
                SetIsBusy(false);
                if (last_error != null)
                    await View.ShowMessage("充值失败", last_error);
                else
                    await View.ShowMessage("充值成功", "成功充值了" + money as string + "元。");
            }
        }
    }
}
