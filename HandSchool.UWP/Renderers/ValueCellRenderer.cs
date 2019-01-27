using HandSchool.UWP.Renderers;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using WApplication = Windows.UI.Xaml.Application;
using WDataTemplate = Windows.UI.Xaml.DataTemplate;

[assembly: ExportCell(typeof(ValueCell), typeof(ValueCellRenderer))]
namespace HandSchool.UWP.Renderers
{
    public class ValueCellRenderer : ICellRenderer
    {
        public WDataTemplate GetTemplate(Cell cell)
        {
            return (WDataTemplate)WApplication.Current.Resources["HSValueCell"];
        }
    }
}