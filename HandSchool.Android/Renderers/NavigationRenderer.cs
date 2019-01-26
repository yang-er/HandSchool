using Android.Content;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using HandSchool.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using AProgressBar = Android.Widget.ProgressBar;
using AToolbar = Android.Support.V7.Widget.Toolbar;
using AToolbarLayout = Android.Support.Design.Widget.AppBarLayout;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigateRenderer))]
namespace HandSchool.Droid
{
    public class NavigateRenderer : NavigationPageRenderer
    {
        const string sPageContainer = "Xamarin.Forms.Platform.Android.PageContainer";

        protected AToolbar Toolbar { get; set; }
        protected AToolbarLayout ToolbarLayout { get; set; }
        protected ViewGroup PageContainer { get; set; }
        protected AProgressBar ProgressBar { get; set; }
        bool AddFlag { get; set; }
        bool CurrentIsTab { get; set; }

        public void SetIsBusy(bool value)
        {
            if (ProgressBar != null)
                ProgressBar.Visibility = value ? ViewStates.Visible : ViewStates.Invisible;
        }

        public NavigateRenderer(Context context) : base(context)
        {
            ProgressBar = new AProgressBar(Context, null, Android.Resource.Attribute.ProgressBarStyleHorizontal)
            {
                Indeterminate = true,
                Visibility = ViewStates.Invisible,
                IndeterminateTintList = ColorStateList.ValueOf(Color.White.ToAndroid())
            };

            MessagingCenter.Subscribe<TabbedRenderer>(this, TabbedRenderer.TabbarUsed, TabbedPageCalling);
        }
        
        private void TabbedPageCalling(TabbedRenderer renderer)
        {
            if (Toolbar != null) CurrentIsTab = true;
        }

        public override void AddView(AView child, int index, LayoutParams @params)
        {
            if (index == -1 && AddFlag && child.GetType().ToString() == sPageContainer)
                index = ChildCount - 1;

            if (child is AToolbar toolbar)
            {
                toolbar.Elevation = PlatformImpl.Dip2Px(1);
            }

            if (CurrentIsTab)
            {
                Toolbar.Elevation = 0;
                CurrentIsTab = false;
            }

            base.AddView(child, index, @params);

            Core.Logger.WriteLine("Renderer", child.GetType().ToString() + " was added into NavPageRender");

            if (child is AToolbar)
            {
                Toolbar = child as AToolbar;
            }
            else if (child.GetType().ToString() == sPageContainer)
            {
                PageContainer = child as ViewGroup;
                var layoutParam = new LayoutParams(LayoutParams.MatchParent, 8);
                if (!AddFlag) AddView(ProgressBar, layoutParam);
            }
            else if (child is AProgressBar)
            {
                AddFlag = true;
            }
        }

        public override void RemoveView(AView view)
        {
            base.RemoveView(view);
            Core.Logger.WriteLine("Renderer", view.GetType().ToString() + " was removed from NavPageRender");

            if (view.GetType().ToString() == sPageContainer)
            {
                PageContainer = null;
            }
            else if (view is AProgressBar)
            {
                AddFlag = false;
            }
            else if (view is AToolbar)
            {
                
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (ProgressBar != null)
            {
                int height = PlatformImpl.Dip2Px(8);
                ProgressBar.BringToFront();
                ProgressBar.Layout(0, Toolbar.Height - height - 8, r, Toolbar.Height + height);
            }
        }
    }
}
