using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Internals;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FeedPage : ViewObject
	{
        bool _isPushing;

        public FeedPage()
		{
            InitializeComponent();
            ViewModel = FeedViewModel.Instance;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = FeedViewModel.Instance;
            vm.IsBusy = !vm.IsBusy;
            vm.IsBusy = !vm.IsBusy;
            
            if (vm.IsFeedOutOfTime)
            {
                FeedViewModel.Instance.LoadItemsCommand.Execute(null);
            }
        }

        async void ItemTapped(object sender, EventArgs args)
        {
            var e = (sender as BindableObject)?.BindingContext as FeedItem;
            if (e is null || _isPushing)
                return;
            _isPushing = true;
            await Navigation.PushAsync<DetailPage>(e);
            _isPushing = false;
        }

        private async void LoadMore(object sender, EventArgs e)
        {
            if (FeedViewModel.Instance.LeftPage <= 0)
                return;
            if (FeedViewModel.Instance.IsBusy) return;

            if (FeedViewModel.Instance.WorkState.Item1 == FeedMode.Search)
                await FeedViewModel.Instance.SearchWord(true, FeedViewModel.Instance.WorkState.Item2);
            else
                await FeedViewModel.Instance.LoadItems(true);
        }
    }
}