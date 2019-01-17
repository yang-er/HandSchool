using HandSchool.Models;
using HandSchool.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HandSchool.Views
{
    // Thanks to arcsinW
    public sealed partial class InfoQueryPageF : ViewPage
    {
        public ObservableCollection<InfoEntranceGroup> DataSource => Core.App.InfoEntrances;

        public InfoQueryPageF()
        {
            InitializeComponent();
            ViewModel = new BaseViewModel { Title = "信息查询" };
            CollectionViewSource.Source = DataSource;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is InfoEntranceWrapper wrapper)
            {
                Frame.Navigate(typeof(WebViewPage), wrapper.Load.Invoke());
            }
        }
    }
}
