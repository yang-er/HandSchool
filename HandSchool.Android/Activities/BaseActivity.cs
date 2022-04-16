using Android.OS;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using HandSchool.Internals;
using System.ComponentModel;
using System.Threading.Tasks;
using SupportFragment  = AndroidX.Fragment.App.Fragment;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using AToolbar = AndroidX.AppCompat.Widget.Toolbar;
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
using Google.Android.Material.AppBar;
using Google.Android.Material.Tabs;
using Xamarin.Forms;
using ProgressBar = Android.Widget.ProgressBar;

namespace HandSchool.Droid
{
    public class BaseActivity : AppCompatActivity, INavigate, IBindTarget
    {
        public static string InternalFileRootPath { get; private set; }

        #region UI Elements

        /// <summary>
        /// 工具栏
        /// </summary>
        [BindView(Resource.Id.toolbar)]
        public AToolbar Toolbar { get; set; }

        /// <summary>
        /// 进度条
        /// </summary>
        [BindView(Resource.Id.main_progress_bar)]
        public ProgressBar ProgressBar { get; set; }

        /// <summary>
        /// 工具栏布局
        /// </summary>
        [BindView(Resource.Id.appbar_layout)]
        public AppBarLayout AppBarLayout { get; set; }

        /// <summary>
        /// 选项卡
        /// </summary>
        [BindView(Resource.Id.sliding_tabs)]
        public TabLayout Tabbar { get; set; }

        /// <summary>
        /// 布局所需要的资源
        /// </summary>
        protected int ContentViewResource { get; set; }

        #endregion
        
        public BaseActivity()
        {
            ContentViewResource = this.SolveSelf();
        }

        #region Fragment Transaction

        private ToolbarMenuTracker MenuTracker { get; set; }

        protected virtual void SetTransactionArguments(FragmentTransaction transition)
        {
            transition.SetCustomAnimations(Resource.Animation.slide_right_in,
                    Resource.Animation.slide_left_out,
                    Resource.Animation.slide_left_in,
                    Resource.Animation.slide_right_out);
        }

        protected void TransactionV3(SupportFragment fragment, IViewCore core)
        {
            if (ProgressBar != null)
            {
                ProgressBar.Visibility = ViewStates.Invisible;
            }
            ClearOldStates();

            if (fragment is TabbedFragment tabbed)
            {
                Tabbar.Visibility = ViewStates.Visible;
                tabbed.Tabbar = Tabbar;
            }

            var transition = SupportFragmentManager.BeginTransaction();
            SetTransactionArguments(transition);
            transition.Replace(Resource.Id.frame_layout, fragment);
            transition.Commit();

            if (Tabbar != null && !(fragment is TabbedFragment))
            {
                Tabbar.Visibility = ViewStates.Gone;
                Tabbar.SetupWithViewPager(null);
            }

            if (core != null)
            {
                SupportActionBar.Title = core.Title;
                MenuTracker = core.ToolbarTracker;
                if (MenuTracker != null)
                    MenuTracker.Changed += ReloadToolbarMenu;
            }

            if (core is IViewLifecycle pg)
            {
                pg.RegisterNavigation(this);
            }

            if (fragment is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += HandBind;
            }

            ReloadToolbarMenu(this, EventArgs.Empty); 
        }

        protected void Transaction(SupportFragment fragment)
        {
            TransactionV3(fragment, fragment as IViewCore);
        }
        
        protected void Transaction(ViewObject viewPage)
        {
            var fm = new EmbeddedFragment(viewPage);
            TransactionV3(fm, viewPage);
        }
        
        private void ReloadToolbarMenu(object sender, EventArgs args)
        {
            InvalidateOptionsMenu();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (MenuTracker != null)
            {
                foreach (var entry in MenuTracker.List)
                {
                    if (entry.HiddenForPull) continue;

                    var menuItem = menu.Add(entry.Title);
                    
                    if (entry.Order != ToolbarItemOrder.Secondary)
                        menuItem.SetShowAsAction(ShowAsAction.Always);
                    menuItem.SetOnMenuItemClickListener(new MenuEntryClickedListener(entry));
                }
            }

            return base.OnCreateOptionsMenu(menu);
        }

        private void HandBind(object sender, PropertyChangedEventArgs args)
        {
            RunOnUiThread(() =>
            {
                var busySignal = sender as IBusySignal;

                switch (args.PropertyName)
                {
                    case "IsBusy":
                        ProgressBar.Visibility = busySignal.IsBusy
                            ? ViewStates.Visible : ViewStates.Invisible;
                        break;
                }
            });
        }

        #endregion

        #region Activity Transaction

        const string BroadcastedArgument = "PARAMGUID";

        private static readonly Dictionary<Guid, object>
            ArgumentBroadcastSource = new Dictionary<Guid, object>();

        protected virtual void OnNavigatedParameter(object obj) { }
        
        public virtual Task PushAsync(Type pageType, object param)
        {
            pageType = Core.Reflection.TryGetImpl(pageType);

            if (typeof(AppCompatActivity).IsAssignableFrom(pageType))
            {
                // When the page type is aimed at an activity.
                var intent = new Intent(this, pageType);
                var guid = Guid.NewGuid();
                ArgumentBroadcastSource.Add(guid, param);
                intent.PutExtra(BroadcastedArgument, guid.ToByteArray());
                StartActivity(intent);
            }
            else
            {
                // When the page type is aimed at an fragment.
                var intent = new Intent(this, typeof(SecondActivity));
                var guid = Guid.NewGuid();
                var param2 = (pageType, param);
                ArgumentBroadcastSource.Add(guid, param2);
                intent.PutExtra(BroadcastedArgument, guid.ToByteArray());
                StartActivity(intent);
            }
            
            return Task.CompletedTask;
        }

        public Task PushAsync(object page, object param)
        {
            var intent = new Intent(this, typeof(SecondActivity));
            var guid = Guid.NewGuid();
            var param2 = (page, param);
            ArgumentBroadcastSource.Add(guid, param2);
            intent.PutExtra(BroadcastedArgument, guid.ToByteArray());
            StartActivity(intent);
            return Task.CompletedTask;
        }

        public Task<bool> PopAsync()
        {
            var topActivity = PlatformImplV2.Instance.PeekAliveActivity(false);
            if (topActivity is null) return Task.FromResult(false);
            PlatformImplV2.Instance.RemoveActivity(this);
            topActivity.Finish();
            return Task.FromResult(true);
        }
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(ContentViewResource);
            this.SolveView(this);
            InternalFileRootPath ??= DataDir?.AbsolutePath;

            SetSupportActionBar(Toolbar);
            Toolbar.SetNavigationOnClickListener(new ToolbarBackListener(this));
            PlatformImplV2.Instance.RegisterActivity(this);

            if (Intent.HasExtra(BroadcastedArgument))
            {
                // notice that this activity conveys an argument.
                var guid = new Guid(Intent.GetByteArrayExtra(BroadcastedArgument));
                var param = ArgumentBroadcastSource[guid];
                _navParam = (guid, param);
                ArgumentBroadcastSource.Remove(guid);
                OnNavigatedParameter(param);
            }
        }

        private ValueTuple<Guid, object>? _navParam;

        private void ClearOldStates()
        {
            if (MenuTracker != null)
            {
                MenuTracker.Changed -= ReloadToolbarMenu;
                MenuTracker = null;
            }

            foreach (var fg in SupportFragmentManager.Fragments)
            {
                if (fg is INotifyPropertyChanged propertyChanged)
                {
                    propertyChanged.PropertyChanged -= HandBind;
                }

                if (fg is TabbedFragment tabbed)
                {
                    tabbed.ClearReference();
                }
            }
        }

        protected override void OnDestroy()
        {
            PlatformImplV2.Instance.RemoveActivity(this);
            base.OnDestroy();
            ClearOldStates();
            if (_navParam is null) return;
            var para = _navParam.Value;
            _navParam = null;
            if (IsFinishing) return;
            ArgumentBroadcastSource.TryAdd(para.Item1, para.Item2);
        }

        public virtual void SolveBindings() { }
    }
}