using HandSchool.Internal;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class FeedViewModel : BaseViewModel
    {
        public ObservableCollection<FeedItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        static FeedViewModel instance = null;

        public static FeedViewModel Instance
        {
            get
            {
                if (instance is null) instance = new FeedViewModel();
                return instance;
            }
        }

        public FeedViewModel()
        {
            Title = "站内消息";
            Items = new ObservableCollection<FeedItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await App.Current.Message.Execute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
