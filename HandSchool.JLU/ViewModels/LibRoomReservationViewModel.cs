using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Views;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms.Internals;

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
        public string Name => UserInfo?.name??"你登录呀";

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
        public bool IsBusyOrRefreshing => IsRefreshing || IsBusy;
        public ICommand RefreshUserInfoCommand { get; }
        public LibRoomReservationViewModel()
        {
            RefreshUserInfoCommand = new CommandAction(RefreshInfosAsync);
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
        /// 第一个是元素是要查询的日子，
        /// 第二个元素是查询的类型 0：3-6人间， 1：5-10人间，
        /// 第三个是导航参数，用以推出页面
        /// </param>
        public async Task<TaskResp> GetRoomAsync(GetRoomUsageParams obj)
        {
            if (IsBusyOrRefreshing) return TaskResp.False;
            
            IsBusy = true;
            
            if (!await Loader.LibRoom.CheckLogin())
            {
                await NoticeError("你先登录呀！(╯°Д°)╯︵ ┻━┻");
                return TaskResp.False;
            }
            
            var now = DateTime.Now;
            var url =
                $"ClientWeb/pro/ajax/device.aspx?dev_order=&kind_order=&classkind=1&display=cld&md=d&kind_id={(obj.RoomType == 0 ? "100654216" : "100654218")}&purpose=&selectOpenAty=&cld_name=default&date={(obj.Date == NearDays.Today ? now.ToString("yyyyMMdd") : now.AddDays(1).ToString("yyyyMMdd"))}&act=get_rsv_sta";
            var res = await Loader.LibRoom.GetUsageAsync(url);
            IsBusy = false;
            if (!res.IsSuccess)
            {
                await NoticeError(res.Msg is null ? "服务器返回信息有问题" : res.Msg.ToString());
                return TaskResp.False;
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
        public async Task GetStuInfoByCardId(string schoolCardId)
        {
            if (IsBusyOrRefreshing) return;
            IsBusy = true;
            var res = await Loader.LibRoom.GetUserInfoAsync(schoolCardId);
            if (!res.IsSuccess) await NoticeError("查询用户信息失败");
            Core.Platform.EnsureOnMainThread(() =>
            {
                Recommends.Clear();
                if (res.Msg is IEnumerable<StudentLibBasicInfo> list)
                {
                    list.ForEach(info => Recommends.Add(info));
                }
            });
            IsBusy = false;
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
                await NoticeError("你先登录呀！(╯°Д°)╯︵ ┻━┻");
                return TaskResp.False;
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

                await NoticeError(error);
                return TaskResp.False;
            }

            Core.Platform.EnsureOnMainThread(() =>
            {
                if (resIrregularities.Msg is List<Irregularities> list)
                {
                    IrregularitiesInfos.Clear();
                    list.ForEach(i => IrregularitiesInfos.Add(i));
                }

                if (resReservationRecords.Msg is List<ReservationInfo> list2)
                {
                    ReservationRecords.Clear();
                    list2.ForEach(i => ReservationRecords.Add(i));
                }
            });
            return TaskResp.True;
        }
        public async Task<TaskResp> SendResvAsync(LibRoom libRoom, NearDays date, DateTime start, DateTime end)
        {
            if (IsBusyOrRefreshing) return TaskResp.False;
            IsBusy = true;
            if (Selected.Count < libRoom.MinUser || Selected.Count > libRoom.MaxUser)
            {
                await NoticeError($"人数必须在{libRoom.MinUser}~{libRoom.MaxUser}之间");
                return TaskResp.False;
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
                    await NoticeError("读取人员信息失败");
                    return TaskResp.False;
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
                if (res.IsSuccess)
                {
                    await RequestMessageAsync("提示", "预约成功", "彳亍");
                    Core.Platform.EnsureOnMainThread(Selected.Clear);
                }
                else
                {
                    if (res.Msg != null)
                    {
                        await NoticeError(res.ToString());
                    }
                }
            }
            catch (WebsException we)
            {
                await NoticeError(we.Message);
            }
            finally
            {
                IsBusy = false;
            }
            return TaskResp.True;
        }
        public async Task DelUser(StudentLibBasicInfo info)
        {
            if (info is null) return;
            if (IsBusyOrRefreshing) return;
            IsBusy = true;
            var res = await RequestAnswerAsync("取消选定", info.Tips, "否", "是");
            IsBusy = false;
            if (!res) return;
            Core.Platform.EnsureOnMainThread(() => Selected.Remove(info));
        }
        public async Task<TaskResp> AddUser(StudentLibBasicInfo info, int maxUser)
        {
            if (string.IsNullOrWhiteSpace(info?.Name) || Selected.Contains(info))
            {
                Core.Platform.EnsureOnMainThread(() =>
                {
                    Recommends.Clear();
                });
                return TaskResp.True;
            }
            
            if (Selected.Count >= maxUser)
            {
                await NoticeError("人数超出房间限制！\n长按已选择的人员以移除列表。");
                return TaskResp.False;
            }

            Core.Platform.EnsureOnMainThread(() =>
            {
                Recommends.Clear();
                Selected.Add(info);
            });
            return TaskResp.True;
        }
        public async Task CancelOrEndResvAsync(ReservationInfo resvInfo)
        {
            if(resvInfo?.ResvInnerId is null) return;
            if (IsBusyOrRefreshing) return;
            IsBusy = true;
            var msg = resvInfo.IsUsing ? "提前结束？" : "取消此次预约？";
            var resvId = resvInfo.ResvInnerId;
            if (!await RequestAnswerAsync("确认", msg, "否", "是"))
            {
                IsBusy = false;
                return;
            }

            try
            {
                var res = resvInfo.IsUsing switch
                {
                    true => await Loader.LibRoom.EndResvAsync(resvId),
                    false => await Loader.LibRoom.CancelResvAsync(resvId)
                };

                if (!res.IsSuccess)
                {
                    if (res.Msg is null)
                    {
                        await NoticeError("服务器返回信息异常");
                    }
                    else
                    {
                        await NoticeError(res.ToString());
                    }

                    return;
                }
                else
                {
                    await RefreshInfosAsync();
                    await RequestMessageAsync("提示", res.ToString(), "彳亍");
                }
            }
            catch (Exception e)
            {
                await NoticeError("出错了" + e.Message);
                return;
            }
            finally
            {
                IsBusy = false;
            }    
        }
    }
}