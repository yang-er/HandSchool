using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.Droid;

namespace HandSchool.Views
{
    public class IndexPage : ViewFragment
    {
        public IndexPage()
        {
            FragmentViewResource = Resource.Layout.layout_index;
            Title = "掌上校园";
        }
    }
}