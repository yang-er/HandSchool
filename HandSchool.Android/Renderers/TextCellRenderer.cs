using Android.Content;
using Android.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ACell = HandSchool.Views.TextCell;
using ARender = HandSchool.Droid.Renderers.TextCellRenderer;
using AView = Android.Views.View;

[assembly: ExportCell(typeof(ACell), typeof(ARender))]
namespace HandSchool.Droid.Renderers
{
    public class TextCellRenderer : ViewCellRenderer
    {
        protected override AView GetCellCore(Cell item,
            AView convertView, ViewGroup parent, Context context)
        {
            if (item is ACell cell && cell.PreferedCardView)
            {
                return base.GetCellCore(item, convertView, parent, context);
                throw new NotImplementedException();
            }
            else
            {
                return base.GetCellCore(item, convertView, parent, context);
            }
        }
    }
}