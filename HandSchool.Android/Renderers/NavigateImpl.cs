using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.Views;

namespace HandSchool.Droid
{
    public class NavigateImpl : INavigate
    {
        public IReadOnlyList<IViewPage> NavigationStack => throw new NotImplementedException();

        public Task<IViewPage> PopAsync()
        {
            throw new NotImplementedException();
        }
        
        public Task PushAsync(string pageType, object param)
        {
            throw new NotImplementedException();
        }
        
        public Task PushAsync(Type pageType, object param)
        {
            throw new NotImplementedException();
        }
    }
}