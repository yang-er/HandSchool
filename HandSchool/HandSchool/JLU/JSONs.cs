using System;
using System.Collections.Generic;
using System.Collections.Specialized;

#pragma warning disable IDE1006

namespace HandSchool.JLU.JsonObject
{
    class RootObject<T>
    {
        public string id { get; set; }
        public int status { get; set; }
        public T[] value { get; set; }
        public string resName { get; set; }
        public string msg { get; set; }
        public string data { get; set; }
    }

    class ErrorMsg
    {
        public int status { get; set; }
        public string msg { get; set; }
    }

    class GPAValue
    {
        public float avgScoreBest { get; set; }
        public float avgScoreFirst { get; set; }
        public float gpaFirst { get; set; }
        public float gpaBest { get; set; }
    }

    class TeachClassMaster
    {
        public string maxStudCnt { get; set; }
        public LessonSchedule[] lessonSchedules { get; set; }
        public string studCnt { get; set; }
        public LessonTeacher[] lessonTeachers { get; set; }
        public string name { get; set; }
        public string tcmId { get; set; }
        public LessonSegment lessonSegment { get; set; }
    }

    class LessonIdList
    {
        public object arrangeInfo { get; set; }
        public LessonSchedule[] lessonSchedules { get; set; }
        public object teachingTerm { get; set; }
        public object department { get; set; }
        public string activeStatus { get; set; }
        public string tcmId { get; set; }
        public DateTime dateCreate { get; set; }
        public string egrade { get; set; }
        public LessonSegment lessonSegment { get; set; }
        public string maxStudCnt { get; set; }
        public object selectPlan { get; set; }
        public string scorePoint { get; set; }
        public object school { get; set; }
        public string studCnt { get; set; }
        public object teachClassStuds { get; set; }
        public LessonTeacher[] lessonTeachers { get; set; }
        public string name { get; set; }
        public string fixedStudCnt { get; set; }
        public string tcmType { get; set; }
        public string scoreFormat { get; set; }
        public object notes { get; set; }
        public string campus { get; set; }
    }

    class ScheduleValue
    {
        public TeachClassMaster teachClassMaster { get; set; }
        public string tcsId { get; set; }
        public object student { get; set; }
        public DateTime dateAccept { get; set; }
    }

    class ArchiveScoreValue
    {
        public string xkkh { get; set; }
        public TeachingTerm teachingTerm { get; set; }
        public string score { get; set; }
        public DateTime dateScore { get; set; }
        public object planDetail { get; set; }
        public string isPass { get; set; }
        public string classHour { get; set; }
        public Course course { get; set; }
        public string isReselect { get; set; }
        public string scoreNum { get; set; }
        public Student student { get; set; }
        public string asId { get; set; }
        public string type5 { get; set; }
        public string gpoint { get; set; }
        public string credit { get; set; }
        public object notes { get; set; }
        public string selectType { get; set; }
    }

    class OutsideScoreValue
    {
        public string isReselect { get; set; }
        public string xkkh { get; set; }
        public string studName { get; set; }
        public string xh { get; set; }
        public string lsrId { get; set; }
        public string cj { get; set; }
        public string gpoint { get; set; }
        public string zscj { get; set; }
        public string credit { get; set; }
        public string kcmc { get; set; }
        public string termId { get; set; }
    }

    class AdminClassSchedule : ScheduleValue
    {
        public string school { get; set; }
        public string tcmAdcId { get; set; }
        public string adviceType { get; set; }
        public string department { get; set; }
        public AdminClass adminClass { get; set; }
        public string gender { get; set; }
        public string mtype { get; set; }
        public string egrade { get; set; }
    }

    class LessonSegment
    {
        public string classHour { get; set; }
        public string lssgId { get; set; }
        public Lesson lesson { get; set; }
        public string weekHour { get; set; }
        public string segType { get; set; }
        public string lsSeq { get; set; }
        public string fullName { get; set; }
    }

    class Lesson
    {
        public CourseInfo courseInfo { get; set; }
        public string extLessonNo { get; set; }
    }

    class CourseInfo
    {
        public string courName { get; set; }
    }

    class Course
    {
        public string englishName { get; set; }
        public SchoolInfo school { get; set; }
        public string courType3 { get; set; }
        public string adviceHour { get; set; }
        public string batch { get; set; }
        public string activeStatus { get; set; }
        public string type5 { get; set; }
        public string extCourseNo { get; set; }
        public string courName { get; set; }
        public string courseId { get; set; }
        public string courType1 { get; set; }
        public string adviceCredit { get; set; }
        public string courType2 { get; set; }
        public string isPe { get; set; }
        public string isCore { get; set; }
    }
    
    class LessonSchedule
    {
        public Classroom classroom { get; set; }
        public TimeBlock timeBlock { get; set; }
        public string lsschId { get; set; }
    }

    class Classroom
    {
        public string roomId { get; set; }
        public string fullName { get; set; }
    }

    class TimeBlock
    {
        public string classSet { get; set; }
        public string name { get; set; }
        public string endWeek { get; set; }
        public string beginWeek { get; set; }
        public string tmbId { get; set; }
        public string dayOfWeek { get; set; }
        public string weekOddEven { get; set; }
    }

    class LessonTeacher
    {
        public string lstchId { get; set; }
        public string canScore { get; set; }
        public string canEval { get; set; }
        public string teachDuty { get; set; }
        public Teacher teacher { get; set; }
    }

    class Teacher
    {
        public string staffId { get; set; }
        public string staffStatus { get; set; }
        public string name { get; set; }
        public DateTime birthdate { get; set; }
        public string gender { get; set; }
        public string workerId { get; set; }
        public string teacherId { get; set; }
        public string profTitle { get; set; }
    }

    class TeachingTerm
    {
        public string termName { get; set; }
        public DateTime startDate { get; set; }
        public string termSeq { get; set; }
        public DateTime examDate { get; set; }
        public string activeStage { get; set; }
        public string year { get; set; }
        public DateTime vacationDate { get; set; }
        public string weeks { get; set; }
        public string termId { get; set; }
        public string egrade { get; set; }
    }

    class Student
    {
        public string studId { get; set; }
        public string name { get; set; }
        public AdminClass adminClass { get; set; }
        public string admissionYear { get; set; }
        public string studStatus { get; set; }
        public string studNo { get; set; }
        public string egrade { get; set; }
    }

    class AdminClass
    {
        public string adcId { get; set; }
        public string formalStudCnt { get; set; }
        public string graduateYear { get; set; }
        public string classNo { get; set; }
        public string studCnt { get; set; }
        public string className { get; set; }
        public string admissionYear { get; set; }
        public string campus { get; set; }
    }

    class MessageBox
    {
        public int count { get; set; }
        public int errno { get; set; }
        public string identifier { get; set; }
        public MessagePiece[] items { get; set; }
        public string label { get; set; }
        public string msg { get; set; }
        public int status { get; set; }
    }

    class MessagePiece
    {
        public MessageMain message { get; set; }
        public string msgInboxId { get; set; }
        public MessageReceiver receiver { get; set; }
        public object dateRead { get; set; }
        public string activeStatus { get; set; }
        public string hasReaded { get; set; }
        public object dateReceive { get; set; }

        public class MessageReceiver
        {
            public MessageSchool school { get; set; }
            public string name { get; set; }

            public class MessageSchool
            {
                public string schoolName { get; set; }
            }
        }

        public class MessageMain
        {
            public MessageSender sender { get; set; }
            public string body { get; set; }
            public string title { get; set; }
            public DateTime dateExpire { get; set; }
            public string messageId { get; set; }
            public DateTime dateCreate { get; set; }

            public class MessageSender
            {
                public string name { get; set; }
            }
        }
    }

    class CollegeInfo
    {
        public string schoolName { get; set; }
        public string website { get; set; }
        public string activeStatus { get; set; }
        public string abbr { get; set; }
        public object departments { get; set; }
        public string extSchNo { get; set; }
        public string schoolType { get; set; }
        public string englishName { get; set; }
        public Staff staff { get; set; }
        public string division { get; set; }
        public string schId { get; set; }
        public AdminClass[] adminClasses { get; set; }
        public string telephone { get; set; }
        public string introduction { get; set; }
        public string xscNo { get; set; }
        public string campus { get; set; }
    }
    
    class SchoolInfo
    {
        public string englishName { get; set; }
        public string schoolName { get; set; }
        public string division { get; set; }
        public string schId { get; set; }
        public string activeStatus { get; set; }
        public string extSchNo { get; set; }
        public string abbr { get; set; }
        public string schoolType { get; set; }
        public string xscNo { get; set; }
        public string campus { get; set; }
    }

    class RoomInfo
    {
        public string setColGroup { get; set; }
        struct buiding
        {
            public string name { get; set; }
        }
        public string floor { get; set; }
        public string setRows { get; set; }
        public string volume { get; set; }
        public string roomNo { get; set; }
        public string usage { get; set; }
        public string roomId { get; set; }
        public string clsrmType { get; set; }
        public string fullName { get; set; }
        public string examVolume { get; set; }
        public string notes { get; set; }
    }

    class Staff
    {
        public string staffId { get; set; }
        public string staffStatus { get; set; }
        public string name { get; set; }
        public DateTime birthdate { get; set; }
        public string gender { get; set; }
        public string workerId { get; set; }
    }

    class LoginValue
    {
        public string loginMethod { get; set; }
        public CacheUpdate cacheUpdate { get; set; }
        public string[] menusFile { get; set; }
        public int trulySch { get; set; }
        public GroupsInfo[] groupsInfo { get; set; }
        public string firstLogin { get; set; }
        public DefRes defRes { get; set; }
        public string userType { get; set; }
        public DateTime sysTime { get; set; }
        public string nickName { get; set; }
        public int userId { get; set; }
        public string welcome { get; set; }
        public string loginName { get; set; }

        public class CacheUpdate
        {
            public long sysDict { get; set; }
            public long code { get; set; }
        }

        public class DefRes
        {
            public int adcId { get; set; }
            public int term_l { get; set; }
            public int university { get; set; }
            public int teachingTerm { get; set; }
            public int school { get; set; }
            public int department { get; set; }
            public int term_a { get; set; }
            public int schType { get; set; }
            public int personId { get; set; }
            public int year { get; set; }
            public int term_s { get; set; }
            public int campus { get; set; }
        }

        public class GroupsInfo
        {
            public int groupId { get; set; }
            public string groupName { get; set; }
            public string menuFile { get; set; }
        }
    }

    class StudEval
    {
        public object personInput { get; set; }
        public object subject { get; set; }
        public Target target { get; set; }
        public string evalItemId { get; set; }
        public object notes { get; set; }
        public EvalActTime evalActTime { get; set; }
        public object dateInput { get; set; }
        public object targetClar { get; set; }

        public class Target
        {
            public School school { get; set; }
            public string name { get; set; }
            public string personId { get; set; }

            public class School
            {
                public string schoolName { get; set; }
            }
        }

        public class EvalActTime
        {
            public string actTimeId { get; set; }
            public EvalGuideLine evalGuideline { get; set; }
            public EvalTime evalTime { get; set; }
            public EvalActivity evalActivity { get; set; }
        }

        public class EvalGuideLine
        {
            public string evalGuidelineId { get; set; }
            public string paperUrl { get; set; }
            public string displayMode { get; set; }
        }

        public class EvalTime
        {
            public string title { get; set; }
            public string phaseType { get; set; }
            public string activeStatus { get; set; }
            public string etimeId { get; set; }
            public Ttl ttl { get; set; }
        }

        public class Ttl
        {
            public string ttlId { get; set; }
            public DateTime dateStop { get; set; }
            public string title { get; set; }
            public DateTime dateStart { get; set; }
        }

        public class EvalActivity
        {
            public string title { get; set; }
        }
    }

    class ProgItem
    {
        public string optionalCredit { get; set; }
        public SchoolInfo inputSchool { get; set; }
        public string practiceCredit { get; set; }
        public string extProgNo { get; set; }
        public LifeStatus lifeStatus { get; set; }
        public string obligatoryCredit { get; set; }
        public string title { get; set; }
        public object programDetails { get; set; }
        public string totalCredit { get; set; }
        public ApplyDept applyDept { get; set; }
        public string publicCredit { get; set; }
        public DateTime datePublish { get; set; }
        public string progId { get; set; }
        public string limitedCredit { get; set; }
        public string priority { get; set; }
        public object applyField { get; set; }
        public object notes { get; set; }
        public string beginEgrade { get; set; }

        public class LifeStatus
        {
            public string name { get; set; }
            public string label { get; set; }
        }

        public class ApplyDept
        {
            public string englishName { get; set; }
            public string deptName { get; set; }
            public string baseCampusYears { get; set; }
            public string activeStatus { get; set; }
            public string years { get; set; }
            public string deptId { get; set; }
            public string isJunior { get; set; }
            public string extDeptNo { get; set; }
            public string campus { get; set; }
        }
    }

    class ProgTerm
    {
        public string exprClassHour { get; set; }
        public string postGraduate { get; set; }
        public string testForm { get; set; }
        public string advGrade { get; set; }
        public string gradRequire { get; set; }
        public Course courseInfo { get; set; }
        public string progDId { get; set; }
        public string classHour { get; set; }
        public string termSeq { get; set; }
        public string exprCredit { get; set; }
        public string type5 { get; set; }
        public object type4 { get; set; }
        public string credit { get; set; }
        public object type6 { get; set; }
        public string introduction { get; set; }
        public object programMaster { get; set; }
    }
    
    class CollegeCourse
    {
        public object examCovers { get; set; }
        public TeachingTerm teachingTerm { get; set; }
        public object lessonSegments { get; set; }
        public string selectAdvice { get; set; }
        public string scoreStatus { get; set; }
        public Course courseInfo { get; set; }
        public SchoolInfo teachSchool { get; set; }
        public string activeStatus { get; set; }
        public string extLessonNo { get; set; }
        public object applyPlanLessons { get; set; }
        public object selectPlan { get; set; }
        public string lessonId { get; set; }
        public string totalClassHour { get; set; }
        public Staff leader { get; set; }
        public string tcmType { get; set; }
        public DateTime dateInput { get; set; }
    }

    class YktResult
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public object obj { get; set; }
    }

    class AlreadyKnownThings
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
        
        static Lazy<List<CollegeOverview>> collegeLoader = new Lazy<List<CollegeOverview>>(LazyCollege);
        static Lazy<List<BuildingOverview>> buildingLoader = new Lazy<List<BuildingOverview>>(LazyBuilding);
        static Lazy<NameValueCollection> campusLoader = new Lazy<NameValueCollection>(LazyCampus);
        static Lazy<NameValueCollection> divisionLoader = new Lazy<NameValueCollection>(LazyDivision);

        public static NameValueCollection Division => divisionLoader.Value;
        public static NameValueCollection Campus => campusLoader.Value;
        public static List<BuildingOverview> Buildings => buildingLoader.Value;
        public static List<CollegeOverview> Colleges => collegeLoader.Value;

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
            public string ToString(string type)
            {
                if (type == "option")
                {
                    string ret = $"<option value=\"{Id}\"";
                    if (Campus != null) ret += $" data-campus=\"{Campus}\"";
                    ret += $">{Name}</option>";
                    return ret;
                }
                return Name;
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

            public string ToString(string type)
            {
                if (type == "option")
                {
                    string ret = $"<option value=\"{Id}\"";
                    if (Campus != null) ret += $" data-campus=\"{Campus}\"";
                    if (Division != null) ret += $" data-part=\"{Division}\"";
                    ret += $">{Name}</option>";
                    return ret;
                }
                return Name;
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

        static NameValueCollection LazyCampus()
        {
            return new NameValueCollection
            {
                { "1401", "前卫校区" },
                { "1402", "南岭校区" },
                { "1403", "新民校区" },
                { "1404", "朝阳校区" },
                { "1405", "南湖校区" },
                { "1406", "和平校区" }
            };
        }

        static NameValueCollection LazyDivision()
        {
            return new NameValueCollection
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
    }
}

#pragma warning restore
