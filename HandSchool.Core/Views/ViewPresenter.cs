namespace HandSchool.Views
{
    /// <summary>
    /// 展示页面用的接口要求。
    /// </summary>
    public interface IViewPresenter
    {
        /// <summary>
        /// 新创建一套页面。
        /// </summary>
        /// <returns>页面内容</returns>
        IViewPage[] GetAllPages();

        /// <summary>
        /// 页面数量
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// 标题
        /// </summary>
        string Title { get; }
    }
}
