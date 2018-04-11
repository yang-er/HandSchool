using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OutlinePage : ContentPage
	{
        public OutlinePage()
		{
			InitializeComponent();
            PrimaryListView.ItemsSource = NavigationViewModel.Instance.PrimaryItems;
            SecondaryListView.ItemsSource = NavigationViewModel.Instance.SecondaryItems;
            SecondaryListView.HeightRequest = 12 + 48 * NavigationViewModel.Instance.SecondaryItems.Count;
        }
    }
}