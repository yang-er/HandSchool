using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.ViewModels;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class DetailActivity : BaseActivity
    {
        public TextView TextContent { get; set; }

        public new DetailViewModel ViewModel
        {
            get => base.ViewModel as DetailViewModel;
            set => base.ViewModel = value;
        }

        protected override void OnNavigatedParameter(object obj)
        {
            base.OnNavigatedParameter(obj);
            ViewModel = DetailViewModel.From(obj);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ContentViewResource = Resource.Layout.activity_detail;
            base.OnCreate(savedInstanceState);
            TextContent = FindViewById<TextView>(Resource.Id.detail_text_view);
            Toolbar.Title = ViewModel.Title;
            Toolbar.Subtitle = "666";
            TextContent.Text = ViewModel.Content;
            // Create your application here
        }
    }
}