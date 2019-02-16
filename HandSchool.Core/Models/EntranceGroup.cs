using System.Collections.ObjectModel;

namespace HandSchool.Models
{
    /// <summary>
    /// 实现了 <see cref="ObservableCollection{EntranceWrapperBase}"/> 的带标题的入口点信息组。
    /// </summary>
    public class InfoEntranceGroup : ObservableCollection<EntranceWrapperBase>
    {
        /// <summary>
        /// 创建一个带标题的入口点信息组。
        /// </summary>
        /// <param name="tit">组的标题名称，用于在ListView中显示。</param>
        public InfoEntranceGroup(string tit = "")
        {
            GroupTitle = tit;
        }

        /// <summary>
        /// 组标题
        /// </summary>
        public string GroupTitle { get; set; }

        /// <summary>
        /// 返回一个字符串表示当前对象。
        /// </summary>
        /// <returns>当前对象</returns>
        public override string ToString()
        {
            return GroupTitle;
        }
    }
}
