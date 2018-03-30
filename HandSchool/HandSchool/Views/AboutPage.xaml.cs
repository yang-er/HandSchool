using System;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HandSchool.Internal.HtmlObject;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : ContentPage
	{
		public AboutPage ()
		{
			InitializeComponent();
            Version.Text = GetType().Assembly.GetName().Version.ToString();
            foreach (string title in (Application.Current as App).Support.Keys)
            {
                SupportedSchools.Text += title + "、";
            }
            SupportedSchools.Text = SupportedSchools.Text.Trim('、');
            if (SupportedSchools.Text == "") SupportedSchools.Text = "无";
        }

        private void Web1_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://github.com/yang-er"));
        }

        private async void Test_Clicked(object sender, EventArgs e)
        {
            var webpg = new WebViewPage(null);
            var sb = new StringBuilder();
            var HtmlObj = new Bootstrap
            {
                Children =
                {
                    new Internal.HtmlObject.Button
                    {
                        Title = "WTF Test",
                        Type = "onclick=\"invokeCSharpAction('hhhh, mmpd. ')\""
                    }
                }
            };
            HtmlObj.ToHtml(sb);
            webpg.WebView.Html = sb.ToString();
            webpg.WebView.RegisterAction(async (str) => await DisplayAlert("invokeCS", str, "ojbk"));
            webpg.ToolbarItems.Add(new ToolbarItem { Text = "Test", Command = new Command(() => webpg.WebView.JavaScript("$('button').removeClass('btn-primary').addClass('btn-danger')")) });
            await webpg.ShowAsync(Navigation);
        }
    }
}
