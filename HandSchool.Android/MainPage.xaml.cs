using HandSchool.Droid;
using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
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
            PrimaryItems = PlatformImpl.Instance.NavigationItems;
            SecondaryItems = new List<NavMenuItemImpl>();
            Master.BindingContext = IndexViewModel.Instance;

            if (!Core.Initialized)
            {
                Detail = new SelectTypePage();
            }
            else
            {
                Detail = GuessCurrentPage();
                SetOutline();
            }
        }

        private List<NavMenuItemImpl> PrimaryItems { get; }
        private List<NavMenuItemImpl> SecondaryItems { get; }

        private void WSizeChanged(object sender, EventArgs e)
        {
            var master = sender as VisualElement;
            infoBar.HeightRequest = master.Width * 0.625;
            stackOfInfo.Margin = new Thickness(20, master.Width * 0.625 - 70, 0, 0);
        }
        
        private void SetOutline()
        {
            SecondaryItems.Add(new NavMenuItemImpl("设置", "SettingPage", ""));
            PrimaryListView.ItemsSource = PrimaryItems;
            SecondaryListView.ItemsSource = SecondaryItems;
            SecondaryListView.HeightRequest = 12 + 48 * SecondaryItems.Count;

            PrimaryListView.ItemSelected += MasterPageItemSelected;
            SecondaryListView.ItemSelected += MasterPageItemSelected;
        }

        public void FinishSettings()
        {
            SetOutline();
            Detail = PrimaryItems[0].OutsidePage;

            Core.App.Service.RequestLogin().ContinueWith(LoginSuccessContinue);
        }

        private void LoginSuccessContinue(Task<bool> success)
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
        }

        private void ScheduleComplete()
        {
            GradePointViewModel.Instance.LoadItemsCommand.Execute(null);
            ScheduleViewModel.Instance.RefreshComplete -= ScheduleComplete;
        }

        private async void MasterPageItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is NavMenuItemImpl item)
            {
                Detail = item.OutsidePage;
                
                (sender as ListView).SelectedItem = null;

                PrimaryItems.ForEach((one) => one.Selected = false);
                SecondaryItems.ForEach((one) => one.Selected = false);

                item.Selected = true;

                // Funny fucky question: why this makes fluency?
                await Task.Delay(200);
                IsPresented = false;
            }
        }

        public NavigationPage GuessCurrentPage()
        {
            var nav_item = PrimaryItems.Find((one) => one.Selected);
            nav_item = nav_item ?? SecondaryItems.Find((one) => one.Selected);
            nav_item = nav_item ?? PrimaryItems[0];
            return nav_item.OutsidePage;
        }
    }
}