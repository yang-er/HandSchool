using HandSchool.UWP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using SelectionMode = Windows.UI.Xaml.Controls.ListViewSelectionMode;
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
                List.SelectionMode = SelectionMode.None;
            }
        }

    }
}