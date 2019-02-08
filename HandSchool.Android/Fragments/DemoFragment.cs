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
using HandSchool.Views;

namespace HandSchool.Droid
{
    public class DemoFragment : ViewFragment
    {
        public DemoFragment()
        {
            FragmentViewResource = Resource.Layout.layout_index;
            Title = "测试页面";
            
        }
    }
}