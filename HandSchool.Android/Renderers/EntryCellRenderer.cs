using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ARender = HandSchool.Droid.EntryCellRenderer;
using AView = Android.Views.View;
using XRender = Xamarin.Forms.Platform.Android.EntryCellRenderer;
using XView = Xamarin.Forms.Platform.Android.EntryCellView;

[assembly: ExportRenderer(typeof(EntryCell), typeof(ARender))]
namespace HandSchool.Droid
{
    public class EntryCellRenderer : XRender
    {
        protected override AView GetCellCore(Cell item, AView convertView, ViewGroup parent, Context context)
        {
            var obj = base.GetCellCore(item, convertView, parent, context) as XView;
            var label = obj.GetChildAt(0) as TextView;
            label.SetWidth((int)context.ToPixels(80));
            return obj;
        }
    }
}
