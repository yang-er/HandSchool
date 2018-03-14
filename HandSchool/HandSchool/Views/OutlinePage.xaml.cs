using HandSchool.Internal;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OutlinePage : ContentPage
	{
		public OutlinePage ()
		{
			InitializeComponent ();
            
            var primaryItems = new List<MasterPageItem>() {
                new MasterPageItem
                {
                    Title = "BROWSE",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE10F",
                    Color = Color.DeepSkyBlue,
                    Selected = true,
                    DestPage = typeof(ItemsPage)
                },
                new MasterPageItem
                {
                    Title = "课程表",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE11F",
                    Color = Color.Black,
                    Selected = false,
                    DestPage = typeof(SchedulePage)
                },
                new MasterPageItem
                {
                    Title = "学分成绩",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE12F",
                    Color = Color.Black,
                    Selected = false,
                    DestPage = typeof(GradePointPage)
                }
            };

            var secondaryItems = new List<MasterPageItem>() {
                new MasterPageItem
                {
                    Title = "关于",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE783",
                    Color = Color.Black,
                    Selected = false,
                    DestPage = typeof(AboutPage)
                }
            };
            
            PrimaryListView.ItemsSource = primaryItems;
            SecondaryListView.ItemsSource = secondaryItems;
            
            SecondaryListView.HeightRequest = 48 * secondaryItems.Count;
        }
	}
}