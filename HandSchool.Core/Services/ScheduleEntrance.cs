namespace HandSchool.Services
{
    /// <summary>
    /// 课程表获取
    /// </summary>
    /// <inheritdoc cref="ISystemEntrance"/>
    public interface IScheduleEntrance : ISystemEntrance
    {
        /// <summary>
        /// 下一节课
        /// </summary>
        int GetClassNext();

        /// <summary>
        /// 下一节课
        /// </summary>
        int ClassNext { get; }
    }
}
