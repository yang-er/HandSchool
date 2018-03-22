using HandSchool.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CurriculumPage : PopContentPage
	{
        public bool IsSave;

        public CurriculumPage(CurriculumItem item, bool isCreate = false)
		{
			InitializeComponent();
            BindingContext = item;

            if (isCreate)
            {
                saveButton.Command = new Command(async () => await CreateCommand());
                removeButton.Command = new Command(async () => await Close());
                saveButton.Text = "创建";
                removeButton.Text = "取消";
                Title = "添加自定义课程";
            }
            else
            {
                saveButton.Command = new Command(async () => await SaveCommand());
                removeButton.Command = new Command(async () => await RemoveCommand());
                saveButton.Text = "保存";
                removeButton.Text = "删除";
                Title = "编辑课程";
            }
            
            for (int i = 1; i <= App.Current.DailyClassCount; i++) 
            {
                beginDay.Items.Add($"第{i}节");
                endDay.Items.Add($"第{i}节");
            }
            beginDay.SetBinding(Picker.SelectedIndexProperty, new Binding("DayBegin"));
            endDay.SetBinding(Picker.SelectedIndexProperty, new Binding("DayEnd"));

            foreach (var sub in grid.Children)
            {
                if (sub is Label lab)
                {
                    if (lab.FontAttributes == FontAttributes.None)
                        lab.FontSize = Device.GetNamedSize(Device.OnPlatform(NamedSize.Default, NamedSize.Medium, NamedSize.Default), lab);
                    else if (lab.FontAttributes == FontAttributes.Bold)
                        lab.FontSize = Device.GetNamedSize(Device.OnPlatform(NamedSize.Medium, NamedSize.Large, NamedSize.Medium), lab);
                    lab.FontAttributes = FontAttributes.None;
                    lab.VerticalOptions = LayoutOptions.Center;
                }
            }
        }

        async Task SaveCommand()
        {
            App.Current.Schedule.Save();
            await Close();
        }

        async Task RemoveCommand()
        {
            App.Current.Schedule.Items.Remove(BindingContext as CurriculumItem);
            App.Current.Schedule.Save();
            await Close();
        }

        async Task CreateCommand()
        {
            App.Current.Schedule.Items.Add(BindingContext as CurriculumItem);
            App.Current.Schedule.Save();
            await Close();
        }
	}
}