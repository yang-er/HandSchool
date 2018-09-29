using HandSchool.Models;
using System;
using System.Collections.ObjectModel;
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

        private FeedViewModel()
        {
            Title = "学校通知";
            Items = new ObservableCollection<FeedItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载新闻……");

            try
            {
                await Core.App.Feed.Execute();
            }
            catch (Exception ex)
            {
                Core.Log(ex);
            }
            finally
            {
                SetIsBusy(false);
            }
        }
    }
}
