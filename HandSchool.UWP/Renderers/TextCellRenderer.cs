using Windows.UI.Xaml;
using Xamarin.Forms.Platform.UWP;
using XCell = Xamarin.Forms.Cell;
using XRenderer = HandSchool.UWP.Renderers.TextCellRenderer;
using XTextCell = Xamarin.Forms.TextCell;

[assembly: ExportCell(typeof(XTextCell), typeof(XRenderer))]
namespace HandSchool.UWP.Renderers
{
    public class TextCellRenderer : ICellRenderer
    {
        public DataTemplate GetTemplate(XCell cell)
        {
            return (DataTemplate)Application.Current.Resources["XamarinTextCell"];
        }
    }
}
