using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HandSchool.UWP.Renderers
{
    public class ListGroupStyleSelector : GroupStyleSelector
    {
        protected override GroupStyle SelectGroupStyleCore(object group, uint level)
        {
            return (GroupStyle)Application.Current.Resources["HSListViewGroupStyle"];
        }
    }
}