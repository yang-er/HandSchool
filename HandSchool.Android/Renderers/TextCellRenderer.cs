using Android.Content;
using Android.Views;
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
            var cell = item as ACell;

            switch (cell.PreferedCardView)
            {
                case 1:
                    return new TextCellView(context, Resource.Layout.cell_text1, cell);
                case 2:
                    return new TextCellView(context, Resource.Layout.cell_text2, cell);
                case 3:
                    return new TextCellView(context, Resource.Layout.cell_text3, cell);
                case 4:
                    return new TextCellView(context, Resource.Layout.cell_text4, cell);
                default:
                    return base.GetCellCore(item, convertView, parent, context);
            }
        }
    }
}