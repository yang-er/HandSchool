using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using HandSchool.Internals;
using Dict = System.Collections.Generic.Dictionary<string, string>;

namespace HandSchool.JLU.JsonObject
{
    internal class AlreadyKnownThings
    {
        public static string Type5Name(string type5)
        {
            switch (type5)
            {
                case "4160": return "必修课";
                case "4161": return "选修课";
                case "4162": return "限选课";
                case "4163": return "校选修课";
                case "4164": return "体育课";
                default: return "未知";
            }
        }

        static readonly Lazy<List<CollegeOverview>> collegeLoader = new Lazy<List<CollegeOverview>>(LazyCollege);
        static readonly Lazy<List<BuildingOverview>> buildingLoader = new Lazy<List<BuildingOverview>>(LazyBuilding);
        static readonly Lazy<KeyValueDict> campusLoader = new Lazy<KeyValueDict>(LazyCampus);
        static readonly Lazy<KeyValueDict> divisionLoader = new Lazy<KeyValueDict>(LazyDivision);
        static readonly Lazy<Dict> termInfoLoader = new Lazy<Dict>(LazyTermInfo);

        public static KeyValueDict Division => divisionLoader.Value;
        public static KeyValueDict Campus => campusLoader.Value;
        public static List<BuildingOverview> Buildings => buildingLoader.Value;
        public static List<CollegeOverview> Colleges => collegeLoader.Value;
        public static Dict TermInfo => termInfoLoader.Value;

        public struct BuildingOverview
        {
            public string Campus;
            public string Id;
            public string Name;

            public BuildingOverview(string name, string id, string campus)
            {
                Name = name;
                Id = id;
                Campus = campus;
            }
        }

        public struct CollegeOverview
        {
            public string Name;
            public string Campus;
            public string Division;
            public string Id;
            public string Opt;

            public CollegeOverview(string name, string campus, string division, string id, string opt = null)
            {
                Name = name;
                Campus = campus;
                Division = division;
                Id = id;
                Opt = opt;
            }
        }

        static List<CollegeOverview> LazyCollege()
        {
            return new List<CollegeOverview>
            {
                new CollegeOverview("哲学社会学院", "1401", "1420", "174", "11"),
                new CollegeOverview("文学院", "1401", "1420", "175", "12"),
                new CollegeOverview("外国语学院", "1401", "1420", "104", "13"),
                new CollegeOverview("艺术学院", "1401", "1420", "105", "14"),
                new CollegeOverview("体育学院", "1401", "1420", "106", "15"),
                new CollegeOverview("新闻与传播学院", "1401", "1420", "182", "17"),
                new CollegeOverview("经济学院", "1401", "1421", "107", "21"),
                new CollegeOverview("法学院", "1401", "1421", "108", "22"),
                new CollegeOverview("行政学院", "1401", "1421", "109", "23"),
                new CollegeOverview("商学院", "1401", "1421", "110", "24"),
                new CollegeOverview("马克思主义学院", "1405", "1421", "111", "25"),
                new CollegeOverview("金融学院", "1401", "1421", "102", "26"),
                new CollegeOverview("公共外交学院", "1401", "1421", "181", "27"),
                new CollegeOverview("数学学院", "1401", "1422", "112", "31"),
                new CollegeOverview("物理学院", "1401", "1422", "113", "32"),
                new CollegeOverview("化学学院", "1401", "1422", "114", "33"),
                new CollegeOverview("生命科学学院", "1401", "1422", "115", "34"),
                new CollegeOverview("机械科学与工程学院", "1402", "1423", "116", "41"),
                new CollegeOverview("汽车工程学院", "1402", "1423", "117", "42"),
                new CollegeOverview("材料科学与工程学院", "1402", "1423", "118", "43"),
                new CollegeOverview("交通学院", "1402", "1423", "119", "44"),
                new CollegeOverview("生物与农业工程学院", "1402", "1423", "120", "45"),
                new CollegeOverview("管理学院", "1402", "1423", "121", "46"),
                new CollegeOverview("工程训练中心", null, null, "183"),
                new CollegeOverview("电子科学与工程学院", "1401", "1424", "122", "51"),
                new CollegeOverview("通信工程学院", "1405", "1424", "123", "52"),
                new CollegeOverview("计算机科学与技术学院", "1401", "1424", "100", "53"),
                new CollegeOverview("软件学院", "1401", "1424", "101", "54"),
                new CollegeOverview("地球科学学院", "1404", "1425", "124", "61"),
                new CollegeOverview("地球探测科学与技术学院", "1404", "1425", "125", "62"),
                new CollegeOverview("建设工程学院", "1404", "1425", "126", "63"),
                new CollegeOverview("环境与资源学院", "1404", "1425", "127", "64"),
                new CollegeOverview("仪器科学与电气工程学院", "1404", "1425", "128", "65"),
                new CollegeOverview("白求恩医学院", "1403", "1426", "103", "70"),
                new CollegeOverview("基础医学院", "1403", "1426", "129", "71"),
                new CollegeOverview("公共卫生学院", "1403", "1426", "130", "72"),
                new CollegeOverview("药学院", "1403", "1426", "131", "73"),
                new CollegeOverview("护理学院", "1403", "1426", "132", "74"),
                new CollegeOverview("第一临床医学院", "1403", "1426", "133", "75"),
                new CollegeOverview("第二临床医学院", "1403", "1426", "134", "76"),
                new CollegeOverview("第三临床医学院", "1403", "1426", "135", "77"),
                new CollegeOverview("口腔医学院", "1403", "1426", "136", "78"),
                new CollegeOverview("临床医学院", "1403", "1426", "176", "79"),
                new CollegeOverview("畜牧兽医学院", "1406", "1428", "137", "81"),
                new CollegeOverview("植物科学学院", "1406", "1428", "138", "82"),
                new CollegeOverview("军需科技学院", "1406", "1428", "139", "83"),
                new CollegeOverview("农学部公共教学中心", null, "1428", "149"),
                new CollegeOverview("动物科学学院", "1406", "1428", "177", "85"),
                new CollegeOverview("动物医学学院", "1406", "1428", "178", "86"),
                new CollegeOverview("食品科学与工程学院", "1406", "1423", "187", "87"),
                new CollegeOverview("应用技术学院", "1405", null, "168", "90"),
                new CollegeOverview("公共外语教育学院", null, null, "141"),
                new CollegeOverview("公共体育教学与研究中心", null, null, "142"),
                new CollegeOverview("公共数学教学与研究中心", null, null, "143"),
                new CollegeOverview("公共物理教学与研究中心", null, null, "144"),
                new CollegeOverview("公共化学教学与研究中心", null, null, "145"),
                new CollegeOverview("公共计算机教学与研究中心", null, null, "146"),
                new CollegeOverview("机械基础教学与研究中心", null, "1423", "147"),
                new CollegeOverview("艺术教学与研究中心", null, null, "148"),
                new CollegeOverview("古生物与地学研究中心", null, null, "150"),
                new CollegeOverview("综合信息矿产预测研究所", null, null, "151"),
                new CollegeOverview("汽车动态模拟国家重点实", null, null, "152"),
                new CollegeOverview("塑性与超塑性研究所", null, null, "153"),
                new CollegeOverview("辊锻工艺研究所", null, null, "154"),
                new CollegeOverview("链传动研究所", null, null, "155"),
                new CollegeOverview("测试中心", null, null, "156"),
                new CollegeOverview("军事理论教研室", null, null, "173"),
                new CollegeOverview("理论化学研究所", null, null, "157"),
                new CollegeOverview("分子酶工程教育部重点实验室", null, null, "158"),
                new CollegeOverview("原子与分子物理研究所", null, null, "159"),
                new CollegeOverview("超分子结构与谱学教育部", null, null, "160"),
                new CollegeOverview("农学部实验动物中心", null, null, "161"),
                new CollegeOverview("古籍研究所", null, null, "162"),
                new CollegeOverview("东北亚研究院", null, null, "163"),
                new CollegeOverview("高等教育研究所", null, null, "164"),
                new CollegeOverview("经济信息学院", null, null, "165", "X1"),
                new CollegeOverview("工商管理学院", null, null, "166", "X2"),
                new CollegeOverview("软件学院(珠海)", null, null, "167", "X3"),
                new CollegeOverview("外事服务中心", null, null, "169"),
                new CollegeOverview("网络中心", null, null, "170"),
                new CollegeOverview("中心校区", "1401", null, "201"),
                new CollegeOverview("东区教务办", "1402", null, "202"),
                new CollegeOverview("北区教务办", "1404", null, "204"),
                new CollegeOverview("西区教务办", "1406", null, "206"),
                new CollegeOverview("待定", null, null, "140", "99"),
                new CollegeOverview("公选课", null, null, "-3"),
                new CollegeOverview("所有学院", null, null, "-1"),
                new CollegeOverview("未分配", null, null, "-2"),
                new CollegeOverview("管理部门", null, null, "-4"),
                new CollegeOverview("再生医学研究所", null, null, "171"),
                new CollegeOverview("实验动物中心", null, null, "172"),
                new CollegeOverview("学生心理健康指导中心", null, null, "184"),
                new CollegeOverview("人兽共患病研究所", "1401", null, "209"),
                new CollegeOverview("创新创业教育学院", null, null, "191"),
                new CollegeOverview("就业指导中心", null, null, "210"),
                new CollegeOverview("数学研究所", null, null, "180"),
                new CollegeOverview("注册与考试中心", null, null, "211"),
                new CollegeOverview("莱姆顿学院", null, null, "179"),
                new CollegeOverview("超硬材料国家重点实验室", "1401", null, "208")
            };
        }

        static List<BuildingOverview> LazyBuilding()
        {
            return new List<BuildingOverview>
            {
                new BuildingOverview("逸夫楼","96","1401"),
                new BuildingOverview("第三教学楼","68","1401"),
                new BuildingOverview("经信教学楼","117","1401"),
                new BuildingOverview("李四光楼","84","1401"),
                new BuildingOverview("外语楼","121","1401"),
                new BuildingOverview("计算机新楼","107","1401"),
                new BuildingOverview("萃文教学楼","100","1401"),
                new BuildingOverview("体育馆","109","1401"),
                new BuildingOverview("体育场","67","1401"),
                new BuildingOverview("画室","123","1401"),
                new BuildingOverview("新理化楼","120","1401"),
                new BuildingOverview("数学楼","127","1401"),
                new BuildingOverview("图书馆","87","1401"),
                new BuildingOverview("文科实验楼","103","1401"),
                new BuildingOverview("理化楼","122","1401"),
                new BuildingOverview("实验楼","102","1401"),
                new BuildingOverview("外语学院","74","1401"),
                new BuildingOverview("游泳池","85","1401"),
                new BuildingOverview("无机合成楼","98","1401"),
                new BuildingOverview("行政学院","93","1401"),
                new BuildingOverview("力学实验室","90","1401"),
                new BuildingOverview("化学楼","92","1401"),
                new BuildingOverview("公用机房","99","1401"),
                new BuildingOverview("北区白楼","112","1401"),
                new BuildingOverview("琴房","113","1401"),
                new BuildingOverview("行政楼","60","1401"),
                new BuildingOverview("逸夫楼","65","1402"),
                //new BuildingOverview("(一)","73","1402"),
               // new BuildingOverview("(二)","82","1402"),
                new BuildingOverview("体育馆","91","1402"),
                //new BuildingOverview("(五)","119","1402"),
                new BuildingOverview("基础实验楼","124","1402"),
                new BuildingOverview("汽车交通实验馆","111","1402"),
                new BuildingOverview("公用机房","70","1402"),
                new BuildingOverview("能源动力大楼","128","1402"),
                //new BuildingOverview("","2","1402"),
                new BuildingOverview("画室","110","1402"),
                new BuildingOverview("车身教室","116","1402"),
                new BuildingOverview("测试中心","86","1402"),
                new BuildingOverview("实习","69","1402"),
                //new BuildingOverview("（五）","97","1402"),
                new BuildingOverview("造型室","114","1402"),
                new BuildingOverview("第一教学楼","72","1403"),
                new BuildingOverview("第二教学楼","322","1403"),
                new BuildingOverview("新教学楼","77","1403"),
                new BuildingOverview("公用机房","71","1403"),
                new BuildingOverview("第二阶梯教室","78","1403"),
                new BuildingOverview("第一阶梯教室","81","1403"),
                new BuildingOverview("义和路","125","1403"),
                new BuildingOverview("水工楼","94","1404"),
                new BuildingOverview("鸽子楼","101","1404"),
                new BuildingOverview("实验楼","95","1404"),
                new BuildingOverview("地质宫","105","1404"),
                new BuildingOverview("第1教学楼","89","1405"),
                new BuildingOverview("第3教学楼","76","1405"),
                new BuildingOverview("体育场馆","301","1405"),
                new BuildingOverview("运动场","126","1405"),
                new BuildingOverview("公用机房","75","1405"),
                new BuildingOverview("第一教学楼","83","1405"),
                new BuildingOverview("第2教学楼","106","1405"),
                new BuildingOverview("风雨操场","108","1405"),
                new BuildingOverview("体育场","61","1406"),
                new BuildingOverview("公用机房","80","1406"),
                new BuildingOverview("科学讲堂","200","1406"),
                new BuildingOverview("兽医实验楼","201","1406"),
            };
        }

        static KeyValueDict LazyCampus()
        {
            return new KeyValueDict
            {
                { "1401", "前卫校区" },
                { "1402", "南岭校区" },
                { "1403", "新民校区" },
                { "1404", "朝阳校区" },
                { "1405", "南湖校区" },
                { "1406", "和平校区" }
            };
        }

        static KeyValueDict LazyDivision()
        {
            return new KeyValueDict
            {
                { "1420", "人文学部" },
                { "1421", "社会科学学部" },
                { "1422", "理学部" },
                { "1423", "工学部" },
                { "1424", "信息科学学部" },
                { "1425", "地球科学学部" },
                { "1426", "白求恩医学部" },
                { "1428", "农学部" }
            };
        }

        static Dict LazyTermInfo()
        {
            return new Dict
            {
                { "136", "2018-2019学年第2学期" },
                { "135", "2018-2019学年第1学期" },
                { "134", "2017-2018学年第2学期" },
                { "133", "2017-2018学年第1学期" },
                { "132", "2016-2017学年第2学期" },
                { "131", "2016-2017学年第1学期" },
                { "130", "2015-2016学年第2学期" },
                { "129", "2015-2016学年第1学期" },
            };
        }
    }
}