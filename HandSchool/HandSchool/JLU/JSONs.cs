namespace HandSchool.JLU
{
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
}
