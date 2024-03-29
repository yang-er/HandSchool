﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;

#pragma warning disable IDE1006

namespace HandSchool.JLU.JsonObject
{
    public class RootObject<T>
    {
        public string id { get; set; }
        public int status { get; set; }
        public T[] value { get; set; }
        public string resName { get; set; }
        public string msg { get; set; }
        public string data { get; set; }
    }

    public class ErrorMsg
    {
        public int status { get; set; }
        public string msg { get; set; }
    }

    public class GPAValue
    {
        public double? avgScoreBest { get; set; }
        public double? avgScoreFirst { get; set; }
        public double? gpaFirst { get; set; }
        public double? gpaBest { get; set; }

        /// <summary>
        /// 此属性指示条目中是否含有空值
        /// </summary>
        public bool HasNull => avgScoreBest == null || avgScoreFirst == null || gpaFirst == null || gpaBest == null;
    }

    public class TeachClassMaster
    {
        public string maxStudCnt { get; set; }
        public LessonSchedule[] lessonSchedules { get; set; }
        public string studCnt { get; set; }
        public LessonTeacher[] lessonTeachers { get; set; }
        public string name { get; set; }
        public string tcmId { get; set; }
        public LessonSegment lessonSegment { get; set; }
    }

    public class LessonIdList
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

    public class ScheduleValue
    {
        public TeachClassMaster teachClassMaster { get; set; }
        public string tcsId { get; set; }
        public object student { get; set; }
        public DateTime dateAccept { get; set; }
    }

    public class ArchiveScoreValue
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
        public StudentBasic student { get; set; }
        public string asId { get; set; }
        public string type5 { get; set; }
        public string gpoint { get; set; }
        public string credit { get; set; }
        public object notes { get; set; }
        public string selectType { get; set; }
        public GradeDetails distribute { get; set; }
    }

    public class QueryScoreValue
    {
        public class FirstTerm
        {
            public int? termId { get; set; }
            public string termName { get; set; }
        }

        public int? passScoreNum { get; set; }
        public Student student { get; set; }
        public string bestXkkh { get; set; }
        public string hasReselect { get; set; }
        public int? scoreSign { get; set; }
        public string firstXkkh { get; set; }
        public string bestGpoint { get; set; }
        public FirstTerm firstTerm { get; set; }
        public string passGpoint { get; set; }
        public string planDetail { get; set; }
        public CourseBasic course { get; set; }
        public string classHour { get; set; }
        public string credit { get; set; }
        public string type5 { get; set; }
        public string firstGpoint { get; set; }
        public string bestScore { get; set; }
        public string passScore { get; set; }
        public string isMinor { get; set; }
        public int? firstScoreNum { get; set; }
        public string passXkkh { get; set; }
        public string studyCnt { get; set; }
        public string firstScore { get; set; }
        public string isPass { get; set; }
        public int? bestScoreNum { get; set; }
        public string sbId { get; set; }
    }
    

    public class GradeDetails
    {
        public int count { get; set; }
        public int errno { get; set; }
        public string identifier { get; set; }
        public GradeEntry[] items { get; set; }
        public string label { get; set; }
        public string msg { get; set; }
        public int status { get; set; }

        public class GradeEntry
        {
            public int count { get; set; }
            public string label { get; set; }
            public float percent { get; set; }
            public int seq { get; set; }
        }
    }

    public class OutsideScoreValue
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

    public class AdminClassSchedule : ScheduleValue
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

    public class LessonSegment
    {
        public string classHour { get; set; }
        public string lssgId { get; set; }
        public Lesson lesson { get; set; }
        public string weekHour { get; set; }
        public string segType { get; set; }
        public string lsSeq { get; set; }
        public string fullName { get; set; }
    }

    public class Lesson
    {
        public CourseInfo courseInfo { get; set; }
        public string extLessonNo { get; set; }
    }

    public class CourseInfo
    {
        public string courName { get; set; }
    }

    public class CourseBasic
    {
        public string extCourseNo { get; set; }

        public int courseId { get; set; }
        public string courName { get; set; }
    }
    public class Course : CourseBasic
    {
        public string englishName { get; set; }
        public SchoolInfo school { get; set; }
        public string courType3 { get; set; }
        public string adviceHour { get; set; }
        public string batch { get; set; }
        public string activeStatus { get; set; }
        public string type5 { get; set; }
        public string courType1 { get; set; }
        public string adviceCredit { get; set; }
        public string courType2 { get; set; }
        public string isPe { get; set; }
        public string isCore { get; set; }
    }

    public class LessonSchedule
    {
        public Classroom classroom { get; set; }
        public TimeBlock timeBlock { get; set; }
        public string lsschId { get; set; }
        public TeachClassMaster teachClassMaster { get; set; }
    }

    public class Classroom
    {
        public string roomId { get; set; }
        public string fullName { get; set; }
    }

    public class TimeBlock
    {
        public string classSet { get; set; }
        public string name { get; set; }
        public string endWeek { get; set; }
        public string beginWeek { get; set; }
        public string tmbId { get; set; }
        public string dayOfWeek { get; set; }
        public string weekOddEven { get; set; }
    }

    public class LessonTeacher
    {
        public string lstchId { get; set; }
        public string canScore { get; set; }
        public string canEval { get; set; }
        public string teachDuty { get; set; }
        public Teacher teacher { get; set; }
    }

    public class Teacher
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

    public class TeachingTerm
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
    public class StudentBasic
    {
        public string studNo { get; set; }
        public string name { get; set; }
        public int studId { get; set; }
    }

    public class Student : StudentBasic
    {
        public AdminClass adminClass { get; set; }
        public string admissionYear { get; set; }
        public string studStatus { get; set; }
        public string egrade { get; set; }
    }

    public class AdminClass
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

    public class MessageBox
    {
        public int count { get; set; }
        public int errno { get; set; }
        public string identifier { get; set; }
        public MessagePiece[] items { get; set; }
        public string label { get; set; }
        public string msg { get; set; }
        public int status { get; set; }
    }

    public class MessagePiece
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

    public class CollegeInfo
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

    public class SchoolInfo
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

    public class RoomInfo
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

    public class Staff
    {
        public string staffId { get; set; }
        public string staffStatus { get; set; }
        public string name { get; set; }
        public DateTime birthdate { get; set; }
        public string gender { get; set; }
        public string workerId { get; set; }
    }

    public class LoginValue
    {
        public string loginMethod { get; set; }
        public CacheUpdate cacheUpdate { get; set; }
        public string[] menusFile { get; set; }
        public int trulySch { get; set; }
        public GroupsInfo[] groupsInfo { get; set; }
        public string firstLogin { get; set; }
        public LoginInfo loginInfo { get; set; }
        public DefRes defRes { get; set; }
        public string userType { get; set; }
        public DateTime sysTime { get; set; }
        public string nickName { get; set; }
        public int userId { get; set; }
        public string welcome { get; set; }
        public string loginName { get; set; }

        public class LoginInfo
        {
            public string method { get; set; }
            public string nickName { get; set; }
            public string loginName { get; set; }
            public int loginCounter { get; set; }
            public string lrToken { get; set; }
        }

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
    
    public class ProgItem
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

    public class ProgTerm
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

    public class CollegeCourse
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

    public class YktResult
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public object obj { get; set; }
    }

    public class CJCXCJ
    {
        public int count { get; set; }
        public int errno { get; set; }
        public string id { get; set; }
        public Item[] items { get; set; }
        public string msg { get; set; }
        public string resName { get; set; }
        public int status { get; set; }

        public class Item
        {
            public string isReselect { get; set; }
            public string xkkh { get; set; }
            public string studName { get; set; }
            public string xh { get; set; }
            public string lsrId { get; set; }
            public string cj { get; set; }
            public decimal gpoint { get; set; }
            public decimal zscj { get; set; }
            public decimal credit { get; set; }
            public string kcmc { get; set; }
            public int termId { get; set; }
        }
    }

    public class DigResultStatus
    {
        public string message { get; set; }
        public string code { get; set; }
    }

    public class DigResultValue
    {
        public string title { get; set; }
        public string depart { get; set; }
        public string publishdate { get; set; }
        public string link { get; set; }
        public string content { get; set; }
        public bool flgtop { get; set; }
    }

    public class OaListRootObject
    {
        public DigResultStatus resultStatus { get; set; }
        public DigResultValue[] resultValue { get; set; }
    }

    public class OaItemRootObject
    {
        public DigResultStatus resultStatus { get; set; }
        public DigResultValue resultValue { get; set; }
    }

    public class RemoteTcm
    {
        public TeachClassMaster teachClassMaster { get; set; }

        public class TcmRemote
        {
            public string platform { get; set; }
            public string contactGroup { get; set; }
            public int tcmId { get; set; }
            public string teachMethod { get; set; }
        }

        public class LessonSegment
        {
            public string fullName { get; set; }
        }

        public class TeachClassMaster
        {
            public string name { get; set; }
            public TcmRemote tcmRemote { get; set; }
            public int tcmId { get; set; }
            public LessonSegment lessonSegment { get; set; }
        }
    }
}

#pragma warning restore
