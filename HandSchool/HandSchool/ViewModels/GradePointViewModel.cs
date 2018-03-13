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

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                await App.Current.GradePoint.Execute();
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