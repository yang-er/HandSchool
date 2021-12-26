using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HandSchool.JLU.Views;
using HandSchool.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HandSchool.JLU.JsonObject
{
    public class LibRoomData
    {
        public string id { get; set; }
        public string accno { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string msn { get; set; }
        public string ident { get; set; }
        public string dept { get; set; }
        public string deptid { get; set; }
        public string tutor { get; set; }
        public string tutorid { get; set; }
        public string cls { get; set; }
        public string clsid { get; set; }
        public bool receive { get; set; }
        public string tsta { get; set; }
        public string rtsta { get; set; }
        public string pro { get; set; }
        public int score { get; set; }
        public List<List<string>> credit { get; set; }
        public string role { get; set; }
    }

    public class LibRoomJson
    {
        public int ret { get; set; }
        public string act { get; set; }
        public string msg { get; set; }
        public LibRoomData data { get; set; }
        public string ext { get; set; }
    }
}

namespace HandSchool.JLU.Services
{
    [Entrance("JLU", "图书馆座位预约", "鼎新馆研讨间预约")]
    [UseStorage("JLU", ConfigUsername, ConfigPassword, ConfigAutoLogin)]
    public class LibRoomReservation : NotifyPropertyChanged, ILoginField
    {
        const string ConfigUsername = "jlu.libroom.username.txt";
        const string ConfigPassword = "jlu.libroom.password.txt";
        const string ConfigAutoLogin = "jlu.libroom.autologin.txt";

        string baseUrl = "http://libzwyy.jlu.edu.cn/";
        bool _isLogin = false;
        bool _autoLogin = false;
        bool _savePassword = false;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Tips => "登录名/密码均与校园卡相同";
        public bool NeedLogin => !_isLogin;

        public bool IsLogin
        {
            get => _isLogin;
            private set => SetProperty(ref _isLogin, value);
        }

        public bool AutoLogin
        {
            get => _autoLogin;
            set
            {
                SetProperty(ref _autoLogin, value);
                if (value) SetProperty(ref _savePassword, true, nameof(SavePassword));
            }
        }

        public bool SavePassword
        {
            get => _savePassword;
            set
            {
                SetProperty(ref _savePassword, value);
                if (!value) SetProperty(ref _autoLogin, false, nameof(AutoLogin));
            }
        }

        public string FormName => "图书馆IC空间管理系统";
        public string CaptchaCode { get; set; }
        public byte[] CaptchaSource { get; }

        public void DeleteUserInfo()
        {
            Core.Configure.Remove(ConfigPassword);
            Core.Configure.Remove(ConfigUsername);
            Core.Configure.Remove(ConfigAutoLogin);
            SavePassword = AutoLogin = IsLogin = false;
            Username = Password = "";
        }

        public LibRoomReservation()
        {
            WebClient = Core.New<IWebClient>();
            WebClient.Timeout = 5000;
            WebClient.BaseAddress = baseUrl;

            IsLogin = false;
            Username = Core.Configure.Read(ConfigUsername);
            if (!string.IsNullOrWhiteSpace(Username)) 
                Password = Core.Configure.Read(ConfigPassword);
            SavePassword = !string.IsNullOrEmpty(Password);
            AutoLogin = bool.TryParse(Core.Configure.Read(ConfigAutoLogin), out var al) && al;
            TimeoutManager = new TimeoutManager(900);
        }

        public event EventHandler<LoginStateEventArgs> LoginStateChanged;

        public async Task<TaskResp> Login()
        {
            if (Username == "" || Password == "")
            {
                return TaskResp.False;
            }

            Core.Configure.Write(ConfigUsername, Username);
            Core.Configure.Write(ConfigPassword, SavePassword ? Password : "");
            Core.Configure.Write(ConfigAutoLogin, AutoLogin.ToString());
            if (Username.Trim().Length != 11)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "用户名不对劲！"));
                return TaskResp.False;
            }

            var postValue = new KeyValueDict
            {
                {"id", Username.Trim()},
                {"pwd", Password},
                {"act", "login"}
            };

            try
            {
                var reqMeta = new WebRequestMeta("ClientWeb/pro/ajax/login.aspx", WebRequestMeta.All);
                reqMeta.SetHeader("Referer", baseUrl + "ClientWeb/xcus/ic2/Default.aspx");
                var resp = await WebClient.PostAsync(reqMeta, postValue);
                var respStr = await resp.ReadAsStringAsync();
                var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<LibRoomJson>(respStr);
                if (msg is null)
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, "服务器返回格式错误！"));
                    return TaskResp.False;
                }

                if (msg.ret == 0)
                {
                    LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, msg.msg));
                    return TaskResp.False;
                }

                LibRoomReservationViewModel.Instance.InitUserInfo(msg.data);
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Succeeded));
                IsLogin = true;
                return TaskResp.True;
            }
            catch (Exception e)
            {
                LoginStateChanged?.Invoke(this, new LoginStateEventArgs(LoginState.Failed, e.Message));
                return TaskResp.False;
            }
        }

        public async Task LoginOut()
        {
            try
            {
                await WebClient.GetStringAsync("ClientWeb/pro/ajax/login.aspx?act=logout");
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<TaskResp> PrepareLogin()
        {
            try
            {
                await WebClient.GetAsync(baseUrl);
                return TaskResp.True;
            }
            catch
            {
                return TaskResp.False;
            }
        }

        public Task<TaskResp> BeforeLoginForm() => Task.FromResult(TaskResp.True);

        public async Task<bool> CheckLogin()
        {
            if (!IsLogin || TimeoutManager.IsTimeout())
            {
                if (IsLogin)
                {
                    await LoginOut();
                    IsLogin = false;
                }
            }
            else return true;

            if (await this.RequestLogin() == RequestLoginState.SUCCESSED)
            {
                TimeoutManager.Refresh();
                return true;
            }
            else return false;
        }

        public IWebClient WebClient { get; set; }
        public TimeoutManager TimeoutManager { get; set; }

        public async Task<TaskResp> GetIrregularitiesAsync()
        {
            try
            {
                var response = await WebClient.GetStringAsync("ClientWeb/xcus/a/center.aspx");
                var html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(response);
                var trs = html.DocumentNode.SelectNodes("//table[@class='tab_con']//tbody//tr")?.ToList();
                var list = new List<Irregularities>();
                if (trs is null || trs.Count == 0) list.Clear();
                else
                {
                    foreach (var tr in trs)
                    {
                        var tds = tr.SelectNodes(".//td").ToList();
                        try
                        {
                            var record = new Irregularities
                            {
                                DateTime = DateTime.Parse(tds[0].InnerText),
                                Place = tds[1].InnerText,
                                Tips = tds[2].InnerText,
                                State = tds[3].InnerText,
                                Score = int.Parse(tds[4].InnerText)
                            };
                            list.Add(record);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                return new TaskResp(true, list);
            }
            catch (Exception e)
            {
                return new TaskResp(false, e.Message);
            }
        }

        public async Task<TaskResp> GetResvRecordsAsync()
        {
            try
            {
                var response =
                    await WebClient.GetStringAsync(
                        "ClientWeb/pro/ajax/center.aspx?act=get_History_resv&strat=90&StatFlag=New");
                var msgJson = JsonConvert.DeserializeObject<JObject>(response);
                var msg = msgJson["msg"].ToString();
                var html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(msg);

                var tBodies = html.DocumentNode.SelectNodes("//tbody")?.ToList();
                var list = new List<ReservationInfo>();
                if (tBodies is null || tBodies.Count == 0) list.Clear();
                else
                {
                    foreach (var tbody in tBodies)
                    {
                        try
                        {
                            var type = tbody.SelectSingleNode(".//h3");
                            var resvDate = tbody.SelectSingleNode(".//span[@class='pull-right']/span[@class='grey']");
                            var times = tbody.SelectNodes(".//span[@class='text-primary']").ToList();
                            var states = tbody.SelectNodes(".//span[@class='uni_trans']").ToList();
                            var people = tbody.SelectSingleNode(".//span[@class='grey' and @title]");
                            var roomId = tbody.SelectSingleNode(".//div/a");
                            var owner = tbody.SelectSingleNode(".//tr[@class='content']/td[2]");
                            var floor = tbody.SelectSingleNode(".//div[@class='box']/div");
                            var ops = tbody.SelectSingleNode(".//td[@class='text-center']/a");

                            var ssb = new StringBuilder();
                            foreach (var state in states)
                            {
                                ssb.Append(state.InnerText).Append('\n');
                            }

                            var item = new ReservationInfo
                            {
                                IsUsing = false,
                                Kind = floor?.InnerText,
                                Start = times[0].InnerText,
                                End = times[1].InnerText,
                                People = people.InnerText.Replace(',', '\n'),
                                RoomId = roomId.InnerText,
                                ResvTime = DateTime.Parse(resvDate.InnerText),
                                State = ssb.ToString(),
                                Owner = owner.InnerText,
                                ResvType = type.InnerText,
                                ResvInnerId = ops?.GetAttributeValue("rsvid", null)
                            };
                            if(item.ResvInnerId is null)
                            {
                                var usingRoomId = ops?.GetAttributeValue("onclick", null);
                                if (!string.IsNullOrWhiteSpace(usingRoomId))
                                {
                                    var match = Regex.Match(usingRoomId, "\\(.*\\)");
                                    if (match.Length > 0)
                                    {
                                        var realId = match.Value.Replace("(", "").Replace(")", "");
                                        if (long.TryParse(realId, out var numId))
                                        {
                                            item.ResvInnerId = numId.ToString();
                                            item.IsUsing = true;
                                        }
                                    }
                                }
                            }
                            list.Add(item);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                return new TaskResp(true, list);
            }
            catch (Exception e)
            {
                return new TaskResp(false, e.Message);
            }
        }

        public async Task<TaskResp> GetUsageAsync(string url)
        {
            try
            {
                var res = new List<LibRoom>();
                var dt = DateTime.Now;
                var str = await WebClient.GetStringAsync(url);
                var jo = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(str);
                if (jo is null)
                {
                    return new TaskResp(false, "服务器返回数据错误");
                }
                var data = jo["data"];
                if (data is null)
                {
                    return new TaskResp(false, "服务器返回数据错误");
                }

                foreach (var i in data)
                {
                    if(i is null) continue;
                    
                    //从data提取出LibRoom的信息
                    var room = new LibRoom
                    {
                        Name = i["name"].ToString(),
                        Floor = i["labName"].ToString(),
                        MinMins = i["min"].ToObject<int?>(),
                        MaxMins = i["max"].ToObject<int?>(),
                        MinUser = i["minUser"].ToObject<int?>(),
                        MaxUser = i["maxUser"].ToObject<int?>(),
                        LabId = i["labId"].ToString(),
                        RoomId = i["roomId"].ToString(),
                        DevId = i["devId"].ToString(),
                        KindId = i["kindId"].ToString(),
                        OpenTimes = i["open"].ToObject<List<string>>(),
                        Ts = i["ts"].ToObject<List<RoomUsageInfo>>(),
                        Cls = i["cls"].ToObject<List<RoomUsageInfo>>(),
                        Ops = i["ops"].ToObject<List<RoomUsageInfo>>(),
                        State = i["state"].ToString().Contains("close")
                            ? LibRoom.LibRoomState.Close
                            : LibRoom.LibRoomState.NotClose
                    };

                    //处理房间里面的预约
                    if (room.Ts.Count != 0)
                    {
                        room.Times = new List<TimeSlot>();
                        foreach
                        (var ts in
                            from item in room.Ts
                            where !(item.start is null) && !(item.end is null)
                            select new TimeSlot
                            {
                                Start = new Time(item.start.Value),
                                End = new Time(item.end.Value),
                                Msg = item.title
                            }
                        )
                        {
                            room.Times.Add(ts);
                        }
                        room.Times.Sort((a, b) => a.Start.CompareTo(b.Start));
                    }

                    res.Add(room);
                }

                return new TaskResp(true, res);
            }
            catch (HttpRequestException)
            {
                return new TaskResp(false, "网络连接异常");
            }
            catch (Exception e)
            {
                return new TaskResp(false, e.Message);
            }
        }

        public async Task<TaskResp> GetUserInfoAsync(string schoolCardId)
        {
            try
            {
                var msg = await WebClient.GetStringAsync(
                    $"ClientWeb/pro/ajax/data/searchAccount.aspx?type=&ReservaApply=ReservaApply&term={schoolCardId}");
                var data = JsonConvert.DeserializeObject<List<JObject>>(msg);
                if (data is null) return TaskResp.False;
                var res = data.Select(item => new StudentLibBasicInfo
                {
                    SchoolCardId = schoolCardId,
                    Name = item["name"]?.ToString(),
                    Tips = $"{item["name"]}({schoolCardId})",
                    InnerId = item["id"]?.ToString()
                }).ToList();

                return new TaskResp(true, res);
            }
            catch
            {
                return TaskResp.False;
            }
        }

        public async Task<TaskResp> CancelResvAsync(string resvId)
        {
            var str = await Loader.LibRoom.WebClient.GetStringAsync(
                $"ClientWeb/pro/ajax/reserve.aspx?act=del_resv&id={resvId}");
            var jo = JsonConvert.DeserializeObject<JObject>(str);
            if (jo == null)
            {
                return new TaskResp(false, "操作失败！请前往网站尝试");
            }

            if (jo["ret"]?.ToString() != "1")
            {
                return new TaskResp(false, jo["msg"]?.ToString());
            }

            return new TaskResp(true, jo["msg"]);
        }

        public async Task<TaskResp> EndResvAsync(string resvId)
        {
            var getUrl = $"ClientWeb/pro/ajax/reserve.aspx?act=resv_leave&type=2&resv_id={resvId}";
            var str = await Loader.LibRoom.WebClient.GetStringAsync(getUrl);
            var jo = JsonConvert.DeserializeObject<JObject>(str);
            if (jo == null)
            {
                return new TaskResp(false, "操作失败！请前往网站尝试");
            }

            if (jo["ret"]?.ToString() != "1")
            {
                return new TaskResp(false, jo["msg"]?.ToString());
            }

            return new TaskResp(true, jo["msg"]);
        }

        public async Task<TaskResp> SendResvAsync(LibRoom libRoom, string mbList, DateTime start, DateTime end)
        {

            var getParam =
                $"dialogid=&dev_id={libRoom.DevId}&lab_id={libRoom.LabId}&kind_id={libRoom.KindId}&room_id=&type=dev&" +
                $"prop=&test_id=&term=&number=&classkind=&resv_kind=1&test_name=0&min_user={libRoom.MinUser}&max_user={libRoom.MaxUser}&" +
                $"mb_list={mbList}&start={start:yyyy-MM-dd HH:mm}&end={end:yyyy-MM-dd HH:mm}&start_time={start:HHmm}&end_time={end:HHmm}&up_file=&memo=&act=set_resv";
            var str = await Loader.LibRoom.WebClient.GetStringAsync($"ClientWeb/pro/ajax/reserve.aspx?{getParam}");
            var jo = JsonConvert.DeserializeObject<JObject>(str);
            if (jo is null)
            {
                return new TaskResp(false, "服务器返回信息有问题");
            }

            var ret = jo["ret"]?.ToString();
            if (ret != "1")
            {
                return new TaskResp(false, jo["msg"]?.ToString());
            }

            return TaskResp.True;
        }

    }
}