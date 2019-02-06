namespace HandSchool.Views
{
    /// <summary>
    /// 页面的生命周期控制接口。
    /// </summary>
    public interface IViewLifecycle
    {
        /// <summary>
        /// 向页面发送“正在显示”的信号。
        /// </summary>
        void SendAppearing();

        /// <summary>
        /// 向页面发送“正在关闭”的信号。
        /// </summary>
        void SendDisappearing();

        /// <summary>
        /// 设置页面的导航参数。
        /// </summary>
        void SetNavigationArguments(object param);

        /// <summary>
        /// 注册页面的导航控制器。
        /// </summary>
        /// <param name="navigate">导航控制器</param>
        void RegisterNavigation(INavigate navigate);
    }
}