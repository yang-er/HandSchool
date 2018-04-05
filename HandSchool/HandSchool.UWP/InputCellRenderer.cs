using HandSchool.UWP;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using WApplication = Windows.UI.Xaml.Application;
using WDataTemplate = Windows.UI.Xaml.DataTemplate;

[assembly: ExportRenderer(typeof(InputCell), typeof(InputCellRenderer))]
namespace HandSchool.UWP
{
    public class InputCellRenderer : EntryCellRenderer
    {
        public override WDataTemplate GetTemplate(Cell cell)
        {
            return (WDataTemplate)WApplication.Current.Resources["InputCell"];
        }
    }
}
