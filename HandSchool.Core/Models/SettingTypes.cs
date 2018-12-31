namespace HandSchool.Models
{
    /// <summary>
    /// 设置中心的条目类型。
    /// </summary>
    public enum SettingTypes
    {
        /// <summary>
        /// 未知类型
        /// </summary>
        Unknown,

        /// <summary>
        /// 数字类型
        /// </summary>
        Integer,

        /// <summary>
        /// 文本类型
        /// </summary>
        String,

        /// <summary>
        /// 常量说明
        /// </summary>
        Const,

        /// <summary>
        /// 开关类型
        /// </summary>
        Boolean,

        /// <summary>
        /// 行为类型
        /// </summary>
        Action,
    }
}
