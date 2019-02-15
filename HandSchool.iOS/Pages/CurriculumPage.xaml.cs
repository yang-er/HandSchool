using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CurriculumPage : ViewObject, ICurriculumPage
	{
        public TaskCompletionSource<bool> Awaiter { get; }

        public CurriculumItem Model
        {
            get => BindingContext as CurriculumItem;
            set => BindingContext = value;
        }

        public CurriculumPage()
		{
			InitializeComponent();
            Awaiter = new TaskCompletionSource<bool>();
        }

        private async Task SaveCommand()
        {
            ScheduleViewModel.Instance.SaveToFile();
            Awaiter.SetResult(true);
            await CloseAsync();
        }

        private async Task RemoveCommand()
        {
            ScheduleViewModel.Instance.RemoveItem(Model);
            ScheduleViewModel.Instance.SaveToFile();
            Awaiter.SetResult(true);
            await CloseAsync();
        }

        private async Task CreateCommand()
        {
            ScheduleViewModel.Instance.AddItem(Model);
            ScheduleViewModel.Instance.SaveToFile();
            Awaiter.SetResult(true);
            await CloseAsync();
        }

        public void SetNavigationArguments(CurriculumItem item, bool isCreate)
        {
            Model = item;

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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Awaiter.TrySetResult(false);
        }

        public Task<bool> ShowAsync()
        {
            (this as Page).Navigation.PushModalAsync(this);
            return Awaiter.Task;
        }

        private Task CloseAsync()
        {
            return (this as Page).Navigation.PopModalAsync();
        }
    }
}