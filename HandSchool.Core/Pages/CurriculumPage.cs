using HandSchool.Models;
using System.Threading.Tasks;

namespace HandSchool.Views
{
    /// <summary>
    /// 课程修改的页面。
    /// </summary>
    public interface ICurriculumPage : IViewPage
    {
        /// <summary>
        /// 设置的课程表内容
        /// </summary>
        CurriculumItem Model { get; }

        /// <summary>
        /// 设置导航参数，保存使用的类型。
        /// </summary>
        /// <param name="item">视图模型</param>
        /// <param name="isCreate">是否为新建</param>
        void SetNavigationArguments(CurriculumItem item, bool isCreate);

        /// <summary>
        /// 显示登录对话框。
        /// </summary>
        Task<bool> ShowAsync();
    }
}