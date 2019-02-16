using HandSchool.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using ListView = Xamarin.Forms.ListView;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRenderer2))]
namespace HandSchool.iOS
{
    public class ListViewRenderer2 : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                e.NewElement.SelectionMode = ListViewSelectionMode.None;
                e.NewElement.OnThisPlatform().SetSeparatorStyle(SeparatorStyle.FullWidth);

                if (e.NewElement.Footer == null)
                {
                    e.NewElement.Footer = new StackLayout { HeightRequest = 1 };
                }

                e.NewElement.BackgroundColor = Color.Transparent;
            }
        }
    }
}