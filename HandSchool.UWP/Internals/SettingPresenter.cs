using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using HandSchool.UWP;

namespace HandSchool.Views
{
    public class SettingPresenter : IViewPresenter
    {
        public int PageCount => 2;

        public string Title => "设置";

        public IViewPage[] GetAllPages()
        {
            return new IViewPage[]
            {
                new SettingPage() { Title = "设置" },
                new AboutPageView() { Title = "关于" },
            };
        }
    }
}