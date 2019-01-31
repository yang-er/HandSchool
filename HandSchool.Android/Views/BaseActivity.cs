using Android.OS;
using Android.Support.V7.App;
using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using SupportFragment = Android.Support.V4.App.Fragment;
using AToolbar = Android.Support.V7.Widget.Toolbar;

namespace HandSchool.Droid
{
    public class BaseActivity : AppCompatActivity, INavigate
    {
        private static readonly Dictionary<Guid, ViewObject>
            TransactionSource = new Dictionary<Guid, ViewObject>();

        private BaseViewModel _viewModel;
        private Guid? ViewObjectIdentity;

        public AToolbar Toolbar { get; private set; }

        public BaseViewModel ViewModel { get; set; }

        protected int ContentViewResource { get; set; }
        
        protected void Transaction(SupportFragment fragment)
        {
            RemoveViewObject();

            if (fragment is IViewCore core)
            {
                SupportActionBar.Title = core.Title;
                ViewModel = core.ViewModel;
            }

            var transition = SupportFragmentManager.BeginTransaction();
            transition.Replace(Resource.Id.frame_layout, fragment);
            transition.Commit();
        }

        protected void Transaction(ViewFragment fragment)
        {
            fragment.RegisterNavigation(this);
            Transaction(fragment as SupportFragment);
        }

        protected void Transaction(ViewObject viewPage)
        {
            var internalPage = new Xamarin.Forms.ContentPage
            {
                Content = viewPage.Content,
                BindingContext = viewPage.ViewModel,
            };

            viewPage.RegisterNavigation(this);
            SupportActionBar.Title = viewPage.Title;
            //viewPage.PropertyChanged += ViewPropChanged;
            ViewModel = viewPage.ViewModel;

            Transaction(internalPage.CreateSupportFragment(this));
            var currentIdentity = Guid.NewGuid();
            TransactionSource.Add(currentIdentity, viewPage);
            ViewObjectIdentity = currentIdentity;
        }

        /*
        void ViewPropChanged(object sender, PropertyChangedEventArgs args)
        {
            var objectSender = (ViewObject)sender;

            switch (args.PropertyName)
            {
                case "Title":
                    SupportActionBar.Title = objectSender.Title;
                    break;
            }
        }
        */

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(ContentViewResource);
            Toolbar = FindViewById<AToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(Toolbar);
        }

        private void RemoveViewObject()
        {
            // If the activity is using a viewObject...
            if (ViewObjectIdentity.HasValue)
            {
                //var vm = TransactionSource[ViewObjectIdentity.Value].ViewModel;
                //vm.PropertyChanged -= ViewPropChanged;
                TransactionSource.Remove(ViewObjectIdentity.Value);
                ViewObjectIdentity = null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveViewObject();
        }
        
        Task INavigate.PushAsync(string pageType, object param)
        {
            throw new NotImplementedException();
        }

        Task INavigate.PushAsync(Type pageType, object param)
        {
            throw new NotImplementedException();
        }
    }
}