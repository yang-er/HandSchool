using Android.App;
using Android.Content;
using Android.Support.Design.Widget;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using Jaredrummler.MaterialSpinner;
using System.Threading.Tasks;
using JObject = Java.Lang.Object;

namespace HandSchool.Droid
{
    public class CurriculumDialog : ICurriculumPage, IBindTarget
    {
        /// <summary>
        /// 是否为新建课程
        /// </summary>
        public bool IsCreate { get; set; }
        
        /// <summary>
        /// 要设置的课程项
        /// </summary>
        public CurriculumItem Model { get; set; }

        /// <summary>
        /// 异步控制源
        /// </summary>
        public TaskCompletionSource<bool> ControlSource { get; }

        #region UI Bindings

        [BindView(Resource.Id.startnum)]
        public MaterialSpinner StartNum { get; set; }
        
        [BindView(Resource.Id.startweek)]
        public MaterialSpinner StartWeek { get; set; }

        [BindView(Resource.Id.endweek)]
        public MaterialSpinner EndWeek { get; set; }

        [BindView(Resource.Id.endnum)]
        public MaterialSpinner EndNum { get; set; }

        [BindView(Resource.Id.weekoen)]
        public MaterialSpinner WeekOen { get; set; }

        [BindView(Resource.Id.weekday)]
        public MaterialSpinner WeekDay { get; set; }

        [BindView(Resource.Id.teacher)]
        public TextInputEditText Teacher { get; set; }

        [BindView(Resource.Id.classroom)]
        public TextInputEditText ClassRoom { get; set; }

        [BindView(Resource.Id.classname)]
        public TextInputEditText ClassName { get; set; }

        #endregion
        
        private readonly JObject[] WeekDaySelection;
        private readonly JObject[] WeekOenSelection;
        private readonly JObject[] WeekStartSelection;
        private readonly JObject[] DayStartSelection;

        public CurriculumDialog()
        {
            ControlSource = new TaskCompletionSource<bool>();
            WeekDaySelection = new JObject[] { "请选择", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天" };
            WeekOenSelection = new JObject[] { "双周", "单周", "单双周" };
            DayStartSelection = new JObject[Core.App.DailyClassCount + 1];
            WeekStartSelection = new JObject[24];
            DayStartSelection[0] = WeekStartSelection[0] = WeekDaySelection[0];
            for (int i = 1; i <= Core.App.DailyClassCount; i++)
                DayStartSelection[i] = $"第{i}节";
            for (int i = 1; i <= 23; i++)
                WeekStartSelection[i] = $"第{i}周";
        }
        
        public void SolveBindings()
        {
            // Set data sources
            StartNum.SetItems(DayStartSelection);
            StartNum.SelectedIndex = Model.DayBegin;

            EndNum.SetItems(DayStartSelection);
            EndNum.SelectedIndex = Model.DayEnd;

            StartWeek.SetItems(WeekStartSelection);
            StartWeek.SelectedIndex = Model.WeekBegin;

            EndWeek.SetItems(WeekStartSelection);
            EndWeek.SelectedIndex = Model.WeekEnd;

            WeekDay.SetItems(WeekDaySelection);
            WeekDay.SelectedIndex = Model.WeekDay;

            WeekOen.SetItems(WeekOenSelection);
            WeekOen.SelectedIndex = (int)Model.WeekOen;

            ClassName.Text = Model.Name;
            ClassRoom.Text = Model.Classroom;
            Teacher.Text = Model.Teacher;
        }
        
        public void SetNavigationArguments(CurriculumItem item, bool isCreate)
        {
            Model = item;
            IsCreate = isCreate;
        }

        public async Task<bool> IsSuccess()
        {
            await ShowAsync();
            return await ControlSource.Task;
        }

        private (bool legal, string msg) IsLegal()
        {
            if (WeekDay.SelectedIndex == 0) return (false, "星期几不能为空");
            if (StartNum.SelectedIndex == 0) return (false, "起始节不能为空");
            if (EndNum.SelectedIndex == 0) return (false, "结束节不能为空");
            if (StartWeek.SelectedIndex == 0) return (false, "起始周不能为空");
            if (EndWeek.SelectedIndex == 0) return (false, "结束周不能为空");

            if (StartWeek.SelectedIndex > EndWeek.SelectedIndex) return (false, "起始周不能晚于结束周");
            if (StartNum.SelectedIndex > EndNum.SelectedIndex) return (false, "起始节不能晚于结束节");
            return (true, null);

        }
        private void OnFinishEditing(object sender, DialogClickEventArgs args)
        {
            var check = IsLegal();
            if (!check.legal)
            {
                var ac = new AlertDialog.Builder((Core.Platform as PlatformImplV2).PeekContext())
                    .SetTitle("失败")
                    .SetMessage(check.msg)
                    .SetPositiveButton("好", listener: null).Create();
                Core.Platform.EnsureOnMainThread(() =>
                {
                    ac.Show();
                });
            }
            else
            {
                Model.DayBegin = StartNum.SelectedIndex;
                Model.DayEnd = EndNum.SelectedIndex;
                Model.WeekBegin = StartWeek.SelectedIndex;
                Model.WeekEnd = EndWeek.SelectedIndex;
                Model.WeekDay = WeekDay.SelectedIndex;
                Model.WeekOen = (WeekOddEvenNone)WeekOen.SelectedIndex;
                Model.Name = ClassName.Text;
                Model.Classroom = ClassRoom.Text;
                Model.Teacher = Teacher.Text;

                if (IsCreate) ScheduleViewModel.Instance.AddItem(Model);
                ScheduleViewModel.Instance.SaveToFile();
            }
            ControlSource.TrySetResult(true);
        }

        private void OnDeleted(object sender, DialogClickEventArgs args)
        {
            ScheduleViewModel.Instance.RemoveItem(Model);
            ScheduleViewModel.Instance.SaveToFile();
            ControlSource.TrySetResult(true);
        }

        private void OnCancelEditing(object sender, DialogClickEventArgs args)
        {
            ControlSource.TrySetResult(false);
        }

        public Task ShowAsync()
        {
            var context = PlatformImplV2.Instance.PeekContext();

            var builder = new AlertDialog.Builder(context);
            builder.SetView(Resource.Layout.dialog_curriculum);
            builder.SetTitle(IsCreate ? "创建课程" : "修改课程信息");
            builder.SetPositiveButton("保存", OnFinishEditing);
            if (!IsCreate)
                builder.SetNeutralButton("删除", OnDeleted);
            builder.SetNegativeButton("取消", OnCancelEditing);
            builder.SetCancelable(false);
            var dialog = builder.Show();

            this.SolveView(dialog);
            return Task.CompletedTask;
        }
    }
}