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
    public sealed class GradePointViewModel : BaseViewModel, ICollection<IGradeItem>
    {
        private static readonly Lazy<GradePointViewModel> Lazy =
            new Lazy<GradePointViewModel>(() => new GradePointViewModel());

        private bool lockedView;

        /// <summary>
        /// 绩点成绩列表
        /// </summary>
        public ObservableCollection<IGradeItem> Items { get; set; }

        /// <summary>
        /// 加载绩点的命令
        /// </summary>
        public ICommand LoadItemsCommand { get; set; }

        /// <summary>
        /// 视图模型的实例
        /// </summary>
        public static GradePointViewModel Instance => Lazy.Value;

        /// <summary>
        /// 建立绩点视图模型的数据源和刷新操作。
        /// </summary>
        private GradePointViewModel()
        {
            Title = "学分成绩";
            Items = new ObservableCollection<IGradeItem>();
            LoadItemsCommand = new CommandAction(ExecuteLoadItemsCommand);
        }

        /// <summary>
        /// 加载绩点的具体函数。
        /// </summary>
        public async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return; IsBusy = true;

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

        /// <summary>
        /// 展示成绩详情。
        /// </summary>
        /// <param name="iGi">成绩项</param>
        public async Task ShowGradeDetailAsync(IGradeItem iGi)
        {
            if (iGi is GPAItem) return;
            if (lockedView) return;
            lockedView = true;

            var info = string.Format(
                "名称：{0}\n类型：{1}\n学期：{2}\n发布日期：{3}\n" +
                "学分：{4}\n分数：{5}\n绩点：{6}\n通过：{7}\n重修：{8}",
                iGi.Title, iGi.Type, iGi.Term, iGi.Date.ToString(),
                iGi.Credit, iGi.Score, iGi.Point, iGi.Pass ? "是" : "否", iGi.ReSelect ? "是" : "否");

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

            lockedView = false;
        }

        #region ICollection<T> Implements

        public int Count => Items.Count;
        public void Add(IGradeItem item) => Items.Add(item);
        public void Clear() => Items.Clear();

        bool ICollection<IGradeItem>.IsReadOnly => ((ICollection<IGradeItem>)Items).IsReadOnly;
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        IEnumerator<IGradeItem> IEnumerable<IGradeItem>.GetEnumerator() => Items.GetEnumerator();
        bool ICollection<IGradeItem>.Contains(IGradeItem item) => Items.Contains(item);
        void ICollection<IGradeItem>.CopyTo(IGradeItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
        bool ICollection<IGradeItem>.Remove(IGradeItem item) => Items.Remove(item);
        
        public void AddRange(IEnumerable<IGradeItem> toAdd)
        {
            foreach (var item in toAdd) Items.Add(item);
        }
        
        #endregion
    }
}
