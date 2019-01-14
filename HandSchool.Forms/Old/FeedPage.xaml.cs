﻿using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FeedPage : ViewPage
    {
        public bool IsPushing { get; set; } = false;

        public FeedPage()
        {
            InitializeComponent();
            ViewModel = FeedViewModel.Instance;
            On<_iOS_>().UseTabletMode();
        }

        FeedItem LastItem;

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || e.Item == LastItem || IsPushing) return;
            LastItem = e.Item as FeedItem;

            IsPushing = true;
            await Navigation.PushAsync(new MessageDetailPage(LastItem));
            IsPushing = false;

            if (Device.Idiom != TargetIdiom.Tablet) LastItem = null;
        }
    }
}