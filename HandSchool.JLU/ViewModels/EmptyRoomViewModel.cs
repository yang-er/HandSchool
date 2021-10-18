using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.ViewModels;

namespace HandSchool.JLU.JsonObject
{
    #region 校区信息

    public class SchoolAreaValueItem
    {

        public string NValue { get; set; }
        public string SValue { get; set; }
        public string isLocked { get; set; }
        public string name { get; set; }
        public int dictId { get; set; }
        public string category { get; set; }
        public string parentId { get; set; }
    }

    public class SchoolAreaJson
    {
        public string resName { get; set; }
        public string id { get; set; }
        public List<SchoolAreaValueItem> value { get; set; }
        public int status { get; set; }
    }

    #endregion

    #region 教学楼信息
    public class Campus
    {
        public string name { get; set; }
    }
    public class BuildingValueItem
    {
        public string area { get; set; }
        public int? compus { get; set; }
        public string dateBuild { get; set; }
        public string address { get; set; }
        public int? activeStatus { get; set; }
        public int? extOrderNo { get; set; }
        public Campus campus { get; set; }
        public string name { get; set; }
        public string abbr { get; set; }
        public string storey { get; set; }
        public int? buildingId { get; set; }
    }

    public class BuildingJson
    {
        public string resName { get; set; }
        public string id { get; set; }
        public List<BuildingValueItem> value { get; set; }
        public int? status { get; set; }
    }
    #endregion

    #region 空教室信息
    public class EmptyRoomsBuilding
    {
        public string name { get; set; }
    }

    public class EmptyRoomsValue
    {
        public string notes { get; set; }
        public string roomNo { get; set; }
        public string usage { get; set; }
        public string fullName { get; set; }
        public int? clsrmType { get; set; }
        public string seatColGroup { get; set; }
        public int? roomId { get; set; }
        public EmptyRoomsBuilding building { get; set; }
        public int? examVolume { get; set; }
        public int? volume { get; set; }
        public string seatRows { get; set; }
        public string allowConflict { get; set; }
        public int? floor { get; set; }
    }

    public class EmptyRooms
    {
        public string resName { get; set; }
        public string id { get; set; }
        public List<EmptyRoomsValue> value { get; set; }
        public int? status { get; set; }
    }
    #endregion
}


namespace HandSchool.JLU.ViewModels
{
    public class RoomInfo
    {
        public string Name { get; set; }
        public string Description => $"用途：{Usage}\n容量：{Vol}{(Notice is null ? string.Empty : $"\n备注：{Notice}")}";
        public string Usage { get; set; }
        public int? Vol { get; set; }
        public string Notice { get; set; }
    }
    public class EmptyRoomViewModel : BaseViewModel
    {
        private static readonly Lazy<EmptyRoomViewModel> Lazy =
            new Lazy<EmptyRoomViewModel>(() => new EmptyRoomViewModel());
        public static EmptyRoomViewModel Instance => Lazy.Value;
        public ObservableCollection<string> SchoolAreas { get; set; }
        public ObservableCollection<string> Building { get; set; }
        public ObservableCollection<RoomInfo> Rooms { get; set; }
        private SchoolAreaJson _schoolAreaJson;
        private BuildingJson _buildingJson;
        private const string ServerUrl = "service/res.do";
        public EmptyRoomViewModel()
        {
            SchoolAreas = new ObservableCollection<string>();
            Building = new ObservableCollection<string>();
            Rooms = new ObservableCollection<RoomInfo>();
        }

        public void Clear()
        {
            SchoolAreas?.Clear();
            Building?.Clear();
            Rooms?.Clear();
        }
        public async Task<bool> GetSchoolAreaAsync()
        {
            if (IsBusy) return false;
            IsBusy = true;
            await Task.Yield();
            try
            {
                var postValue =
                    "{\n\"tag\": \"sysDict\",\n\"branch\": \"byCategory\",\n\"params\": {\n\"cg\": \"CAMPUS\"\n}\n}";
                var str = await Core.App.Service.Post(ServerUrl, postValue);
                _schoolAreaJson = Newtonsoft.Json.JsonConvert.DeserializeObject<SchoolAreaJson>(str);
                Core.Platform.EnsureOnMainThread(() =>
                {
                    SchoolAreas.Clear();
                    foreach (var item in _schoolAreaJson.value)
                    {
                        SchoolAreas.Add(item.name);
                    }
                });
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> GetBuildingAsync(string area)
        {
            if (IsBusy) return false;
            IsBusy = true;
            await Task.Yield();
            try
            {
                var schoolArea = _schoolAreaJson.value.Find(x => x.name == area);
                var postVal =
                    "{\"tag\":\"building@input\",\"branch\":\"default\",\"params\":{\"campus\":\"" +
                    schoolArea.dictId +
                    "\",\"name\":\"\"}}";
                var resp = await Core.App.Service.Post(ServerUrl, postVal);
                _buildingJson = Newtonsoft.Json.JsonConvert.DeserializeObject<BuildingJson>(resp);
                Core.Platform.EnsureOnMainThread(() =>
                {
                    Building.Clear();
                    foreach (var item in _buildingJson.value)
                    {
                        Building.Add(item.name);
                    }
                });
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> GetEmptyRoomAsync(string schoolArea, string building, int start, int end)
        {
            if (IsBusy) return false;
            IsBusy = true;
            await Task.Yield();
            try
            {
                var dateStr = DateTime.Now.ToString("yyyy-MM-dd");
                var bdInfo = _buildingJson.value.Find(x => x.name == building);
                var cs = 0;
                for (var i = start; i <= end; i++)
                {
                    cs += (int) Math.Pow(i, 2);
                }

                var postValue =
                    $"{{\"tag\":\"roomIdle@roomUsage\",\"branch\":\"default\",\"params\":{{\"termId\":`term`,\"bid\":\"{bdInfo.buildingId}\",\"rname\":\"\",\"dateActual\":{{}},\"cs\":{cs},\"d_actual\":\"{dateStr}T00:00:00+08:00\"}}}}";
                var empty = await Core.App.Service.Post(ServerUrl, postValue);
                var rooms = Newtonsoft.Json.JsonConvert.DeserializeObject<EmptyRooms>(empty);
                if (rooms is null) return false;
                Core.Platform.EnsureOnMainThread(() =>
                {
                    Rooms.Clear();
                    foreach (var item in rooms.value)
                    {
                        Rooms.Add(new RoomInfo
                        {
                            Name = item.roomNo,
                            Notice = item.notes,
                            Usage = AlreadyKnownThings.GetClassRoomUsage(item.usage),
                            Vol = item.volume
                        });
                    }
                });
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
