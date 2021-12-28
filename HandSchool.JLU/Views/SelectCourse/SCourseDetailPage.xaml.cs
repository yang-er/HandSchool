using HandSchool.JLU.JsonObject;
using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SCourseDetailPage : ViewObject
    {
        private readonly SelectCourseViewModel _viewModel;
        public SCourseDetailPage()
        {
            InitializeComponent();        
            ViewModel = _viewModel = SelectCourseViewModel.Instance;

        }

        private SCCourses _curCourse;
        public override void SetNavigationArguments(object param)
        {
            Title = param.ToString();
            _curCourse = param as SCCourses;
            base.SetNavigationArguments(param);
        }

        private async void ClassSelected(object sender, EventArgs e)
        {
            if ((_viewModel.CurrentPlan.EndTime?.CompareTo(DateTime.Now) ?? -1) < 0)
            {
                await NoticeError("选课已结束\n结束时间：" + _viewModel.CurrentPlan.EndTime);
                return;
            }               
            var detail = (sender as BindableObject)?.BindingContext as SCCourseDetail;
            if (detail is null) return;
            var list = new List<string>();
            if ((_viewModel.CurrentPlan.StartTime?.CompareTo(DateTime.Now) ?? 1) <= 0)
            {

                if (detail.selectTag?.Trim() == "G")
                {
                    await RequestMessageAsync("提示", "该教学班被固定，无法操作", "好");
                    return;
                }
                var courseSelected = _curCourse?.selectResult?.Trim() == "Y";
                var classSelected = detail.selectTag?.Trim() switch
                {
                    "Y" => true,
                    "G" => true,
                    _ => false
                };


                if (courseSelected)
                {
                    if (classSelected)
                    {
                        list.Add("退课");
                    }
                    else
                    {
                        await RequestMessageAsync("提示", "本门课已选其它教学班", "好");
                        return;
                    }
                }
                else
                {
                    list.Add("选课");
                }
            }
            var isQuick = detail.isQuick?.Trim() == "Y";
            list.Add(isQuick ? "移除快捷选课" : "加入快捷选课");
            var res = await RequestActionAsync("选择操作", "取消", null, list.ToArray());
            switch (res)
            {
                case null: return;
                case "移除快捷选课":
                    await _viewModel.SetQuickSelect(detail.lsltId, QuickSelectOperator.Delete).ContinueWith(async t => { if ((await t).IsSuccess) detail.isQuick = "N"; });
                    break;
                case "加入快捷选课":
                    await _viewModel.SetQuickSelect(detail.lsltId, QuickSelectOperator.Add).ContinueWith(async t => { if ((await t).IsSuccess) detail.isQuick = "Y"; });
                    break;
                case "选课":
                    {
                        var taskResp = await _viewModel.SelectCourse(detail.lsltId, SelectCourseOperator.Select);
                        if (taskResp.IsSuccess)
                        {
                            await RequestMessageAsync("提示", "选课成功", "好");
                            detail.selectTag = "Y";
                            _curCourse.selectResult = "Y";
                            await Navigation.PopAsync();
                            var index = _viewModel.Courses.IndexOf(_curCourse);
                            if (index < 0) return;
                            Core.Platform.EnsureOnMainThread(() =>
                            {
                                _viewModel.Courses.Remove(_curCourse);
                            });
                            Core.Platform.EnsureOnMainThread(() =>
                            {
                                _viewModel.Courses.Insert(index, _curCourse);
                            });
                        }
                        break;
                    }
                case "退课":
                    {
                        var taskResp = await _viewModel.SelectCourse(detail.lsltId, SelectCourseOperator.UnSelect);
                        if (taskResp.IsSuccess)
                        {
                            await RequestMessageAsync("提示", "退课成功", "好");
                            detail.selectTag = "N";
                            _curCourse.selectResult = "N";
                            await Navigation.PopAsync();
                            var index = _viewModel.Courses.IndexOf(_curCourse);
                            if (index < 0) return;
                            Core.Platform.EnsureOnMainThread(() =>
                            {
                                _viewModel.Courses.Remove(_curCourse);
                            });
                            Core.Platform.EnsureOnMainThread(() =>
                            {
                                _viewModel.Courses.Insert(index, _curCourse);
                            });
                        }
                        break;
                    }
            }
        }
    }
}