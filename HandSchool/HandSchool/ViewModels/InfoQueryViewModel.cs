using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using HandSchool.Models;
using HandSchool.Views;

namespace HandSchool.ViewModels
{
    public class InfoQueryViewModel : BaseViewModel
    {
        public ObservableCollection<IInfoEntrance> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        static InfoQueryViewModel instance = null;

        public static InfoQueryViewModel Instance
        {
            get
            {
                if (instance is null) instance = new InfoQueryViewModel();
                return instance;
            }
        }

        public InfoQueryViewModel()
        {
            Title = "信息查询";
            Items = new ObservableCollection<IInfoEntrance>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                throw new NotImplementedException();
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