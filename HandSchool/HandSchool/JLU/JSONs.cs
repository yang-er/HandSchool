using System;

namespace HandSchool.JLU.JsonObject
{
    public class RootObject<T>
    {
        public string id { get; set; }
        public int status { get; set; }
        public T[] value { get; set; }
        public string resName { get; set; }
        public string msg { get; set; }
    }

    public class ErrorMsg
    {
        public int status { get; set; }
        public string msg { get; set; }
    }

    public class GPAValue
    {
        public float avgScoreBest { get; set; }
        public float avgScoreFirst { get; set; }
        public float gpaFirst { get; set; }
        public float gpaBest { get; set; }
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
        public Student student { get; set; }
        public string asId { get; set; }
        public string type5 { get; set; }
        public string gpoint { get; set; }
        public string credit { get; set; }
        public object notes { get; set; }
        public string selectType { get; set; }
    }

    public class LessonSegment
    {
        public string lssgId { get; set; }
        public Lesson lesson { get; set; }
        public string fullName { get; set; }
    }

    public class Lesson
    {
        public CourseInfo courseInfo { get; set; }
    }

    public class CourseInfo
    {
        public string courName { get; set; }
    }

    public class Course
    {
        public string englishName { get; set; }
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

    public class LessonSchedule
    {
        public Classroom classroom { get; set; }
        public TimeBlock timeBlock { get; set; }
        public string lsschId { get; set; }
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
        public Teacher teacher { get; set; }
    }

    public class Teacher
    {
        public string name { get; set; }
        public string teacherId { get; set; }
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

    public class Student
    {
        public string studId { get; set; }
        public string name { get; set; }
        public AdminClass adminClass { get; set; }
        public string admissionYear { get; set; }
        public string studStatus { get; set; }
        public string studNo { get; set; }
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
    }

    public class MessageMain
    {
        public MessageSender sender { get; set; }
        public string body { get; set; }
        public string title { get; set; }
        public DateTime dateExpire { get; set; }
        public string messageId { get; set; }
        public DateTime dateCreate { get; set; }
    }

    public class MessageReceiver
    {
        public MessageSchool school { get; set; }
        public string name { get; set; }
    }

    public class MessageSchool
    {
        public string schoolName { get; set; }
    }

    public class MessageSender
    {
        public string name { get; set; }
    }
}
