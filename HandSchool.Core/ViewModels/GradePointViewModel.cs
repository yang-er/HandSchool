using HandSchool.Internal;
using HandSchool.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        /// <summary>
        /// 绩点成绩列表
        /// </summary>
        public ObservableCollection<IGradeItem> Items { get; set; }

        /// <summary>
        /// 加载绩点的命令
        /// </summary>
        public Command LoadItemsCommand { get; set; }

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
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);
        }

        /// <summary>
        /// 加载绩点的具体函数。
        /// </summary>
        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy) return; IsBusy = true;

            try
            {
                await Core.App.GradePoint.Execute();
            }
            catch (Exception ex)
            {
                Core.Log(ex);
            }
            finally
            {
                IsBusy = false;
            }
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
