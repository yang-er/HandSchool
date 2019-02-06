using HandSchool.Internals.HtmlObject;

namespace HandSchool.Services
{
    /// <summary>
    /// 信息入口点
    /// </summary>
    /// <inheritdoc cref="IWebEntrance"/>
    public interface IInfoEntrance : IWebEntrance
    {
        /// <summary>
        /// 使用的Bootstrap文档
        /// </summary>
        Bootstrap HtmlDocument { get; set; }
    }
}
