using System;
using System.Collections.Generic;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Util;
using Android.Widget;
using HandSchool.ViewModels;

namespace HandSchool.Droid
{
	[BroadcastReceiver(Label = "HellApp Widget")]
	[IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	// The "Resource" file has to be all in lower caps
	[MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
	public class AppWidget : AppWidgetProvider
	{
		private static string AnnouncementClick = "AnnouncementClickTag";


        /// <summary>
        /// This method is called when the 'updatePeriodMillis' from the AppwidgetProvider passes,
        /// or the user manually refreshes/resizes.
        /// </summary>
        public AppWidget():base()
        {
            Log.Debug("[Widget]", "初始化");
        }
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
		{
            var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
            var View = BuildRemoteViews(context, appWidgetIds);
            View.SetTextViewText(Resource.Id.widgetMedium, IndexViewModel.Instance.NextClass);
            appWidgetManager.UpdateAppWidget(me, View);
		}

		private RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
		{
            // Retrieve the widget layout. This is a RemoteViews, so we can't use 'FindViewById'
            
            var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);

			SetTextViewText(widgetView);
			RegisterClicks(context, appWidgetIds, widgetView);
            
			return widgetView;
		}

		private void SetTextViewText(RemoteViews widgetView)
		{
			widgetView.SetTextViewText(Resource.Id.widgetMedium, "刷新中");
			widgetView.SetTextViewText(Resource.Id.widgetSmall, string.Format("{0:H:mm:ss}", DateTime.Now));
		}

		private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
		{
			var intent = new Intent(context, typeof(AppWidget));
			intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
			intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

			// Register click event for the Background
			var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
			widgetView.SetOnClickPendingIntent(Resource.Id.widgetBackground, piBackground);

            // Register click event for the Announcement-icon
        }

		private PendingIntent GetPendingSelfIntent(Context context, string action)
		{
			var intent = new Intent(context, typeof(AppWidget));
			intent.SetAction(action);
			return PendingIntent.GetBroadcast(context, 0, intent, 0);
		}

		/// <summary>
		/// This method is called when clicks are registered.
		/// </summary>
		public override void OnReceive(Context context, Intent intent)
		{
			base.OnReceive(context, intent);

			// Check if the click is from the "Announcement" button
			if (AnnouncementClick.Equals(intent.Action))
			{
				var pm = context.PackageManager;
				try
				{
					var packageName = "com.android.settings";
					var launchIntent = pm.GetLaunchIntentForPackage(packageName);
					context.StartActivity(launchIntent);
				}
				catch
				{
					// Something went wrong :)
				}
			}
		}
	}
}
