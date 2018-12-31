namespace HandSchool.Models
{
    /// <summary>
    /// 描述课程的具体信息，用于显示。
    /// </summary>
    public class CurriculumDescription
    {
        internal CurriculumDescription(string tit, string desc)
        {
            Title = tit;
            Description = desc;
        }

        /// <summary>
        /// 课程的标题。
        /// </summary>
        public readonly string Title;

        /// <summary>
        /// 课程的描述，如操作地点和时间等。
        /// </summary>
        public readonly string Description;
    }
}
