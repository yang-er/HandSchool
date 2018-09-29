using HandSchool.Internal;
using HandSchool.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        public ObservableCollection<IMessageItem> Items { get; set; }
        static MessageViewModel instance = null;
        public Command LoadItemsCommand { get; set; }
        public Command DeleteAllCommand { get; set; }
        public Command ReadAllCommand { get; set; }

        public static MessageViewModel Instance
        {
            get
            {
                if (instance is null) instance = new MessageViewModel();
                return instance;
            }
        }

        public MessageViewModel()
        {
            Title = "站内消息";
            Items = new ObservableCollection<IMessageItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            DeleteAllCommand = new Command(async () => await ExecuteDeleteAllCommand());
            ReadAllCommand = new Command(async () => await ExecuteReadAllCommand());
        }

        async Task ExecuteDeleteAllCommand()
        {
            foreach (var item in Instance.Items)
            {
                await Core.App.Message.Delete(item.Id);
            }

            Instance.Items.Clear();
        }

        async Task ExecuteReadAllCommand()
        {
            foreach (var item in Instance.Items)
            {
                await Core.App.Message.SetReadState(item.Id, true);
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return;
            SetIsBusy(true, "正在加载消息……");

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
                SetIsBusy(false);
            }
        }
    }
}
