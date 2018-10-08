using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.Models;

namespace HandSchool.Droid
{
    [BroadcastReceiver(Label = "课程表")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/classtablewidgetprovider")]
    class ClassTableWidget: AppWidgetProvider
    {
        public static readonly Color[] ClassColors = 
        {
            new Color(250,249,222),
            new Color(255, 242, 226),
            new Color(253, 230, 224),
            new Color(227, 237, 205),
            new Color(220, 226, 241),
            new Color(233, 235, 254),
            new Color(234, 234, 239),
            new Color(131,175,155),
            new Color(200,200,169),
            new Color(252,157,154)
        };

        public CurriculumItem[,] items = new CurriculumItem[7, 11];
        public List<CurriculumItem> item;
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

        private void RenderItems()
        {
            if (Core.App is null && !Core.Initialize()) return;
            if (Core.App.Schedule is null) return;

            Core.App.Schedule.RenderWeek(Core.App.Service.CurrentWeek, out var temp);
            //item = temp;
            for (int i = 0; i < 7; i++)
            {
                List<CurriculumItem> list = null;// = temp.FindAll((item) => item.WeekDay == i + 1);
                int LastEnd = 0;
                int Count = 0;

                foreach (var OneClass in list)
                {
                    //items[i, Count] = OneClass;
                    //Count++;
                    int Start = OneClass.DayBegin - 1; //4
                    int Period = OneClass.DayBegin - LastEnd - 1; //1
                    if (Period != 0)
                    {
                        items[i, Count] = new CurriculumItem();
                        items[i, Count].Name = "";
                        items[i, Count].DayBegin = LastEnd + 1;
                        items[i, Count].DayEnd = Start;
                        Count++;
                    }
                    items[i, Count] = OneClass;
                    Count++;
                    LastEnd = OneClass.DayEnd; //3
                }

                for (; Count < 11; Count++)
                {
                    items[i, Count] = null;
                }
            }
        }

        private RemoteViews UpdateWidgetListView(Context context, int appWidgetId)
        {
            RenderItems();
            RemoteViews ClassIndex = new RemoteViews(context.PackageName, Resource.Layout.classindex);
            RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.classtablewidget);
            
            remoteViews.RemoveAllViews(Resource.Id.ClassGrid);
            remoteViews.AddView(Resource.Id.ClassGrid, ClassIndex);

            for (int i = 0; i < 7; i++)
            {
                RemoteViews SingleLine = new RemoteViews(context.PackageName, Resource.Layout.SingleLine);
                int AlreadyFillBlanks = 0;

                for (int j = 0; j < 11; j++)
                {
                    if (items[i, j] == null)
                    {
                        if (AlreadyFillBlanks == 11)
                            continue;
                        int Period = 11 - AlreadyFillBlanks;

                        int LayoutId = (int)typeof(Resource.Layout).GetField("singleclassitem_" + Period.ToString()).GetRawConstantValue();
                        RemoteViews AddView = new RemoteViews(context.PackageName, LayoutId);
                        SingleLine.AddView(Resource.Id.singleline, AddView);
                        AlreadyFillBlanks += Period;
                    }
                    else if (items[i, j].Name == "")
                    {
                        int Period = items[i, j].DayEnd - items[i, j].DayBegin + 1;
                        int LayoutId = (int)typeof(Resource.Layout).GetField("singleclassitem_" + Period.ToString()).GetRawConstantValue();
                        AlreadyFillBlanks += Period;
                        RemoteViews AddView = new RemoteViews(context.PackageName, LayoutId);
                        SingleLine.AddView(Resource.Id.singleline, AddView);
                    }
                    else
                    {
                        int Period = items[i, j].DayEnd - items[i, j].DayBegin + 1;
                        int LayoutId = (int)typeof(Resource.Layout).GetField("singleclassitem_" + Period.ToString()).GetRawConstantValue();
                        int ViewId = (int)typeof(Resource.Id).GetField("class" + Period.ToString()).GetRawConstantValue();
                        RemoteViews AddView = new RemoteViews(context.PackageName, LayoutId);
                        AddView.SetTextViewText(ViewId, items[i, j].Name +"\n"+ items[i, j].Classroom);
                        Color color = ClassColors[items[i, j].Name[0] % 10];
                        color.A = 95;
                        AlreadyFillBlanks += Period;
                        AddView.SetInt(ViewId, "setBackgroundColor", color);
                        SingleLine.AddView(Resource.Id.singleline, AddView);
                    }
                }

                AlreadyFillBlanks = 0;
                remoteViews.AddView(Resource.Id.ClassGrid, SingleLine);
            }
                
            /*
            RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.classtablewidget);
            string PACKAGE_NAME = context.PackageName;
            Intent svcIntent = new Intent(context, typeof(ClassTableRemoteService));
            svcIntent.SetPackage(PACKAGE_NAME);
            svcIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
            svcIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToUri(Android.Content.IntentUriType.AndroidAppScheme)));
            remoteViews.SetRemoteAdapter(Resource.Id.ClassGrid, svcIntent);
            */

            return remoteViews;
        }
        
    }
}