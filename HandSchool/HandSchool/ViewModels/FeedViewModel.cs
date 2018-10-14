using HandSchool.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 学校通知的视图模型，提供了刷新和数据源的功能。
    /// </summary>
    public class FeedViewModel : BaseViewModel
    {
        static FeedViewModel instance = null;

        /// <summary>
        /// 消息内容列表
        /// </summary>
        public ObservableCollection<FeedItem> Items { get; set; }

        /// <summary>
        /// 加载消息的命令
        /// </summary>
        public Command LoadItemsCommand { get; set; }

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static FeedViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new FeedViewModel();
                return instance;
            }
        }

        /// <summary>
        /// 将学校通知的数据源和刷新操作组织起来。
        /// </summary>
        private FeedViewModel()
        {
            Title = "学校通知";
            Items = new ObservableCollection<FeedItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        /// <summary>
        /// 加载消息的方法。
        /// </summary>
        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return; IsBusy = true;

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
                IsBusy = false;
            }
        }
    }
}
