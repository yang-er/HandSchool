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

namespace HandSchool.Droid
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    class ClassTableRemoteService : RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {

            return new ClassTableWidgetFactory(ApplicationContext); 
        }
    }
}