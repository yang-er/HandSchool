using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            webpg.WebView.Uri = "bootstrap.html";
            webpg.ToolbarItems.Add(new ToolbarItem { Text = "Test", Command = new Command(() => webpg.WebView.JavaScript("$('.table tbody').append('<tr><th scope=\"row\">4</th><td>What</td><td>the</td><td>@fuck</td></tr>')")) });
            await webpg.ShowAsync(Navigation);
        }
    }
}
