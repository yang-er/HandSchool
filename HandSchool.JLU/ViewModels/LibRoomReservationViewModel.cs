using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Views;
using HandSchool.Models;
using HandSchool.ViewModels;

namespace HandSchool.JLU.ViewModels
{
    public class LibRoomReservationViewModel : BaseViewModel
    {
        private static readonly Lazy<LibRoomReservationViewModel> Lazy =
            new Lazy<LibRoomReservationViewModel>(() => new LibRoomReservationViewModel());
        
        public static LibRoomReservationViewModel Instance => Lazy.Value;
        public ObservableCollection<StudentLibBasicInfo> Recommends { get; set; } = new ObservableCollection<StudentLibBasicInfo>();
        public ObservableCollection<Irregularities> IrregularitiesInfos { get; set; } = new ObservableCollection<Irregularities>();
        public ObservableCollection<ReservationInfo> ReservationRecords { get; set; } = new ObservableCollection<ReservationInfo>();
        public ObservableCollection<StudentLibBasicInfo> Selected { get; set; } = new ObservableCollection<StudentLibBasicInfo>();

        public LibRoomData UserInfo { get; set; }
        public string Name => UserInfo?.name??"你登陆呀";

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public List<List<string>> Credits => UserInfo?.credit;
        public string Dept => UserInfo?.dept??"看不清";
        public string Id => UserInfo?.id??"不清楚";
        public string Profession => UserInfo?.cls ?? "不知道";
        public LibRoomReservationViewModel()
        {
            Task.Run(async () =>
            {
                if (await Loader.LibRoom.CheckLogin())
                {
                    await RefreshInfosAsync();
                }
            });
        }

        public void InitUserInfo(LibRoomData info)
        {
            UserInfo = info;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Dept));
            OnPropertyChanged(nameof(Id));
            OnPropertyChanged(nameof(Profession));
            OnPropertyChanged(nameof(Credits));
        }
        
        /// <summary>
        /// 获取图书馆占用信息并展示
        /// </summary>
        /// <param name="obj">
        /// 一个object的列表。
        /// 其中，第一个是元素是要查询的日子，
        /// 第二个元素是查询的类型 0：3-6人间， 1：5-10人间，
        /// 第三个是导航参数，用以推出页面
        /// </param>
        public async Task<TaskResp> GetRoomAsync(GetRoomUsageParams obj)
        {
            if (IsBusyOrRefreshing) return TaskResp.False;
            
            IsBusy = true;
            
            if (!await Loader.LibRoom.CheckLogin())
            {
                IsBusy = false;
                return new TaskResp(false, "你先登录呀！(╯°Д°)╯︵ ┻━┻");
            }
            
            var now = DateTime.Now;
            var url =
                $"ClientWeb/pro/ajax/device.aspx?dev_order=&kind_order=&classkind=1&display=cld&md=d&kind_id={(obj.RoomType == 0 ? "100654216" : "100654218")}&purpose=&selectOpenAty=&cld_name=default&date={(obj.Date == NearDays.Today ? now.ToString("yyyyMMdd") : now.AddDays(1).ToString("yyyyMMdd"))}&act=get_rsv_sta";
            var res = await Loader.LibRoom.GetUsageAsync(url);
            IsBusy = false;
            if (!res.IsSuccess)
            {
                return new TaskResp(false, res.Msg is null ? "服务器返回信息有问题" : res.Msg.ToString());
            }
            
            var resPageParams = new LibRoomResultPageParams
            {
                GetRoomUsageParams = obj,
                Rooms = res.Msg as IList<LibRoom>,
                Title = (obj.RoomType == 0 ? "3-6" : "5-10") +
                        $"人研讨间{(obj.Date == NearDays.Today ? "今日" : "明日")}占用情况",
                Date = obj.Date
            };
            return new TaskResp(true, resPageParams);
        }

        public void ClearUserInfo()
        {
            if (IsBusyOrRefreshing) return;
            IsBusy = true;
            Loader.LibRoom.DeleteUserInfo();
            InitUserInfo(null);
            Recommends.Clear();
            Selected.Clear();
            IrregularitiesInfos.Clear();
            ReservationRecords.Clear();
            IsBusy = false;
        }

        public bool IsBusyOrRefreshing => IsRefreshing || IsBusy;

        public async Task<TaskResp> RefreshInfosAsync()
        {
            if (IsBusyOrRefreshing)
            {
                if (IsBusy)
                {
                    IsRefreshing = true;
                    IsRefreshing = false;
                }
                return TaskResp.False;
            }
            IsRefreshing = true;
            if(!await Loader.LibRoom.CheckLogin())
            {
                IsRefreshing = false;
                return new TaskResp(false, "你先登录呀！(╯°Д°)╯︵ ┻━┻");
            }

            var resIrregularities = await Loader.LibRoom.GetIrregularitiesAsync();
            var resReservationRecords = await Loader.LibRoom.GetResvRecordsAsync();
            IsRefreshing = false;

            if (!resIrregularities.IsSuccess || !resReservationRecords.IsSuccess)
            {
                string error;
                if (!resIrregularities.IsSuccess && !resReservationRecords.IsSuccess)
                {
                    error = resIrregularities.Msg.Equals(resReservationRecords.Msg)
                        ? resIrregularities.ToString()
                        : $"{resIrregularities}\n{resReservationRecords}";
                }
                else
                {
                    error = !resIrregularities.IsSuccess
                        ? resIrregularities.ToString()
                        : resReservationRecords.ToString();
                }

                return new TaskResp(false, error);
            }

            Core.Platform.EnsureOnMainThread(() =>
            {
                if (resIrregularities.Msg is List<Irregularities> list)
                {
                    IrregularitiesInfos.Clear();
                    foreach (var item in list)
                    {
                        IrregularitiesInfos.Add(item);
                    }
                }

                if (resReservationRecords.Msg is List<ReservationInfo> list2)
                {

                    ReservationRecords.Clear();
                    foreach (var item in list2)
                    {
                        ReservationRecords.Add(item);
                    }
                }
            });
            return TaskResp.True;
        }

        public async Task<TaskResp> StartResvAsync(LibRoom libRoom, NearDays date,DateTime start, DateTime end)
        {
            if (IsBusyOrRefreshing) return TaskResp.False;
            IsBusy = true;
            if (Selected.Count < libRoom.MinUser || Selected.Count > libRoom.MaxUser)
            {
                IsBusy = false;
                return new TaskResp(false, $"人数必须在{libRoom.MinUser}~{libRoom.MaxUser}之间");
            }
            
            if (Selected.All(info => info.SchoolCardId.Trim() != Loader.LibRoom.Username.Trim()))
            {
                if (!await RequestAnswerAsync("提示", "人员列表中不包含预约人，如果继续预约，则该条预约无法取消，是否继续？", "否", "是"))
                {
                    IsBusy = false;
                    return TaskResp.False;
                }
            }
            
            var sb = new StringBuilder("$");
            for (var i = 0; i < Selected.Count; i++)
            {
                if (Selected[i].InnerId is null)
                {
                    IsBusy = false;
                    return new TaskResp(false, "读取人员信息失败");
                }
                if (i != Selected.Count - 1)
                {
                    sb.Append(Selected[i].InnerId).Append(",");
                }
                else
                {
                    sb.Append(Selected[i].InnerId);
                }
            }
            
            if (date == NearDays.Tomorrow)
            {
                start = start.AddDays(1);
                end = end.AddDays(1);
            }

            try
            {
                var res = await Loader.LibRoom.SendResvAsync(libRoom, sb.ToString(), start, end);
                return !res.IsSuccess ? res : TaskResp.True;
            }
            catch (WebsException we)
            {
                return new TaskResp(false, we.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<TaskResp> CancelResvAsync(string resvId)
        {
            if (IsBusyOrRefreshing) return TaskResp.False;
            IsBusy = true;
            try
            {
                var res = await Loader.LibRoom.CancelResvAsync(resvId);
                if (!res.IsSuccess)
                {
                    return res.Msg is null ? new TaskResp(false, "服务器返回信息异常") : res;
                }
                else
                {
                    return TaskResp.True;
                }
            }
            catch (Exception e)
            {
                await NoticeError(e.Message);
                return new TaskResp(false, e.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}