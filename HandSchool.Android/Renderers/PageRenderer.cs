using Android.Content;
using HandSchool.Droid;
using HandSchool.Internal;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ViewPage), typeof(ViewPageRenderer))]
namespace HandSchool.Droid
{
    public class ViewPageRenderer : PageRenderer
    {
        public ViewPageRenderer(Context context) : base(context) { }

        public new ViewPage Element => base.Element as ViewPage;
        
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);

            if (e.NewElement is ViewPage pg)
            {
                if ((bool)pg.GetValue(PlatformExtensions.ShowLeftCancelProperty))
                {
                    pg.ToolbarItems.Add(new ToolbarItem("取消", null, async () => await pg.CloseAsync()));
                }

                if ((bool)pg.GetValue(PlatformExtensions.ShowLoadingProperty))
                {
                    MessagingCenter.Subscribe<Page, bool>(this, Page.BusySetSignalName, SetIsBusy, pg);
                }
            }
        }
        
        private static void SetIsBusy(Page page, bool isBusy)
        {
            if (page.Parent is NavigationPage navpg)
            {
                (Platform.GetRenderer(navpg) as NavigateRenderer).SetIsBusy(isBusy);
            }
            else if (page.Parent is TabbedPage tabpg && tabpg.Parent is NavigationPage navpage)
            {
                (Platform.GetRenderer(navpage) as NavigateRenderer).SetIsBusy(isBusy);
            }
        }
    }
}
