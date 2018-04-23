using HandSchool.ViewModels;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MainPage : TabbedPage
    {
        public MainPage()
        {
            if (!Core.App.Loaded)
            {
                Children.Add(new SelectTypePage() { Title = "选择学校", Icon = "tab_feed.png" });
            }
            else
            {
                NavigationViewModel.Instance.PrimaryItems.ForEach((obj) => Children.Add(obj.DestPage));
            }
        }
        
        public void FinishSettings()
        {
            NavigationViewModel.Instance.PrimaryItems.ForEach((obj) => Children.Add(obj.DestPage));
            Children.RemoveAt(0);
        }
    }
}