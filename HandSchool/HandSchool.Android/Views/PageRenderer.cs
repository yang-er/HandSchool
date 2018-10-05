using Android.Content;
using HandSchool.Droid;
using HandSchool.Views;
using System.ComponentModel;
using Xamarin.Forms;
using ElementChangedEventArgs = Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Page>;
using XPageRenderer = Xamarin.Forms.Platform.Android.PageRenderer;
using XPlatform = Xamarin.Forms.Platform.Android.Platform;

[assembly: ExportRenderer(typeof(PopContentPage), typeof(PageRenderer))]
namespace HandSchool.Droid
{
    class PageRenderer : XPageRenderer
    {
        public PageRenderer(Context context) : base(context) { }

        public new PopContentPage Element => base.Element as PopContentPage;
        
        protected override void OnElementChanged(ElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            
            if (e.NewElement is PopContentPage pg)
            {
                if (pg.ShowCancel)
                {
                    pg.ToolbarItems.Add(new ToolbarItem("取消", null, async () => await pg.CloseAsync()));
                }
            }
        }
        
        private void SetIsBusy()
        {
            if (Element.ShowIsBusyDialog && Element.Parent is NavigationPage navpg)
            {
                (XPlatform.GetRenderer(navpg) as NavigationRenderer).SetIsBusy(Element.IsBusy);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Page.IsBusyProperty.PropertyName)
                SetIsBusy();
        }
    }
}
