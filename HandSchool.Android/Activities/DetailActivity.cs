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
        [BindView(Resource.Id.detail_text_view)]
        public TextView TextContent { get; set; }

        [BindView(Resource.Id.detail_title)]
        public TextView DetailTitle { get; set; }

        [BindView(Resource.Id.detail_time)]
        public TextView DetailTime { get; set; }

        [BindView(Resource.Id.detail_sender)]
        public TextView DetailSender { get; set; }

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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var detail = menu.Add(ViewModel.Operation);
            detail.SetShowAsAction(ShowAsAction.Always);
            detail.SetOnMenuItemClickListener(new MenuEntryClickedListener(ViewModel.Command));
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ContentViewResource = Resource.Layout.activity_detail;
            base.OnCreate(savedInstanceState);

            TextContent.Text = ViewModel.Content;
            DetailTitle.Text = ViewModel.Name;
            DetailTime.Text = ViewModel.Date;
            DetailSender.Text = ViewModel.Sender;

            var ActionBar = SupportActionBar;
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.Title = ViewModel.Title;
        }
    }
}