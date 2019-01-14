using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Forms
{
    public abstract class PlatformFormsImpl : PlatformBase
    {
        /// <summary>
        /// 输入文字请求的实现。
        /// </summary>
        /// <param name="sender">请求窗体</param>
        /// <param name="args">请求参数</param>
        public abstract void InputRequested(ViewPage sender, RequestInputArguments args);

        /// <summary>
        /// 展示图表请求的实现。
        /// </summary>
        /// <param name="sender">请求窗体</param>
        /// <param name="args">请求参数</param>
        public abstract void ChartRequested(ViewPage sender, RequestChartArguments args);

        /// <summary>
        /// 创建一个登录页面。
        /// </summary>
        /// <param name="viewModel">登录页面的视图模型。</param>
        /// <returns>登录页面</returns>
        public override ILoginPage CreateLoginPage(LoginViewModel viewModel) => new LoginPage(viewModel);

        /// <summary>
        /// 创建一个空白视图页面。
        /// </summary>
        /// <returns>视图页面的内容</returns>
        public override IViewPage CreatePage() => new ViewPage();

        /// <summary>
        /// 初始化平台相关的参数。
        /// </summary>
        protected PlatformFormsImpl()
        {
            MessagingCenter.Subscribe<ViewPage, RequestInputArguments>(this, ViewPage.RequestInputSignalName, InputRequested);
            MessagingCenter.Subscribe<ViewPage, RequestChartArguments>(this, ViewPage.RequestChartSignalName, ChartRequested);
        }

        /// <summary>
        /// 创建一个添加课程表的页面。
        /// </summary>
        /// <param name="item">课程表项</param>
        /// <param name="navigationContext">导航上下文</param>
        public override async Task<bool> ShowNewCurriculumPageAsync(CurriculumItem item, INavigate navigationContext)
        {
            var page = new CurriculumPage(item, true);
            await page.ShowAsync(navigationContext);
            return false;
        }
    }
}
