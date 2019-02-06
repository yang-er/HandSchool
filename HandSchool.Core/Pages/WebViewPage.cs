using HandSchool.ViewModels;

namespace HandSchool.Views
{
    public interface IWebViewPage : IViewPage
    {
        BaseController Controller { get; }
    }
}