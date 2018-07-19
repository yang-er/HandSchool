using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using HandSchool.ViewModels;
using Java.Lang;
using Java.Util;

namespace HandSchool.Droid
{
    [Service]
    class SampleService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            System.Diagnostics.Debug.WriteLine("SampleService->OnCreat()");
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            //获取NotificationManager实例
            //实例化NotificationCompat.Builde并设置相关属性
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                    //设置小图标
                    .SetSmallIcon(Resource.Drawable.abc_ab_share_pack_mtrl_alpha)
                    //设置通知标题
                    .SetContentTitle("最简单的Notification")
                    //设置通知内容
                    .SetContentText("只有小图标、标题、内容");
           //设置通知时间，默认为系统发出通知的时间，通常不用设置
           //.setWhen(System.currentTimeMillis());
           //通过builder.build()方法生成Notification对象,并发送通知,id=1
            StartForeground(startId,builder.Build());
            System.Diagnostics.Debug.WriteLine("SampleService->OnStart()");
            Timer timer = new Timer();
            TimerTask task = new MyTask();
            timer.Schedule(task, 0, 10);
            return StartCommandResult.Sticky;
        }
        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine("SampleService->OnDestroy()");

            base.OnDestroy();
        }
    }
    class MyTask : TimerTask
    {
        public override void Run()
        {
            AppWidgetManager manager = AppWidgetManager.GetInstance(MainActivity.ActivityContext);
            ComponentName componentName = new ComponentName(MainActivity.ActivityContext, Class.FromType(typeof(AppWidget)));
            var View = new RemoteViews(MainActivity.ActivityContext.PackageName,Resource.Layout.Widget);
            View.SetTextViewText(Resource.Id.widgetSmall, string.Format("{0:H:mm:ss}", DateTime.Now));
            View.SetTextViewText(Resource.Id.widgetMedium,"下节课:" +IndexViewModel.Instance.NextClass);
            View.SetTextViewText(Resource.Id.NextClassPlaceString, "上课地点:" + IndexViewModel.Instance.NextClassroom);
            manager.UpdateAppWidget(componentName, View);
        }
    }
}