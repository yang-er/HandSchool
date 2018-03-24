using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        public ObservableCollection<IMessageItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        static MessageViewModel instance = null;
        
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
