using HandSchool.Models;
using System;
using System.Globalization;
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
                removeButton.Command = new Command(async () => await CloseAsync());
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
            
            for (int i = 1; i <= Core.App.DailyClassCount; i++) 
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
                        lab.FontSize = Device.GetNamedSize(NamedSize.Default, lab);
                    else if (lab.FontAttributes == FontAttributes.Bold)
                        lab.FontSize = Device.GetNamedSize(NamedSize.Medium, lab);
                    lab.FontAttributes = FontAttributes.None;
                    lab.VerticalOptions = LayoutOptions.Center;
                }
            }
        }

        async Task SaveCommand()
        {
            Core.App.Schedule.Save();
            await CloseAsync();
        }

        async Task RemoveCommand()
        {
            Core.App.Schedule.Items.Remove(BindingContext as CurriculumItem);
            Core.App.Schedule.Save();
            await CloseAsync();
        }

        async Task CreateCommand()
        {
            Core.App.Schedule.Items.Add(BindingContext as CurriculumItem);
            Core.App.Schedule.Save();
            await CloseAsync();
        }

    }

    /// <summary>
    /// WeekOen与int互相转化
    /// </summary>
    public class OenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return (WeekOddEvenNone)value;
        }
    }
}