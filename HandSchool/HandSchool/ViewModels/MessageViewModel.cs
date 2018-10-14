using HandSchool.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 站内消息的视图模型，提供了读取删除等功能。
    /// </summary>
    public class MessageViewModel : BaseViewModel
    {
        static MessageViewModel instance = null;

        /// <summary>
        /// 目前的所有站内消息
        /// </summary>
        public ObservableCollection<IMessageItem> Items { get; set; }

        /// <summary>
        /// 加载消息的命令
        /// </summary>
        public Command LoadItemsCommand { get; set; }

        /// <summary>
        /// 删除所有的命令
        /// </summary>
        public Command DeleteAllCommand { get; set; }

        /// <summary>
        /// 全部设置已读的命令
        /// </summary>
        public Command ReadAllCommand { get; set; }

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static MessageViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new MessageViewModel();
                return instance;
            }
        }

        /// <summary>
        /// 将视图模型的操作加载。
        /// </summary>
        public MessageViewModel()
        {
            Title = "站内消息";
            Items = new ObservableCollection<IMessageItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            DeleteAllCommand = new Command(async () => await ExecuteDeleteAllCommand());
            ReadAllCommand = new Command(async () => await ExecuteReadAllCommand());
        }

        /// <summary>
        /// 删除所有站内消息。
        /// </summary>
        private async Task ExecuteDeleteAllCommand()
        {
            foreach (var item in Instance.Items)
            {
                await Core.App.Message.Delete(item.Id);
            }

            Instance.Items.Clear();
        }

        /// <summary>
        /// 将所有站内消息设置为已读状态。
        /// </summary>
        private async Task ExecuteReadAllCommand()
        {
            foreach (var item in Instance.Items)
            {
                await Core.App.Message.SetReadState(item.Id, true);
            }
        }

        /// <summary>
        /// 加载所有的站内消息内容。
        /// </summary>
        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return; IsBusy = true;

            try
            {
                await Core.App.Message.Execute();
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
