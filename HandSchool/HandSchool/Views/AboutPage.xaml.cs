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
        
        private void Web2_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("http://www.90yang.com"));
        }
    }
}
