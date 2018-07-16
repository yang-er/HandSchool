namespace HandSchool.Services
{
    /// <summary>
    /// 加载学校的接口
    /// </summary>
    public interface ISchoolWrapper
    {
        /// <summary>
        /// 在加载之前运行
        /// </summary>
        void PreLoad();

        /// <summary>
        /// 加载完后运行
        /// </summary>
        void PostLoad();

        /// <summary>
        /// 学校的名称
        /// </summary>
        string SchoolName { get; }
        
        /// <summary>
        /// 学校的简写字母
        /// </summary>
        string SchoolId { get; }
    }
}
