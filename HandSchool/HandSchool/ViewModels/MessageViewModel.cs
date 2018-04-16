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

        }

        async Task ExecuteDeleteAllCommand()
        {
            foreach (var item in Instance.Items)
            {
                await Core.App.Message.Delete(item.Id);
            }

            Instance.Items.Clear();
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                await Core.App.Message.Execute();
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
