using Android.OS;
using Android.Support.V7.App;
using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using HandSchool.Internals;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using SupportFragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using AToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Support.Design.Widget;
using System.Collections.ObjectModel;

namespace HandSchool.Droid
{
    public class BaseActivity : AppCompatActivity, INavigate, IBindTarget
    {
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
            transition.SetTransition(0x00001001);
        }

        protected void TransactionV3(SupportFragment fragment, IViewCore core)
        {
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
                    
                    if (entry.Order != Xamarin.Forms.ToolbarItemOrder.Secondary)
                        menuItem.SetShowAsAction(ShowAsAction.Always);
                    menuItem.SetOnMenuItemClickListener(new MenuEntryClickedListener(entry));
                }
            }

            return base.OnCreateOptionsMenu(menu);
        }

        private void HandBind(object sender, PropertyChangedEventArgs args)
        {
            var busySignal = sender as IBusySignal;
            
            switch (args.PropertyName)
            {
                case "IsBusy":
                    ProgressBar.Visibility = busySignal.IsBusy
                        ? ViewStates.Visible : ViewStates.Invisible;
                    break;
            }
        }

        #endregion

        #region Activity Transaction

        const string BroadcastedArgument = "PARAMGUID";

        private static readonly Dictionary<Guid, object>
            ArgumentBroadcastSource = new Dictionary<Guid, object>();

        protected virtual void OnNavigatedParameter(object obj) { }
        
        public virtual Task PushAsync(Type pageType, object param)
        {
            pageType = Core.Reflection.TryGetType(pageType);

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

        #endregion
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(ContentViewResource);
            this.SolveView(this);
            
            SetSupportActionBar(Toolbar);
            Toolbar.SetNavigationOnClickListener(new ToolbarBackListener(this));
            PlatformImplV2.Instance.SetContext(this);

            if (Intent.HasExtra(BroadcastedArgument))
            {
                // notice that this activity conveys an argument.
                var guid = new Guid(Intent.GetByteArrayExtra(BroadcastedArgument));
                OnNavigatedParameter(ArgumentBroadcastSource[guid]);
            }
        }

        private void ClearOldStates()
        {
            if (MenuTracker != null)
            {
                MenuTracker.Changed -= ReloadToolbarMenu;
                MenuTracker = null;
            }

            foreach (var fg in SupportFragmentManager.Fragments)
            {
                if (fg is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged -= HandBind;
                }

                if (fg is TabbedFragment tabbed)
                {
                    Tabbar.RemoveOnTabSelectedListener(tabbed.Adapter);
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearOldStates();
            PlatformImplV2.Instance.RemoveContext(this);
        }

        public virtual void SolveBindings() { }
    }
}