using Android.Content;
using Android.Views;
using Android.Widget;
using HandSchool.Internals;
using System.ComponentModel;
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
            if (item is ACell cell && cell.PreferedCardView == 1)
            {
                return new TextCellView(context, Resource.Layout.cell_text1, cell);
            }
            else
            {
                return base.GetCellCore(item, convertView, parent, context);
            }
        }
    }
}