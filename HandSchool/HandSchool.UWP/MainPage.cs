using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    public class MainPage : MasterDetailPage
    {
        public OutlinePage Outline = new OutlinePage();

        public MainPage()
        {
            Master = Outline;
            MasterBehavior = MasterBehavior.Default;
            Outline.PrimaryListView.ItemTapped += MasterPageItemTapped;
            Outline.SecondaryListView.ItemTapped += MasterPageItemTapped;
            Detail = NavigationViewModel.Instance.PrimaryItems[0].DestPage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.Current.Service.NeedLogin && !App.Current.Service.IsLogin)
            {
                LoginViewModel.RequestAsync(App.Current.Service);
            }
        }

        private void MasterPageItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is MasterPageItem item)
            {
                Detail = item.DestPage;

                (sender as ListView).SelectedItem = null;

                NavigationViewModel.Instance.PrimaryItems.ForEach((one) => one.Selected = false);
                NavigationViewModel.Instance.SecondaryItems.ForEach((one) => one.Selected = false);

                item.Selected = true;
            }
        }
    }
}