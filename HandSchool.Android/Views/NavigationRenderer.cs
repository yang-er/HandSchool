using Android.Content;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using AProgressBar = Android.Widget.ProgressBar;
using AToolbar = Android.Support.V7.Widget.Toolbar;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(HandSchool.Droid.NavigationRenderer))]
namespace HandSchool.Droid
{
    public class NavigationRenderer : NavigationPageRenderer
    {
        const string sPageContainer = "Xamarin.Forms.Platform.Android.PageContainer";

        protected AToolbar Toolbar { get; set; }
        protected ViewGroup PageContainer { get; set; }
        protected AProgressBar ProgressBar { get; set; }
        bool AddFlag { get; set; }

        public void SetIsBusy(bool value)
        {
            if (ProgressBar != null)
                ProgressBar.Visibility = value ? ViewStates.Visible : ViewStates.Invisible;
        }

        public NavigationRenderer(Context context) : base(context)
        {
            ProgressBar = new AProgressBar(Context, null, Android.Resource.Attribute.ProgressBarStyleHorizontal)
            {
                Indeterminate = true,
                Visibility = ViewStates.Invisible,
                IndeterminateTintList = ColorStateList.ValueOf(Color.White.ToAndroid())
            };
        }
        
        public override void AddView(AView child, int index, LayoutParams @params)
        {
            if (index == -1 && AddFlag && child.GetType().ToString() == sPageContainer)
                index = ChildCount - 1;
            base.AddView(child, index, @params);

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

            if (view.GetType().ToString() == sPageContainer)
            {
                PageContainer = null;
            }
            else if (view is AProgressBar)
            {
                AddFlag = false;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (ProgressBar != null)
            {
                int height = MainActivity.Dip2Px(8);
                ProgressBar.BringToFront();
                ProgressBar.Layout(0, Toolbar.Height - height, r, Toolbar.Height + height);
            }
        }
    }
}
