using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace HandSchool.Droid
{
    class RefreshService : Service
    {
        public static string TAG = "RefreshService";

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(TAG, " ServiceCreating");
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Log.Debug(TAG, "  onStartcommand");
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            Log.Debug(TAG, " ondestory");
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent)
        {

            throw new NotImplementedException();
        }
    }
}