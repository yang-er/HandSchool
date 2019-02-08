namespace HandSchool.Views
{
    /// <summary>
    /// 忙信号的最小绑定
    /// </summary>
    public interface IBusySignal
    {
        /// <summary>
        /// 页面是否正忙
        /// </summary>
        bool IsBusy { get; set; }
    }
}