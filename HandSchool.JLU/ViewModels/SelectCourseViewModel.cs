﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HandSchool.ViewModels
{
    public class SelectCourseViewModel : NoticeCheckViewModel
    {
        public SelectCourseViewModel()
        {
            LoadCourseCommand = new CommandAction(GetCourses);
        }

        public static SelectCourseViewModel Instance => Lazy.Value;
        private static readonly Lazy<SelectCourseViewModel> Lazy = new Lazy<SelectCourseViewModel>();

        private const string PostUrl = "service/res.do";
        private const string QuickSelectUrl = "action/select/set-lslt-quick-select.do";

        private string SelectCoursePlanPostValue =>
            "{\"tag\": \"common@selectPlan\",\"branch\": \"byStudentSet\",\"params\": {\"studId\": " + UserId + "}}";

        private string SelectCoursesPostValue =>
            "{\"tag\": \"lessonSelectLog@selectStore\",\"branch\": \"default\",\"params\": {\"splanId\": " +
            CurrentPlan.splanId + "}}";

        private string GetCourseDetailPostValue(string lslId) =>
            "{\"tag\": \"lessonSelectLogTcm@selectGlobalStore\",\"branch\": \"self\",\"params\": {\"lslId\": " + lslId +
            ",\"myCampus\": \"Y\"}}";

        private SelectCoursePlanValue _currentPlan;

        public SelectCoursePlanValue CurrentPlan
        {
            get => _currentPlan;
            set
            {
                _currentPlan = value;
                OnPropertyChanged(nameof(CurPlanEnd));
                OnPropertyChanged(nameof(CurPlanStart));
            }
        }
        public string CurPlanStart => CurrentPlan?.StartTime?.ToString() ?? "开始时间";
        public string CurPlanEnd => CurrentPlan?.EndTime?.ToString() ?? "结束时间";
        private string UserId { get; set; }
        public ICommand LoadCourseCommand { get; set; }

        public ObservableCollection<SCCourseDetail> Details { get; }
            = new ObservableCollection<SCCourseDetail>();

        public ObservableCollection<SCCourseDetail> QuickSelect { get; }
            = new ObservableCollection<SCCourseDetail>();

        public ObservableCollection<SelectCoursePlanValue> SelectCoursePlanValues { get; }
            = new ObservableCollection<SelectCoursePlanValue>();

        public ObservableCollection<SCCourses> Courses { get; }
            = new ObservableCollection<SCCourses>();

        public async Task GetSelectCoursePlan()
        {
            if (IsBusy) return;
            if (!await CheckEnvAndNotice("GetSelectCoursePlan")) return;
            IsBusy = true;
            Core.Platform.EnsureOnMainThread(() => { SelectCoursePlanValues.Clear(); });
            try
            {
                var stuInfo = await Core.App.Service.Get("action/getCurrentUserInfo.do");
                var stuJo = JsonConvert.DeserializeObject<JObject>(stuInfo);
                if (stuJo == null)
                {
                    await NoticeError("服务器返回信息有问题");
                    return;
                }

                var uid = stuJo["userId"]?.ToObject<string>();
                if (string.IsNullOrWhiteSpace(uid))
                {
                    await NoticeError("服务器返回信息有问题");
                    return;
                }

                UserId = uid;

                var res = await Core.App.Service.Post(PostUrl, SelectCoursePlanPostValue);
                var jo = JsonConvert.DeserializeObject<JObject>(res);
                if (jo == null)
                {
                    await NoticeError("服务器返回信息有问题");
                    return;
                }

                var values = jo["value"]?.ToObject<List<SelectCoursePlanValue>>();
                Core.Platform.EnsureOnMainThread(() => { values?.ForEach(v => { SelectCoursePlanValues.Add(v); }); });
                IsBusy = false;
            }
            catch (Exception e)
            {
                await NoticeError("出错了：" + e.Message);
                return;
            }
        }

        public async Task GetCourses()
        {
            if (IsBusy) return;
            if (!await CheckEnvAndNotice("GetCourses")) return;
            IsBusy = true;
            Core.Platform.EnsureOnMainThread(() => { Courses.Clear(); });
            try
            {
                if (CurrentPlan is null)
                {
                    await NoticeError("尚未选择选课计划");
                    return;
                }

                var res = await Core.App.Service.Post(PostUrl, SelectCoursesPostValue);
                var jo = JsonConvert.DeserializeObject<JObject>(res);
                if (jo == null)
                {
                    await NoticeError("服务器返回信息有问题");
                    return;
                }

                var values = jo["value"]?.ToObject<List<SCCourses>>();
                Core.Platform.EnsureOnMainThread(() => { values?.ForEach(v => { Courses.Add(v); }); });
                IsBusy = false;
            }
            catch (Exception e)
            {
                await NoticeError("出错了：" + e.Message);
            }
        }

        public async Task<TaskResp> GetDetail(string lslId)
        {
            if (IsBusy) return TaskResp.False;
            if (!await CheckEnvAndNotice("GetDetail")) return TaskResp.False;
            IsBusy = true;
            Core.Platform.EnsureOnMainThread(() => { Details.Clear(); });
            try
            {
                var res = await Core.App.Service.Post(PostUrl, GetCourseDetailPostValue(lslId));
                var jo = JsonConvert.DeserializeObject<JObject>(res);
                if (jo is null)
                {
                    await NoticeError("服务器返回信息有问题");
                    return TaskResp.False;
                }

                var values = jo["value"]?.ToObject<List<SCCourseDetail>>();

                Core.Platform.EnsureOnMainThread(() => { values?.ForEach(v => Details.Add(v)); });
                IsBusy = false;
            }
            catch (Exception e)
            {
                await NoticeError("出错了：" + e.Message);
                return TaskResp.False;
            }

            return TaskResp.True;
        }

        public async Task<TaskResp> SelectCourse(string lsltId, SelectCourseOperator @operator)
        {
            if (IsBusy) return TaskResp.False;
            if (!await CheckEnvAndNotice("SelectCourse")) return TaskResp.False;
            IsBusy = true;
            try
            {
                var selectUrl = "action/select/select-lesson.do";
                var op = @operator == SelectCourseOperator.Select ? 'Y' : 'N';
                var postValue = "{\"lsltId\":\"" + lsltId + "\",\"opType\":\"" + op + "\"}";
                var res = await Core.App.Service.Post(selectUrl, postValue);
                var jo = JsonConvert.DeserializeObject<JObject>(res);
                IsBusy = false;
                if (jo == null)
                {
                    await NoticeError("服务器返回信息有问题");
                    return TaskResp.False;
                }

                if (jo["status"]?.ToString()?.Trim() == "0")
                {
                    return TaskResp.True;
                }
                else
                {
                    await NoticeError("来自UIMS：" + jo["msg"]);
                    return TaskResp.False;
                }
            }
            catch (Exception e)
            {
                await NoticeError("出错了：" + e.Message);
                return TaskResp.False;
            }
        }

        public async Task<int> SelectAll()
        {
            if ((CurrentPlan?.StartTime?.CompareTo(DateTime.Now) ?? 1) > 0)
            {
                await NoticeError("选课还未开始\n开始时间：" + CurrentPlan?.StartTime);
                return -1;
            }
            if ((CurrentPlan?.EndTime?.CompareTo(DateTime.Now) ?? -1) < 0)
            {
                await NoticeError("选课已结束\n结束时间：" + CurrentPlan?.EndTime);
                return -1;
            }
            if (IsBusy) return 0;
            if (!await CheckEnvAndNotice("SelectAll")) return 0;
            IsBusy = true;
            
            var count = 0;
            foreach (var item in QuickSelect)
            {
                var val = item.selectTag?.Trim();
                if (val switch
                    {
                        "Y" => true,
                        "G" => true,
                        _ => false
                    })
                {
                    continue;
                }

                try
                {
                    var lsltId = item.lsltId;
                    var selectUrl = "action/select/select-lesson.do";
                    var postValue = "{\"lsltId\":\"" + lsltId + "\",\"opType\":\"Y\"}";
                    var res = await Core.App.Service.Post(selectUrl, postValue);
                    var jo = JsonConvert.DeserializeObject<JObject>(res);
                    if (jo == null)
                    {
                        continue;
                    }

                    if (jo["status"]?.ToString()?.Trim() == "0")
                    {
                        count++;
                    }
                }
                catch
                {
                    continue;
                }
            }

            IsBusy = false;
            return count;
        }

        public async Task<TaskResp> SetQuickSelect(string lsltId, QuickSelectOperator @operator)
        {
            if (IsBusy) return TaskResp.False;
            if (!await CheckEnvAndNotice("SetQuickSelect")) return TaskResp.False;
            IsBusy = true;

            var op = @operator == QuickSelectOperator.Add ? 'A' : 'D';
            var postValue = "{\"lsltId\": \"" + lsltId + "\",\"operator\": \"" + op + "\"}";

            try
            {
                await Core.App.Service.Post(QuickSelectUrl, postValue);
            }
            catch (Exception e)
            {
                await NoticeError("出错了：" + e.Message);
                return TaskResp.False;
            }

            await RequestMessageAsync("提示", "操作成功", "好");
            IsBusy = false;
            return TaskResp.True;
        }

        public async Task GetQuickSelect()
        {
            if (IsBusy) return;
            if (CurrentPlan is null)
            {
                await NoticeError("暂无选课计划");
                return;
            }
            if (!await CheckEnvAndNotice("GetQuickSelect")) return;
            IsBusy = true;

            Core.Platform.EnsureOnMainThread(() => { QuickSelect.Clear(); });
            try
            {
                var postValue =
                    "{\"tag\":\"lessonSelectLogTcm@selectGlobalStore\",\"branch\":\"quick\",\"params\":{\"splanId\":" +
                    CurrentPlan.splanId + "}}";
                var res = await Core.App.Service.Post(PostUrl, postValue);
                var jo = JsonConvert.DeserializeObject<JObject>(res);
                if (jo == null)
                {
                    await NoticeError("服务器返回信息有问题");
                    return;
                }

                var values = jo["value"]?.ToObject<List<SCCourseDetail>>();
                IsBusy = false;
                Core.Platform.EnsureOnMainThread(() => { values?.ForEach(v => QuickSelect.Add(v)); });
            }
            catch (Exception e)
            {
                await NoticeError("出错了：" + e.Message);
            }
        }
    }

    public enum SelectCourseOperator
    {
        Select, UnSelect
    }

    public enum QuickSelectOperator
    {
        Add,Delete
    }
}
