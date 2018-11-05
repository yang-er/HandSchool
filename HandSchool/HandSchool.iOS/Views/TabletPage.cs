using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HandSchool.Views
{
    public partial class MyTabletPage : MasterDetailPage
    {
        public MyTabletPage(string wtf)
        {
            MasterBehavior = MasterBehavior.Split;
            Master = new NavigationPage(new FeedPage()) { Title = "xxcx" };
            Detail = new NavigationPage(new PopContentPage()
            {
                Title = "222",
                BackgroundColor = Color.Black
            });
        }
    }
}
