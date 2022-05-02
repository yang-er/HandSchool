using HandSchool.JLU.JsonObject;

namespace HandSchool.JLU.Models
{
    public class EvaItemShell
    {
        public StudEval InnerInfo { get; }

        public EvaItemShell(StudEval studEval, bool isEnable)
        {
            InnerInfo = studEval;
            IsEnable = isEnable;
        }

        public bool IsEnable { get; set; }

        public string TeacherName => InnerInfo?.target?.name;
        public string TeachCourse => InnerInfo?.targetClar?.notes;

        public bool IsEvaluated => !string.IsNullOrWhiteSpace(InnerInfo?.dateInput);

        public string EvalGuidelineId => InnerInfo?.evalActTime?.evalGuideline?.evalGuidelineId;

        public Xamarin.Forms.Color Color
        {
            get
            {
                if (!IsEnable) return Xamarin.Forms.Color.Black;
                return IsEvaluated ? Xamarin.Forms.Color.Gray : Xamarin.Forms.Color.Red;
            }
        }

        public string Detail
        {
            get
            {
                if (!IsEnable) return "不可用";
                return IsEvaluated ? "已评" : "未评";
            }
        }
    }
}