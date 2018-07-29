using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HandSchool.Droid
{
    [BroadcastReceiver(Label = "课程表")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/classtablewidgetprovider")]
    class ClassTableWidget: AppWidgetProvider
    {
        public bool Updated = false;
        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);
        }
        
        public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            int[] Temp = { appWidgetId };
            OnUpdate(context, appWidgetManager,Temp);
            return;
        }
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            for (int i = 0; i < appWidgetIds.Length; i++)
            {
                
                RemoteViews remoteViews = UpdateWidgetListView(context, appWidgetIds[i]);
                
                appWidgetManager.UpdateAppWidget(appWidgetIds[i], remoteViews);
            }
        }

        private RemoteViews UpdateWidgetListView(Context context, int appWidgetId)
        {

            RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.classtablewidget);

            string PACKAGE_NAME = context.PackageName;

            Intent svcIntent = new Intent(context, typeof(ClassTableRemoteService));

            svcIntent.SetPackage(PACKAGE_NAME);

            svcIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);

            svcIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToUri(Android.Content.IntentUriType.AndroidAppScheme)));

            remoteViews.SetRemoteAdapter(Resource.Id.ClassGrid, svcIntent);

            return remoteViews;
        }
        
    }
}