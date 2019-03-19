using System.Threading.Tasks;

namespace HandSchool.Models
{
    /// <summary>
    /// 校内通知Feed项目的储存类。
    /// </summary>
    public abstract class FeedItem
    {
        /// <summary>
        /// 信息编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 信息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 信息具体链接
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 信息评论
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 信息发布日期
        /// </summary>
        public string PubDate { get; set; }

        /// <summary>
        /// 信息创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 信息分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 获得信息描述。
        /// </summary>
        public abstract Task<string> GetDescriptionAsync();

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool Top { get; set; }

        /// <summary>
        /// 置顶附加
        /// </summary>
        public string TopAttach => Top ? "[置顶] " : "";

        /// <summary>
        /// For internal use.
        /// </summary>
        public string Detail => Category;
    }

    public class RssFeedItem : FeedItem
    {
        /// <summary>
        /// 信息描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获得信息描述。
        /// </summary>
        public override Task<string> GetDescriptionAsync()
        {
            return Task.FromResult(Description);
        }
    }
}
