using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CurriculumPage : ViewPage
	{
        public CurriculumItem Binding
        {
            get => BindingContext as CurriculumItem;
            set => BindingContext = value;
        }

        public CurriculumPage(CurriculumItem item, bool isCreate = false)
		{
			InitializeComponent();
            Binding = item;

            if (isCreate)
            {
                saveButton.Command = new CommandAction(CreateCommand);
                removeButton.Command = new CommandAction(CloseAsync);
                saveButton.Text = "创建";
                removeButton.Text = "取消";
                Title = "添加自定义课程";
            }
            else
            {
                saveButton.Command = new CommandAction(SaveCommand);
                removeButton.Command = new CommandAction(RemoveCommand);
                saveButton.Text = "保存";
                removeButton.Text = "删除";
                Title = "编辑课程";
            }
            
            for (int i = 1; i <= Core.App.DailyClassCount; i++) 
            {
                beginDay.Items.Add($"第{i}节");
                endDay.Items.Add($"第{i}节");
            }

            beginDay.SetBinding(PickerCell.SelectedIndexProperty, new Binding("DayBegin"));
            endDay.SetBinding(PickerCell.SelectedIndexProperty, new Binding("DayEnd"));
        }

        private async Task SaveCommand()
        {
            ScheduleViewModel.Instance.SaveToFile();
            await CloseAsync();
        }

        private async Task RemoveCommand()
        {
            ScheduleViewModel.Instance.RemoveItem(Binding);
            ScheduleViewModel.Instance.SaveToFile();
            await CloseAsync();
        }

        private async Task CreateCommand()
        {
            ScheduleViewModel.Instance.AddItem(Binding);
            ScheduleViewModel.Instance.SaveToFile();
            await CloseAsync();
        }
    }
}
