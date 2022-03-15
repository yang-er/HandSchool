using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using AndroidX.Core.Content;
using System;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [BindView(Resource.Layout.activity_login)]
    public class LoginActivity : BaseActivity
    {
        private LoginFragment Fragment { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetBackground();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            RunOnUiThread(SetBackground);
        }

        protected override void OnNavigatedParameter(object obj)
        {
            Fragment = obj as LoginFragment;
            TransactionV3(Fragment, Fragment);
            var actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetHomeButtonEnabled(true);
        }

        public override void Finish()
        {
            base.Finish();
            Fragment.Completed?.Start();
        }

        private static Drawable _backgroundImage;

        private static (int, int) _size;

        private void SetBackground()
        {
            var bounds = WindowManager?.CurrentWindowMetrics.Bounds;
            if (bounds is null) return;
            var windowHeight = bounds.Height();
            var windowWidth = bounds.Width();
            if (_size.Item1 != windowWidth || _size.Item2 != windowHeight)
            {
                var drawable = ContextCompat.GetDrawable(this, Resource.Drawable.bg_login)!;
                var resWidth = drawable.IntrinsicWidth;
                var resHeight = drawable.IntrinsicHeight;
                var widthScale = 1.0f * windowWidth / resWidth;
                var heightScale = 1.0f * windowHeight / resHeight;
                var maxScale = Math.Max(widthScale, heightScale);
                var bitmap = Bitmap.CreateBitmap((int)(resWidth * maxScale), (int)(resHeight * maxScale), Bitmap.Config.Argb8888!)!;
                var canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, (int)(resWidth * maxScale), (int)(resHeight * maxScale));
                drawable.Draw(canvas);
                var startXp = (bitmap.Width - windowWidth) / 2;
                var startYp = (bitmap.Height - windowHeight) / 2;
                bitmap = Bitmap.CreateBitmap(bitmap, startXp, startYp, windowWidth, windowHeight);
                _backgroundImage = new BitmapDrawable(Resources, bitmap);
                _size = (windowWidth, windowHeight);
            }

            Window?.SetBackgroundDrawable(_backgroundImage);
        }
    }
}