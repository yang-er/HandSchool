using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
    {
		public MainPage()
		{
			InitializeComponent();
            Core.Log("New MainPage.");
            if (!Core.Initialized)
            {
                Detail = new SelectTypePage();
            }
            else
            {
                Detail = NavigationViewModel.Instance.GuessCurrentPage();
                SetOutline();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Core.Log("MainPage appearing...");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Core.Log("MainPage disappearing...");
        }

        private void SetOutline()
        {
            Outline.PrimaryListView.ItemsSource = NavigationViewModel.Instance.PrimaryItems;
            Outline.SecondaryListView.ItemsSource = NavigationViewModel.Instance.SecondaryItems;
            Outline.SecondaryListView.HeightRequest = 12 + 48 * NavigationViewModel.Instance.SecondaryItems.Count;

            Outline.PrimaryListView.ItemSelected += MasterPageItemSelected;
            Outline.SecondaryListView.ItemSelected += MasterPageItemSelected;
        }

        public void FinishSettings()
        {
            SetOutline();
            Detail = NavigationViewModel.Instance.PrimaryItems[0].DestPage;
            Core.App.Service.RequestLogin().ContinueWith((success) =>
            {
                if (success.Result)
                {
                    if (Core.App.Schedule != null)
                    {
                        ScheduleViewModel.Instance.RefreshCommand.Execute(null);
                        if (Core.App.GradePoint != null)
                        {
                            ScheduleViewModel.Instance.RefreshComplete += ScheduleComplete;
                        }
                    }
                    else if (Core.App.GradePoint != null)
                    {
                        GradePointViewModel.Instance.LoadItemsCommand.Execute(null);
                    }
                }
            });
        }

        private void ScheduleComplete()
        {
            GradePointViewModel.Instance.LoadItemsCommand.Execute(null);
            ScheduleViewModel.Instance.RefreshComplete -= ScheduleComplete;
        }

        private async void MasterPageItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MasterPageItem item)
            {
                Detail = item.DestPage;
                
                (sender as ListView).SelectedItem = null;

                NavigationViewModel.Instance.PrimaryItems.ForEach((one) => one.Selected = false);
                NavigationViewModel.Instance.SecondaryItems.ForEach((one) => one.Selected = false);

                item.Selected = true;

                // Funny fucky question: why this makes fluency?
                await Task.Delay(200);
                IsPresented = false;
            }
        }
    }
}