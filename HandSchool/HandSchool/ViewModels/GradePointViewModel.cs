using HandSchool.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 绩点成绩的视图模型，提供了加载绩点的命令和数据源。
    /// </summary>
    public class GradePointViewModel : BaseViewModel
    {
        static GradePointViewModel instance = null;

        /// <summary>
        /// 绩点成绩列表
        /// </summary>
        public ObservableCollection<IGradeItem> Items { get; set; }

        /// <summary>
        /// 加载绩点的命令
        /// </summary>
        public Command LoadItemsCommand { get; set; }

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static GradePointViewModel Instance
        {
            get
            {
                if (instance is null)
                    instance = new GradePointViewModel();
                return instance;
            }
        }

        /// <summary>
        /// 建立绩点视图模型的数据源和刷新操作。
        /// </summary>
        private GradePointViewModel()
        {
            Title = "学分成绩";
            Items = new ObservableCollection<IGradeItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        /// <summary>
        /// 加载绩点的具体函数。
        /// </summary>
        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return; IsBusy = true;

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
