namespace HandSchool.Models
{
    /// <summary>
    /// 校内通知Feed项目的储存类。
    /// </summary>
    public class FeedItem
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
        /// 信息描述
        /// </summary>
        public string Description { get; set; }
    }
}
