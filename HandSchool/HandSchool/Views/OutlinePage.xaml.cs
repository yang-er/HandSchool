using HandSchool.Internal;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OutlinePage : ContentPage
	{
        public List<MasterPageItem> PrimaryItems;
        public List<MasterPageItem> SecondaryItems;

        public event EventHandler LoadingExtraItem;

        public OutlinePage()
		{
			InitializeComponent();

            PrimaryItems = new List<MasterPageItem>() {
                new MasterPageItem
                {
                    Title = "BROWSE",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE10F",
                    Color = Color.DeepSkyBlue,
                    Selected = true,
                    DestPage = new ItemsPage()
                },
                new MasterPageItem
                {
                    Title = "课程表",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE11F",
                    Color = Color.Black,
                    Selected = false,
                    DestPage = new SchedulePage()
                },
                new MasterPageItem
                {
                    Title = "学分成绩",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE12F",
                    Color = Color.Black,
                    Selected = false,
                    DestPage = new GradePointPage()
                }
            };

            SecondaryItems = new List<MasterPageItem>() {
                new MasterPageItem
                {
                    Title = "关于",
                    FontFamily = Helper.SegoeMDL2,
                    Icon = "\xE783",
                    Color = Color.Black,
                    Selected = false,
                    DestPage = new AboutPage()
                }
            };
            
            PrimaryListView.ItemsSource = PrimaryItems;
            SecondaryListView.ItemsSource = SecondaryItems;

            LoadingExtraItem?.Invoke(this, new EventArgs());

            SecondaryListView.HeightRequest = 48 * SecondaryItems.Count;
        }
	}
}