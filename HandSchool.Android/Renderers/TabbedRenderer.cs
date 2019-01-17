using Android.Content;
using Android.Support.Design.Widget;
using HandSchool.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedRenderer))]
namespace HandSchool.Droid
{
    public class TabbedRenderer : TabbedPageRenderer
    {
        public const string TabbarUsed = "HandSchool.Droid.TabbarUsed";

        public TabbedRenderer(Context context) : base(context) { }

        public override void AddView(AView child, int index, LayoutParams @params)
        {
            base.AddView(child, index, @params);

            if (child is TabLayout tabLayout)
            {
                tabLayout.Elevation = PlatformImpl.Dip2Px(1);
                MessagingCenter.Send(this, TabbarUsed);
            }
        }
    }
}