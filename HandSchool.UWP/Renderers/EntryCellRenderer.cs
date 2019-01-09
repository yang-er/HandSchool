using Windows.System;
using Windows.UI.Xaml.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using WApplication = Windows.UI.Xaml.Application;
using WDataTemplate = Windows.UI.Xaml.DataTemplate;
using XRenderer = HandSchool.UWP.Renderers.EntryCellRenderer;

[assembly: ExportCell(typeof(EntryCell), typeof(XRenderer))]
namespace HandSchool.UWP.Renderers
{
    public class EntryCellRenderer : ICellRenderer
    {
        public static BindableProperty IsPasswordProperty =
            BindableProperty.CreateAttached
            (
                propertyName: "IsPassword",
                returnType: typeof(bool),
                declaringType: typeof(EntryCell),
                defaultValue: false
            );

        public WDataTemplate GetTemplate(Cell cell)
        {
            return (WDataTemplate)WApplication.Current.Resources["XamarinEntryCell"];
        }
    }

    public class EntryCellTextBox : FormsTextBox
    {
        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (DataContext is IEntryCellController cell)
                {
                    cell.SendCompleted();
                    e.Handled = true;
                }
            }

            base.OnKeyUp(e);
        }
    }
}
