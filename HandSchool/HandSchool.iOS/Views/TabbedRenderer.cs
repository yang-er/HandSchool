using System.ComponentModel;
using HandSchool.iOS;
using HandSchool.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace HandSchool.iOS
{
    class MainPageRenderer : TabbedRenderer
    {
        internal MainPage MainPage => Element as MainPage;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            HidesBottomBarWhenPushed = true;

            if (e.NewElement is MainPage pg)
                if (pg.IsSelectPage)
                    TabBar.Hidden = true;
            TabBar.Translucent = true;

            if (e.NewElement != null)
                e.NewElement.PropertyChanged += SelectPageChanged;
            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= SelectPageChanged;
        }

        private void SelectPageChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsSelectPage")
            {
                TabBar.Hidden = MainPage.IsSelectPage;
            }
        }
    }
}