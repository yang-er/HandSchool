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
        public Command LoadItemsCommand { get; set; }
        static MessageViewModel instance = null;
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

            Core.App.Confrimed = false;
            await new CheckBehavior("确认删除全部?").CheckAsync();
            if (Core.App.Confrimed == false)
            {
                return;
            }
            foreach (var a in Instance.Items.ToList())
            {
                await Core.App.Message.Delete(a.Id);
                Instance.Items.Remove(a);
            }
        }
        async Task ExecuteLoadItemsCommand()
        {


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
