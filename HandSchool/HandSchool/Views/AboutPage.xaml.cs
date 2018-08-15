using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HandSchool.Services;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : PopContentPage
	{
		public AboutPage ()
		{
            
			InitializeComponent ();
            ViewModel = AboutViewModel.Instance;
        }
        async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            if (e.Item is InfoEntranceWrapper iew)
            {
                if (iew.Name == "检查更新")
                {
#if __ANDROID__
                    Droid.MainActivity.Instance.Update();
#endif
                    //TODO 果子更新
                    return;
                }
                var a = iew.LoadURL.Invoke();
                if(a.HtmlUrl.Contains("http"))

                    Device.OpenUri(new Uri(a.HtmlUrl));
                else
                {
                    var webpg = new WebViewPage((IUrlEntrance)iew.LoadURL.Invoke());
                    await webpg.ShowAsync(Navigation);
                }
                
            }
            else if (e.Item is TapEntranceWrapper tew)
            {
                System.Diagnostics.Debug.Assert(Core.RuntimePlatform == "iOS");
                await tew.Activate(Navigation);
            }
        }

    }
}