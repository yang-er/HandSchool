using Android.App;
using Android.Support.Design.Widget;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using Jaredrummler.MaterialSpinner;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using JavaObject = Java.Lang.Object;

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
        
        [BindView(Resource.Id.curriculum_delete)]
        public Button ButtonDelete { get; set; }
        
        [BindView(Resource.Id.curriculum_cancel)]
        public Button ButtonCancel { get; set; }
        
        [BindView(Resource.Id.curriculum_save)]
        public Button ButtonSave { get; set; }
        private bool _shouldDismiss;
        private AlertDialog _dialog;
        #endregion
        
        private readonly JavaObject[] _weekDaySelection;
        private readonly JavaObject[] _weekOenSelection;
        private readonly JavaObject[] _weekStartSelection;
        private readonly JavaObject[] _dayStartSelection;

        public CurriculumDialog()
        {
            ControlSource = new TaskCompletionSource<bool>();
            _weekDaySelection = new JavaObject[] { "请选择", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天" };
            _weekOenSelection = new JavaObject[] { "双周", "单周", "单双周" };
            _dayStartSelection = new JavaObject[Core.App.DailyClassCount + 1];
            _weekStartSelection = new JavaObject[24];
            _dayStartSelection[0] = _weekStartSelection[0] = _weekDaySelection[0];
            for (var i = 1; i <= Core.App.DailyClassCount; i++)
                _dayStartSelection[i] = $"第{i}节";
            for (var i = 1; i <= 23; i++)
                _weekStartSelection[i] = $"第{i}周";
        }
        
        public void SolveBindings()
        {
            // Set data sources
            StartNum.SetItems(_dayStartSelection);
            StartNum.SelectedIndex = Model.DayBegin;

            EndNum.SetItems(_dayStartSelection);
            EndNum.SelectedIndex = Model.DayEnd;

            StartWeek.SetItems(_weekStartSelection);
            StartWeek.SelectedIndex = Model.WeekBegin;

            EndWeek.SetItems(_weekStartSelection);
            EndWeek.SelectedIndex = Model.WeekEnd;

            WeekDay.SetItems(_weekDaySelection);
            WeekDay.SelectedIndex = Model.WeekDay;

            WeekOen.SetItems(_weekOenSelection);
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

        private (bool, string) IsLegal()
        {
            if (StartWeek.SelectedIndex == 0) return (false, "起始周不能为空");
            if (StartWeek.SelectedIndex > EndWeek.SelectedIndex) return (false, "起始周不能晚于结束周");
            if (EndWeek.SelectedIndex == 0) return (false, "结束周不能为空");
            if (StartNum.SelectedIndex == 0) return (false, "起始节不能为空");
            if (StartNum.SelectedIndex > EndNum.SelectedIndex) return (false, "起始节不能晚于结束节");
            if (EndNum.SelectedIndex == 0) return (false, "结束节不能为空");
            if (WeekDay.SelectedIndex == 0) return (false, "星期几不能为空");
            return (true, null);
        }
        
        private void OnFinishEditing()
        {
            var (legal, msg) = IsLegal();
            if (!legal)
            {
                var topActivity = PlatformImplV2.Instance.PeekAliveActivity();
                topActivity.RunOnUiThread(() =>
                {
                    new AlertDialog.Builder(topActivity)
                        .SetTitle("失败")
                        !.SetMessage(msg)
                        !.SetPositiveButton("好", listener: null)
                        !.Show();
                });
                _shouldDismiss = false;
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
                ControlSource.TrySetResult(true);
                _shouldDismiss = true;
            }
        }

        private void OnDeleted()
        {
            ScheduleViewModel.Instance.RemoveItem(Model);
            ScheduleViewModel.Instance.SaveToFile();
            ControlSource.TrySetResult(true);
            _shouldDismiss = true;
        }

        private void OnCancelEditing()
        {
            ControlSource.TrySetResult(false);
            _shouldDismiss = true;
        }

        private Task ShowAsync()
        {
            var context = PlatformImplV2.Instance.PeekAliveActivity();

            var builder = new AlertDialog.Builder(context);
            builder.SetView(Resource.Layout.dialog_curriculum);
            builder.SetTitle(IsCreate ? "创建课程" : "修改课程信息");
            builder.SetCancelable(false);
            _dialog = builder.Show();
            this.SolveView(_dialog);

            ButtonSave.SetOnClickListener(new ClickListener(v =>
            {
                OnFinishEditing();
                if (_shouldDismiss)
                {
                    RemoveListeners();
                    _dialog.Dismiss();
                }
            }));
            
            ButtonCancel.SetOnClickListener(new ClickListener(v =>
            {
                OnCancelEditing();
                if (_shouldDismiss)
                {
                    RemoveListeners();
                    _dialog.Dismiss();
                }
            }));

            if (!IsCreate)
            {
                ButtonDelete.SetOnClickListener(new ClickListener(v =>
                {
                    OnDeleted();
                    if (_shouldDismiss)
                    {
                        RemoveListeners();
                        _dialog.Dismiss();
                    }
                }));
            }
            else
            {
                ButtonDelete.Visibility = ViewStates.Invisible;
            }
            return Task.CompletedTask;
        }

        private void RemoveListeners()
        {
            ButtonSave.SetOnClickListener(null);
            ButtonCancel.SetOnClickListener(null);
            ButtonDelete.SetOnClickListener(null);
        }
    }
}