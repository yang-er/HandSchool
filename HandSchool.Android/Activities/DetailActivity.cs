﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using HandSchool.ViewModels;
using System;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    [BindView(Resource.Layout.activity_detail)]
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

        public DetailViewModel ViewModel { get; set; }

        protected override void OnNavigatedParameter(object obj)
        {
            base.OnNavigatedParameter(obj);
            ViewModel = DetailViewModel.From(obj);

            var ActionBar = SupportActionBar;
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            TextContent.Text = ViewModel.Content;
            DetailTitle.Text = ViewModel.Name;
            DetailTime.Text = ViewModel.Date;
            DetailSender.Text = ViewModel.Sender;
            ActionBar.Title = ViewModel.Title;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var detail = menu.Add(ViewModel.Operation);
            detail.SetShowAsAction(ShowAsAction.Always);
            detail.SetOnMenuItemClickListener(new MenuEntryClickedListener(ViewModel.Command));
            return base.OnCreateOptionsMenu(menu);
        }
    }
}