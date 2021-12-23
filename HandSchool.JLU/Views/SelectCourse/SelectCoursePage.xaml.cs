using HandSchool.Views;
using System;
using System.Threading.Tasks;
using HandSchool.Internal;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HandSchool.JLU.JsonObject;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectCoursePage : ViewObject
    {
        private readonly SelectCourseViewModel _viewModel;

        public SelectCoursePage()
        {
            ViewModel = _viewModel = SelectCourseViewModel.Instance;          
            InitializeComponent();

            SelectCoursePicker.SelectedIndexChanged += (s, e) =>
            {
                var index = ((Picker)s).SelectedIndex;
                _viewModel.CurrentPlan = index < 0 ? null : _viewModel.SelectCoursePlanValues[index];
                Task.Run(_viewModel.GetCourses);
            };
            Task.Run(async () => await _viewModel.GetSelectCoursePlan().ContinueWith(async t =>
             {
                 await t;
                 Core.Platform.EnsureOnMainThread(() =>
                 {
                     if (_viewModel.SelectCoursePlanValues.Count > 0)
                     {
                         SelectCoursePicker.SelectedIndex = 0;
                     }
                 });
             }));
        }

        private async void ShowDetail(object sender, EventArgs e)
        {
            if ((_viewModel.CurrentPlan.StartTime?.CompareTo(DateTime.Now) ?? 1) > 0)
            {
                await NoticeError("选课还未开始\n开始时间：" + _viewModel.CurrentPlan.StartTime);
                return;
            }
            if ((_viewModel.CurrentPlan.EndTime?.CompareTo(DateTime.Now) ?? -1) < 0)
            {
                await NoticeError("选课已结束\n结束时间：" + _viewModel.CurrentPlan.EndTime);
                return;
            }
            var course = (sender as BindableObject)?.BindingContext as SCCourses;
            if (course is null) return;
            if (!(await _viewModel.GetDetail(course.lslId.ToString())).IsSuccess)
            {
                return;
            }
            await Navigation.PushAsync<SCourseDetailPage>(course);
        }

        private async void ShowQuickSelect(object sender, EventArgs args)
        {
            await _viewModel.GetQuickSelect();
            await Navigation.PushAsync<QuickSelectPage>();
        }
    }

    [Entrance("JLU", "线上选课", "对你的课程进行操作", EntranceType.InfoEntrance)]
    public class SelectCourseShell : ITapEntrace
    {
        public Task Action(INavigate navigate)
        {
            navigate.PushAsync(typeof(SelectCoursePage), null);
            return Task.CompletedTask;
        }
    }
}