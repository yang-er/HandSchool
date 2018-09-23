using HandSchool.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    public class GradePointViewModel : BaseViewModel
    {
        public ObservableCollection<IGradeItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        static GradePointViewModel instance = null;

        public static GradePointViewModel Instance
        {
            get
            {
                if (instance is null) instance = new GradePointViewModel();
                return instance;
            }
        }

        public GradePointViewModel()
        {
            Title = "学分成绩";
            Items = new ObservableCollection<IGradeItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        public async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Core.App.GradePoint.Execute();
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