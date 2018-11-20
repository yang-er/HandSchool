using System;
using System.Collections.Generic;
using HandSchool.Models;
using HandSchool.Services;

namespace HandSchool.Blank
{
    class Loader : ISchoolWrapper
    {
        public string SchoolName => "任意大学";
        public string SchoolId => "blank";
        public static Loader Instance;
        public Lazy<ISchoolSystem> Service { get; set; }
        public Lazy<IGradeEntrance> GradePoint { get; set; }
        public Lazy<IScheduleEntrance> Schedule { get; set; }
        public Lazy<IMessageEntrance> Message { get; set; }
        public Lazy<IFeedEntrance> Feed { get; set; }
        public EventHandler<LoginStateEventArgs> NoticeChange { get; set; }
        public Dictionary<string, string> WeatherLocations => lazyWeatherLoc.Value;
        private Lazy<Dictionary<string, string>> lazyWeatherLoc;

        public void PostLoad() { }

        public void PreLoad()
        {
            Instance = this;
            lazyWeatherLoc = new Lazy<Dictionary<string, string>>(CreateWeatherDetail);
            var sch = new BlankSchool();
            Service = new Lazy<ISchoolSystem>(() => sch);
            Schedule = new Lazy<IScheduleEntrance>(() => new Schedule());
            GradePoint = new Lazy<IGradeEntrance>(() => null);
            Message = new Lazy<IMessageEntrance>(() => null);

            Feed = new Lazy<IFeedEntrance>(() =>
            {
                if (sch.FeedUrl != "") return new FeedEntrance(sch.FeedUrl);
                else return null;
            });
        }

        public override string ToString()
        {
            return SchoolName;
        }

        private Dictionary<string, string> CreateWeatherDetail()
        {
            var dict = new Dictionary<string, string>
            {
                { "未知", "0" },
                { "北京", "101010100" },
                { "上海", "101020100" },
                { "天津", "101030100" },
                { "重庆", "101040100" },
                { "香港", "101320101" },
                { "兰州", "101160101" },
                { "福州", "101230101" },
                { "厦门", "101230201" },
                { "广州", "101280101" },
                { "深圳", "101280601" },
                { "珠海", "101280701" },
                { "南宁", "101300101" },
                { "桂林", "101300501" },
                { "海口", "101310101" },
                { "石家庄", "101090101" },
                { "郑州", "101180101" },
                { "哈尔滨", "101050101" },
                { "武汉", "101200101" },
                { "长沙", "101250101" },
                { "长春", "101060101" },
                { "南京", "101190101" },
                { "无锡", "101190201" },
                { "苏州", "101190401" },
                { "南昌", "101240101" },
                { "沈阳", "101070101" },
                { "大连", "101070201" },
                { "呼和浩特", "101080101" },
                { "西宁", "101150101" },
                { "济南", "101120101" },
                { "青岛", "101120201" },
                { "威海", "101121301" },
                { "太原", "101100101" },
                { "西安", "101110101" },
                { "成都", "101270101" },
                { "拉萨", "101140101" },
                { "昆明", "101290101" },
                { "杭州", "101210101" },
                { "宁波", "101210401" },
                { "合肥", "101220101" }
            };

            return dict;
        }
    }
}
