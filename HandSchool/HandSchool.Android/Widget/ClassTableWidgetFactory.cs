using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.Models;

namespace HandSchool.Droid
{
    class ClassTableWidgetFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        //Colors From https://blog.csdn.net/zhang_hui_cs/article/details/7459414
        public static readonly Color[] ClassColors = {
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

        static string WeekDayName = "一二三四五六七";
        public int Count => 77;

        public bool HasStableIds => true;

        public RemoteViews LoadingView => null;

        public int ViewTypeCount =>1;

        public Context MyContext;
        public CurriculumItem[,] items;
        public List<CurriculumItem> item;
        public ClassTableWidgetFactory(Context context)
        {
            items = new CurriculumItem[7,11];
            
            MyContext = context;
            Core.App.Schedule.RenderWeek(Core.App.Service.CurrentWeek, out var temp);
            item = temp;
            for(int i=0;i<7;i++)
            {
                List<CurriculumItem> list = temp.FindAll((item) => item.WeekDay == i + 1);
                foreach(var OneClass in list)
                {
                    for(int j=OneClass.DayBegin-1;j<=OneClass.DayEnd-1;j++)
                    {
                        items[i,j]= OneClass;
                    }
                }
            }
            
        }
        public long GetItemId(int position)
        {
            return position;
        }

        public RemoteViews GetViewAt(int position)
        {
            TextView b = new TextView(MyContext);
            RemoteViews Remote;

            b.SetBackgroundResource(Resource.Color.ivory);
            /*
            if (position < 7)
            {
                Remote= new RemoteViews(MyContext.PackageName, Resource.Layout.classnumitem);
                Remote.SetTextViewText(Resource.Id.ClassNum, "周" + WeekDayName[position]);
                Color color = Color.Blue;
                color.A = 60;
                Remote.SetInt(Resource.Id.ClassNum, "setBackgroundColor", color);
                return Remote;
            }
            position -= 7;
            */
            Remote = new RemoteViews(MyContext.PackageName, Resource.Layout.classtableitem);
            Remote = new RemoteViews(MyContext.PackageName, Resource.Id.ClassGrid);
            //FrameLayout frame = new FrameLayout(MyContext);
            if (items[position %7, position/7]!=null)
            {
                Remote.SetTextViewText(Resource.Id.ClassTableText, items[position % 7, position / 7].Name);
                Color color = ClassColors[items[position % 7, position / 7].Name[0] % 10];
                color.A = 95;
                Remote.SetInt(Resource.Id.ClassTableText, "setBackgroundColor", color);
            }
            else
            {
                
                Remote.SetTextViewText(Resource.Id.ClassTableText,"    ");
                Color color = Android.Graphics.Color.White;
                color.A = 30;
                TextView a = new TextView(MainActivity.ActivityContext);
                Remote.SetInt(Resource.Id.ClassTableText, "setBackgroundColor", color);
                if(position==1)
                    Remote.SetInt(Resource.Id.ClassTableText, "setHeight", 10);

            }
                
            return Remote;
        }

        public void OnCreate()
        {
            return;
        }

        public void OnDataSetChanged()
        {
            return;
        }

        public void OnDestroy()
        {
            item.Clear();
            return;
        }
    }
}