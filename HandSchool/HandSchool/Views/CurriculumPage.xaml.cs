using HandSchool.Models;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CurriculumPage : PopContentPage
	{
		public CurriculumPage(CurriculumLabel item)
		{
			InitializeComponent();
            BindingContext = item.Context;
            saveButton.Command = new Command(async () =>
            {
                item.Update();
                App.Current.Schedule.Save();
                await Close();
            });
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
	}
}