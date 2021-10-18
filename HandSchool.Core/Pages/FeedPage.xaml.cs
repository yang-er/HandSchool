using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Internals;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Reflection;

namespace HandSchool.Views
{
   
    public static class SelectionChangedEx
    {
        public static T GetItem<T>(this SelectionChangedEventArgs args)
        {
            if (args.CurrentSelection.Count == 0) return default;
            foreach (var i in args.CurrentSelection)
                return (T)i;
            return default;
        }
    }
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FeedPage : ViewObject
	{
        FeedItem LastItem = null;
        bool IsPushing = false;
        CollectionView CollectionView = null;

        public FeedPage()
		{
            InitializeComponent();
            ViewModel = FeedViewModel.Instance;
            CollectionView = collection_view;

        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = FeedViewModel.Instance;
            vm.IsBusy = !vm.IsBusy;
            vm.IsBusy = !vm.IsBusy;
            var now = DateTime.Now;
            
            if (vm.LastReload == null || (now - vm.LastReload.Value).TotalSeconds > 1800)
            {
                FeedViewModel.Instance.LoadItemsCommand.Execute(null);
            }
        }

        async void ItemTapped(object sender, EventArgs args)
        {
            var e = (sender as BindableObject)?.BindingContext as FeedItem;
            if (e is null || e == LastItem || IsPushing)
                return;
            IsPushing = true;
            LastItem = e;
            await Navigation.PushAsync<DetailPage>(LastItem);
            LastItem = null;
            IsPushing = false;
        }

        private async void CollectionView_LoadMore(object sender, EventArgs e)
        {
            if (FeedViewModel.Instance.LeftPage <= 0)
                return;
            if (FeedViewModel.Instance.IsBusy) return;

            if (FeedViewModel.Instance.WorkState.Item1 == FeedState.Search)
                await FeedViewModel.Instance.SearchWord(true, FeedViewModel.Instance.WorkState.Item2);
            else
                await FeedViewModel.Instance.LoadItems(true);
        }

    }
}