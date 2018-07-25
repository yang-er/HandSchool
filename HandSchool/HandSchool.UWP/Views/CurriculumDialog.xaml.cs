using HandSchool.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace HandSchool.UWP.Views
{
    public sealed partial class CurriculumDialog : ContentDialog
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

            beginDay.SetBinding(Selector.SelectedIndexProperty, new Binding { Path = new PropertyPath("DayBegin"), Mode = BindingMode.TwoWay });
            endDay.SetBinding(Selector.SelectedIndexProperty, new Binding { Path = new PropertyPath("DayEnd"), Mode = BindingMode.TwoWay });
        }

        private void Save(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Core.App.Schedule.Save();
        }

        private void Remove(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Core.App.Schedule.Items.Remove(Model);
            Core.App.Schedule.Save();
        }

        private void Create(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Core.App.Schedule.Items.Add(Model);
            Core.App.Schedule.Save();
        }

        private void Cancel(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }

    public class OenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (WeekOddEvenNone)value;
        }
    }
}
