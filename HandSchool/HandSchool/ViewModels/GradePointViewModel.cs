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
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<string> ExcuteLoadGPACommand()
        {
            if (IsBusy)
                return "正在加载中，请稍后。";

            View.SetIsBusy(true, "正在加载GPA信息……");
            IsBusy = true;

            try
            {
                return await Core.App.GradePoint.GatherGPA();
            }
            catch (Exception ex)
            {
                return "发生异常。" + ex.ToString();
            }
            finally
            {
                IsBusy = false;
                View.SetIsBusy(false);
            }
        }
    }
}