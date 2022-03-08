using System;
using System.Collections.Generic;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickSelectPage : ViewObject
    {
        private readonly SelectCourseViewModel _viewModel;
        public QuickSelectPage()
        {
            InitializeComponent();            
            ViewModel = _viewModel = SelectCourseViewModel.Instance;
        }

        private async void QuickSelectClicked(object sender, CollectionItemTappedEventArgs args)
        {
            if ((_viewModel.CurrentPlan.EndTime?.CompareTo(DateTime.Now) ?? -1) < 0)
            {
                await NoticeError("选课已结束\n结束时间：" + _viewModel.CurrentPlan.EndTime);
                return;
            }
            var detail = args.Item as SCCourseDetail;
            if (detail is null) return; 
            var list = new List<string>();

            if ((_viewModel.CurrentPlan.StartTime?.CompareTo(DateTime.Now) ?? 1) <= 0)
            {
                var classSelected = detail.selectTag?.Trim() switch
                {
                    "Y" => true,
                    "G" => true,
                    _ => false
                };
                list.Add(classSelected ? "退课" : "选课");
            }
            list.Add("移除快捷选课");

            var res = await RequestActionAsync("选择操作", "取消", null, list.ToArray());
            switch (res)
            {
                case null: return;
                case "移除快捷选课":
                    await _viewModel.SetQuickSelect(detail.lsltId, QuickSelectOperator.Delete).ContinueWith(async t =>
                    {
                        if ((await t).IsSuccess) detail.isQuick = "N";
                        Core.Platform.EnsureOnMainThread(() => _viewModel.QuickSelect.Remove(detail));
                    });
                    break;
                case "选课":
                    {
                        var taskResp = await _viewModel.SelectCourse(detail.lsltId, SelectCourseOperator.Select);
                        if (taskResp.IsSuccess)
                        {
                            await RequestMessageAsync("提示", "选课成功", "好");
                            await Navigation.PopAsync();
                            await _viewModel.GetCourses();
                        }
                        break;
                    }
                case "退课":
                    {
                        var taskResp = await _viewModel.SelectCourse(detail.lsltId, SelectCourseOperator.UnSelect);
                        if (taskResp.IsSuccess)
                        {
                            await RequestMessageAsync("提示", "退课成功", "好");
                            await Navigation.PopAsync();
                            await _viewModel.GetCourses();
                        }
                        break;
                    }
            }
        }

        private async void Refresh(object sender, EventArgs args)
        {
            await _viewModel.GetQuickSelect();
        }

        private async void SelectAll(object sender, EventArgs args)
        {
            var x = await _viewModel.SelectAll();
            if (x == -1) return;
            await RequestMessageAsync("提示", "操作完成，成功选了" + x + "门课", "好");
            await Navigation.PopAsync();
            await _viewModel.GetCourses();
        }
    }
}