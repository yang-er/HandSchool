using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.UWP;
using HandSchool.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace HandSchool.Views
{
    public sealed partial class CurriculumDialog : ViewDialog
    {
        public CurriculumItem Model
        {
            get => DataContext as CurriculumItem;
            set => DataContext = value;
        }
        
        public CurriculumDialog(CurriculumItem item, bool isCreate = false)
        {
            InitializeComponent();
            Model = item;

            if (isCreate)
            {
                PrimaryButtonClick += Create;
                SecondaryButtonClick += Cancel;
                PrimaryButtonText = "创建";
                SecondaryButtonText = "取消";
                Title = "添加自定义课程";
            }
            else
            {
                PrimaryButtonClick += Save;
                SecondaryButtonClick += Remove;
                Title = "编辑课程";
                PrimaryButtonText = "保存";
                SecondaryButtonText = "删除";
            }
            
            for (int i = 1; i <= Core.App.DailyClassCount; i++)
            {
                beginDay.Items.Add(new ComboBoxItem { Content = $"第{i}节" });
                endDay.Items.Add(new ComboBoxItem { Content = $"第{i}节" });
            }

            beginDay.SetBinding(Selector.SelectedIndexProperty, "DayBegin");
            endDay.SetBinding(Selector.SelectedIndexProperty, "DayEnd");
        }

        private void Save(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ScheduleViewModel.Instance.SaveToFile();
        }

        private void Remove(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ScheduleViewModel.Instance.RemoveItem(Model);
            ScheduleViewModel.Instance.SaveToFile();
        }

        private void Create(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ScheduleViewModel.Instance.AddItem(Model);
            ScheduleViewModel.Instance.SaveToFile();
        }

        private void Cancel(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
