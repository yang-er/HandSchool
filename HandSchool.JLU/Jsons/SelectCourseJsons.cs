using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HandSchool.JLU.JsonObject
{
    public class SelectCoursePlanValue
    {
        private string _currStartTime;

        public string currStartTime
        {
            get => _currStartTime;
            set
            {
                _currStartTime = value.Replace('T', ' ');
                try
                {
                    StartTime = Convert.ToDateTime(_currStartTime);
                }
                catch (Exception)
                {
                    StartTime = null;
                }
            }
        }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        private string _currStopTime;

        public string currStopTime
        {
            get => _currStopTime;
            set
            {
                _currStopTime = value.Replace('T', ' ');
                EndTime = Convert.ToDateTime(_currStopTime);
            }
        }
        public string isOpen { get; set; }
        public string hasReselect { get; set; }
        public int? currStage { get; set; }
        public int? splanId { get; set; }
        public string title { get; set; }

        public class TeachingTerm
        {
            public int? termId { get; set; }
            public int? termSeq { get; set; }
            public string termName { get; set; }
            public int? egrade { get; set; }
        }
        public TeachingTerm teachingTerm { get; set; }
        public string includeGx { get; set; }
        public override string ToString() => title;
    }
    public class SCCourses
    {
        internal IDictionary<int, string> SCType =
            new Dictionary<int, string>
            {
                {3060, "必修课"},
                {3061, "选修课"},
                {3062, "限选课"},
                {3064, "体育课"},
                {3065, "校选修课"},
                {3066, "跨专业"},
                {3067, "通过再修"},
                {3068, "补修"},
                {3099, "绑定公选"}
            };
        public class SCApplyPlanLesson
        {
            public int? aplId { get; set; }
            public int? selectType { get; set; }
            public string isReselect { get; set; }
            public int? credit { get; set; }
        }
        public class SCTeachSchool
        {
            public string schoolName { get; set; }
        }
        public class SCCourseInfo
        {
            public int courseId { get; set; }
            public string courName { get; set; }
        }
        public class SCLesson
        {
            public int? totalClassHour { get; set; }
            public SCTeachSchool teachSchool { get; set; }
            public int? lessonId { get; set; }
            public SCCourseInfo courseInfo { get; set; }
        }

        public SCApplyPlanLesson applyPlanLesson { get; set; }
        public string notes { get; set; }
        public int selLssgCnt { get; set; }
        public string replyTag { get; set; }
        public string selectResult { get; set; }
        public int lslId { get; set; }
        public SCLesson lesson { get; set; }
        public int sumLssgCnt { get; set; }
        public string SelectResult => selectResult.Trim() == "Y" ? "已选" : "未选";
        public Color IsSelectColor => selectResult.Trim() == "Y" ? Color.Green : Color.Red;
        public string CreditAndHour => "学分：" + applyPlanLesson.credit + "  学时：" + lesson.totalClassHour;
        public string SchoolName => "开课学院：" + lesson.teachSchool.schoolName;
        public string Type
        {
            get
            {
                if (SCType.ContainsKey(applyPlanLesson.selectType ?? 0))
                    return "类型：" + SCType[applyPlanLesson.selectType ?? 0];
                else return "未知类型";
            }
        }
        public string IsReselect => "重选：" + (applyPlanLesson.isReselect?.Trim() == "Y" ? "是" : "否");
        public override string ToString()
        {
            return lesson.courseInfo.courName;
        }
    }
    public class SCCourseDetail
    {
        public class ApplyPlanLesson
        {
            public string aplId { get; set; }
            public string selectType { get; set; }
            public string isReselect { get; set; }
            public string credit { get; set; }
        }
        public class CourseInfo
        {
            public string courName { get; set; }
        }
        public class Lesson
        {
            public string extLessonNo { get; set; }
            public string lessonId { get; set; }
            public string dateInput { get; set; }
            public string tcmType { get; set; }
            public string totalClassHour { get; set; }
            public string activeStatus { get; set; }
            public string selectAdvice { get; set; }
            public CourseInfo courseInfo { get; set; }
        }
        public class LessonSelectLog
        {
            public ApplyPlanLesson applyPlanLesson { get; set; }
            public Lesson lesson { get; set; }
        }

        public class Teacher
        {
            public string teacherId { get; set; }
            public string name { get; set; }
        }

        public class LessonTeachers
        {
            public Teacher teacher { get; set; }
        }

        public string Teachers
        {
            get
            {
                var sb = new StringBuilder();
                teachClassMaster.lessonTeachers.ForEach(t => sb.Append(t.teacher.name).Append(" "));
                return sb.ToString();
            }
        }
        public class Classroom
        {
            public string fullName { get; set; }
            public string roomId { get; set; }
        }

        public class TimeBlock
        {
            public string tmbId { get; set; }
            public string beginWeek { get; set; }
            public string dayOfWeek { get; set; }
            public string name { get; set; }
            public string endWeek { get; set; }
            public string classSet { get; set; }
        }

        public class LessonSchedules
        {
            public Classroom classroom { get; set; }
            public TimeBlock timeBlock { get; set; }
        }

        public class TeachClassMaster
        {
            public List<LessonTeachers> lessonTeachers { get; set; }
            public string tcmId { get; set; }
            public string dateCreate { get; set; }
            public string tcmType { get; set; }
            public string maxStudCnt { get; set; }
            public string campus { get; set; }
            public string studCnt { get; set; }
            public string egrade { get; set; }
            public string fixedStudCnt { get; set; }
            public string activeStatus { get; set; }
            public List<LessonSchedules> lessonSchedules { get; set; }

            public string name { get; set; }
        }

        public class LessonSegment
        {
            public string fullName { get; set; }
        }

        public string notes { get; set; }
        public string selectTag { get; set; }
        public string dateSelect { get; set; }
        public LessonSelectLog lessonSelectLog { get; set; }
        public string adviceTag { get; set; }
        public string isQuick { get; set; }
        public string replyTag { get; set; }
        public TeachClassMaster teachClassMaster { get; set; }
        public string lsltId { get; set; }
        public LessonSegment lessonSegment { get; set; }
        public string TimeAndLocation
        {
            get
            {
                var sb = new StringBuilder();
                teachClassMaster.lessonSchedules.ForEach(v =>
                {
                    sb.AppendLine(v.classroom.fullName)
                        .AppendLine(v.timeBlock.name)
                        .Append("\n");
                });
                if (sb.Length > 1)
                    sb.Remove(sb.Length - 2, 2);
                return sb.ToString();
            }
        }
        public string Advice
        {
            get
            {
                var value = adviceTag?.Trim();
                return value switch
                {
                    "A" => "推荐",
                    "G" => "固定",
                    _ => "-"
                };
            }
        }
        public string Selected
        {
            get
            {
                var val = selectTag?.Trim();
                return val switch
                {
                    "Y" => "已选",
                    "G" => "已固定",
                    _ => "未选"
                };
            }
        }

        public string StuCount => $"容量：{teachClassMaster.studCnt}/{teachClassMaster.maxStudCnt}";
        public string SchoolArea
        {
            get
            {
                var campus = AlreadyKnownThings.Campus;
                if (campus.ContainsKey(teachClassMaster.campus))
                {
                    return campus[teachClassMaster.campus];
                }
                return "未知校区";
            }
        }
    }

}
