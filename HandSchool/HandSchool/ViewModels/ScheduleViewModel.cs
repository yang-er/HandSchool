using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class ScheduleViewModel : BaseViewModel
    {
        public List<CurriculumItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        static ScheduleViewModel instance = null;

        public static ScheduleViewModel Instance
        {
            get
            {
                if (instance is null) instance = new ScheduleViewModel();
                return instance;
            }
        }

        public ScheduleViewModel()
        {
            Title = "课程表";
            Items = new List<CurriculumItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                await DataStore.GetItemsAsync(true);
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