using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

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
                new AboutPage() { Title = "关于" },
            };
        }
    }

    public sealed class AboutPage : ViewObject
    {
        public AboutPage()
        {
            Content = new ScrollView { Content = new AboutView().ToView() };
            Content.VerticalOptions = LayoutOptions.FillAndExpand;
            ViewModel = AboutViewModel.Instance;
        }
    }
}