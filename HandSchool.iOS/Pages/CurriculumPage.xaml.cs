using HandSchool.Internal;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CurriculumPage : ViewObject, ICurriculumPage
	{
       
        public TaskCompletionSource<bool> Awaiter { get; }
        TableView TableView;
        public CurriculumItem Model
        {
            get => BindingContext as CurriculumItem;
            set => BindingContext = value;
        }

        public CurriculumPage()
        {
            InitializeComponent();
            On<_iOS_>().UseSafeArea().ShowLeftCancel();
            Awaiter = new TaskCompletionSource<bool>();
            TableView = Content as TableView;
            var tableSec = TableView.Root[1];
            foreach (var item in from i in tableSec
                                 where i is PickerCell
                                 select i)
            {
                (item as PickerCell).Father = this;
            }
        }
        private (bool legal, string msg) IsLegal()
        {
            if (weekDay.SelectedIndex == 0) return (false, "星期几不能为空");
            if (startDay.SelectedIndex == 0) return (false, "起始节不能为空");
            if (endDay.SelectedIndex == 0) return (false, "结束节不能为空");
            if (startWeek.SelectedIndex == 0) return (false, "起始周不能为空");
            if (endWeek.SelectedIndex == 0) return (false, "结束周不能为空");

            if (startWeek.SelectedIndex > endWeek.SelectedIndex) return (false, "起始周不能晚于结束周");
            if (startDay.SelectedIndex > endDay.SelectedIndex) return (false, "起始节不能晚于结束节");
            return (true, null);

        }
        private void Sync()
        {
            Model.DayBegin = startDay.SelectedIndex;
            Model.DayEnd = endDay.SelectedIndex;
            Model.WeekBegin = startWeek.SelectedIndex;
            Model.WeekEnd = endWeek.SelectedIndex;
            Model.WeekDay = weekDay.SelectedIndex;
            Model.WeekOen = (WeekOddEvenNone)weekOen.SelectedIndex;
            Model.Name = className.Text;
            Model.Classroom = classroom.Text;
            Model.Teacher = teacher.Text;
        }
        private async Task SaveCommand()
        {
            var check = IsLegal();
            if (!check.legal)
            {
                await RequestMessageAsync("失败", check.msg, "好");
            }
            else
            {
                Sync();
                ScheduleViewModel.Instance.SaveToFile();
                Awaiter.SetResult(true);
                await CloseAsync();
                if (SchedulePage.Instance != null)
                    SchedulePage.Instance.LoadList();
            }
        }

        private async Task RemoveCommand()
        {
            ScheduleViewModel.Instance.RemoveItem(Model);
            ScheduleViewModel.Instance.SaveToFile();
            Awaiter.SetResult(true);
            await CloseAsync();
            if (SchedulePage.Instance != null)
                SchedulePage.Instance.LoadList();
        }

        private async Task CreateCommand()
        {
            var check = IsLegal();
            if (!check.legal)
            {
                await RequestMessageAsync("失败", check.msg, "好");
            }
            else
            {
                Sync();
                ScheduleViewModel.Instance.AddItem(Model);
                ScheduleViewModel.Instance.SaveToFile();
                Awaiter.SetResult(true);
                await CloseAsync();
                if (SchedulePage.Instance != null)
                    SchedulePage.Instance.LoadList();
            }

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
                startDay.Items.Add($"第{i}节");
                endDay.Items.Add($"第{i}节");
            }

            startDay.SetBinding(PickerCell.SelectedIndexProperty, new Binding("DayBegin", BindingMode.OneTime));
            endDay.SetBinding(PickerCell.SelectedIndexProperty, new Binding("DayEnd", BindingMode.OneTime));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Awaiter.TrySetResult(false);
        }

        public Task<bool> ShowAsync()
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
            return Awaiter.Task;
        }

        private Task CloseAsync() => Application.Current.MainPage.Navigation.PopModalAsync();
    }
}