using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Views;
using Android.Widget;
using HandSchool.Views;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;
using System.ComponentModel;
using HandSchool.ViewModels;

namespace HandSchool.Droid
{
    internal class BaseActivity : AppCompatActivity, INavigate
    {
        private static readonly Dictionary<Guid, ViewObject>
            TransactionSource = new Dictionary<Guid, ViewObject>();

        private BaseViewModel _viewModel;
        private Guid? ViewObjectIdentity;



        public BaseViewModel ViewModel { get; set; }

        protected int ContentViewResource { get; set; }
        
        protected void Transaction(SupportFragment fragment)
        {
            RemoveViewObject();
            var transition = SupportFragmentManager.BeginTransaction();
            transition.Replace(Resource.Id.frame_layout, fragment);
            transition.Commit();
        }

        protected void Transaction(ViewObject viewPage)
        {
            var internalPage = new Xamarin.Forms.ContentPage
            {
                Content = viewPage.Content,
                BindingContext = viewPage.ViewModel,
            };

            viewPage.RegisterNavigation(this);
            viewPage.PropertyChanged += ViewPropChanged;
            
           

            Transaction(internalPage.CreateSupportFragment(this));
            var currentIdentity = Guid.NewGuid();
            TransactionSource.Add(currentIdentity, viewPage);
            ViewObjectIdentity = currentIdentity;
        }

        void ViewPropChanged(object sender, PropertyChangedEventArgs args)
        {
            var objectSender = (ViewModels.BaseViewModel)sender;

            switch (args.PropertyName)
            {
                case "Title":
                    SupportActionBar.Title = objectSender.Title;
                    break;

                case "IsBusy":
                    FindViewById<ProgressBar>(Resource.Id.main_progress_bar).Visibility = ViewStates.Visible;
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(ContentViewResource);
        }

        private void RemoveViewObject()
        {
            // If the activity is using a viewObject...
            if (ViewObjectIdentity.HasValue)
            {
                var vm = TransactionSource[ViewObjectIdentity.Value].ViewModel;
                vm.PropertyChanged -= ViewPropChanged;
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