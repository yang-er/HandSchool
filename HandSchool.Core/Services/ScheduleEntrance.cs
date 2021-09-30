namespace HandSchool.Services
{
    public enum SectionState
    {
        ClassOn, ClassOver
    }
    /// <summary>
    /// 课程表获取
    /// </summary>
    /// <inheritdoc cref="ISystemEntrance"/>
    public interface IScheduleEntrance : ISystemEntrance
    {
        /// <summary>
        /// 当前是第几节
        /// </summary>
        (int section, SectionState? state) GetCurrentClass();

        /// <summary>
        /// 当前是第几节
        /// </summary>
        (int section, SectionState? state) CurrentClass { get; }
    }
}
