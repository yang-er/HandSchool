﻿using Android.Content;
using HandSchool.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRenderer2))]
namespace HandSchool.Droid.Renderers
{
    public class ListViewRenderer2 : ListViewRenderer
    {
        public ListViewRenderer2(Context context) : base(context) {}
        
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            
            if (e.NewElement != null)
            {
                e.NewElement.SelectionMode = ListViewSelectionMode.None;

                if (e.NewElement.SeparatorVisibility == SeparatorVisibility.None)
                {
                    // Only items that use CardView would reach here
                    Control.SetSelector(Android.Resource.Color.Transparent);
                }
            }
        }
    }
}