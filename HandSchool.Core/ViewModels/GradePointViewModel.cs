using HandSchool.Internals;
using HandSchool.Models;
using Microcharts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 绩点成绩的视图模型，提供了加载绩点的命令和数据源。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    /// <inheritdoc cref="ICollection{T}" />
    public sealed class GradePointViewModel : BaseViewModel
    {
        private static readonly Lazy<GradePointViewModel> Lazy =
            new Lazy<GradePointViewModel>(() => new GradePointViewModel());

        private bool _lockedView;

        /// <summary>
        /// 绩点成绩列表
        /// </summary>
        public ObservableCollection<IGradeItem> NewerGradeItems { get; set; }
        
        public ObservableCollection<IBasicGradeItem> AllGradeItems { get; set; }

        /// <summary>
        /// 加载绩点的命令
        /// </summary>
        public ICommand LoadNewerItemsCommand { get; set; }
        
        public ICommand LoadAllItemsCommand { get; set; }


        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static GradePointViewModel Instance => Lazy.Value;

        /// <summary>
        /// 建立绩点视图模型的数据源和刷新操作。
        /// </summary>
        private GradePointViewModel()
        {
            Title = "最新成绩";
            NewerGradeItems = new ObservableCollection<IGradeItem>();
            AllGradeItems = new ObservableCollection<IBasicGradeItem>();
            LoadNewerItemsCommand = new CommandAction(ExecuteLoadNewerItemsCommand);
            LoadAllItemsCommand = new CommandAction(ExecuteLoadAllItemsCommand);
        }

        /// <summary>
        /// 加载绩点的具体函数。
        /// </summary>
        public async Task ExecuteLoadNewerItemsCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            var msg = await CheckEnv("LoadNewerItems");
            if (!msg)
            {
                await RequestMessageAsync("错误", msg.ToString());
                IsBusy = false;
                return;
            }

            try
            {
                await Core.App.GradePoint.Execute();
            }
            catch (Exception ex)
            {
                this.WriteLog(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteLoadAllItemsCommand()
        {
            if (IsBusy) return;

            IsBusy = true;
            var msg = await CheckEnv("LoadAllItems");
            if (!msg)
            {
                await RequestMessageAsync("错误", msg.ToString());
                IsBusy = false;
                return;
            }

            try
            {
                await Core.App.GradePoint.EntranceAll();
            }
            catch (Exception ex)
            {
                this.WriteLog(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// 展示成绩详情。
        /// </summary>
        /// <param name="iGi">成绩项</param>
        public async Task ShowGradeDetailAsync(IGradeItem iGi)
        {
            if (iGi is GPAItem) return;
            if (_lockedView) return;
            _lockedView = true;

            var info = $"名称：{iGi.Title}\n类型：{iGi.Type}\n学期：{iGi.Term}\n发布日期：{iGi.Date.ToString()}\n" +
                       $"学分：{iGi.Credit}\n分数：{iGi.FirstScore}\n绩点：{iGi.FirstPoint}\n通过：{(iGi.IsPassed ? "是" : "否")}\n重修：{(iGi.ReSelect ? "是" : "否")}";

            foreach (var key in iGi.Attach.Keys)
            {
                info += "\n" + key + "：" + iGi.Attach.Get((string)key);
            }

            await RequestMessageAsync("成绩详情", info, "确定");

            var list = iGi.GetGradeDistribute().ToList();
            if (list.Count > 0)
            {
                var pie = new PieChart { Entries = list, Margin = 10 };
                await RequestChartAsync(pie, "成绩分布");
            }

            _lockedView = false;
        }
    }
}
