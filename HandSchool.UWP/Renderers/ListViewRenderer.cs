using HandSchool.UWP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using WSelectionMode = Windows.UI.Xaml.Controls.ListViewSelectionMode;
using WStyle = Windows.UI.Xaml.Style;
using WApp = Windows.UI.Xaml.Application;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRender))]
namespace HandSchool.UWP.Renderers
{
    public class ListViewRender : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            var old = List;
            base.OnElementChanged(e);
            
            if (List != old)
            {
                List.GroupStyleSelector = (ListGroupStyleSelector)
                    WApp.Current.Resources["HSListViewGroupStyleSelector"];
                List.ItemContainerStyle = (WStyle)
                    WApp.Current.Resources["HSListViewItem"];
                List.SelectionMode = WSelectionMode.None;
            }
        }
    }
}