using HandSchool.Internals;
using HandSchool.Models;
using System.Windows.Input;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 用于构建消息详情的视图模型。
    /// </summary>
    public class DetailViewModel : BaseViewModel
    {
        string _name, _sender, _date, _content;

        /// <summary>
        /// 消息大标题
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// 消息发送者/新闻发布者
        /// </summary>
        public string Sender
        {
            get => _sender;
            set => SetProperty(ref _sender, value);
        }

        /// <summary>
        /// 发送时间
        /// </summary>
        public string Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        /// <summary>
        /// 正文内容
        /// </summary>
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        /// <summary>
        /// 消息命令
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 显示的图标
        /// </summary>
        public string UWPIcon { get; set; }

        /// <summary>
        /// 从消息项创建。
        /// </summary>
        /// <param name="item">消息</param>
        /// <returns>视图模型</returns>
        public static DetailViewModel From(IMessageItem item)
        {
            return new DetailViewModel
            {
                Title = "消息详情",
                Name = item.Title,
                Sender = "发件人：" + item.Sender,
                Date = "时间：" + item.Time.ToString(),
                Content = item.Body,
                Command = item.Delete,
                Operation = "删除",
                UWPIcon = "\uE74D",
            };
        }

        /// <summary>
        /// 从通知项创建。
        /// </summary>
        /// <param name="item">通知</param>
        /// <returns>视图模型</returns>
        public static DetailViewModel From(FeedItem item)
        {
            var desc = item.Description.Trim();
            while (desc.Contains("    ")) desc = desc.Replace("    ", "  ");

            return new DetailViewModel
            {
                Title = "通知详情",
                Name = item.Title,
                Sender = "分类：" + item.Category,
                Date = "时间：" + item.PubDate,
                Content = desc,
                Command = new CommandAction(() => Core.Platform.OpenUrl(item.Link)),
                Operation = "详情",
                UWPIcon = "\uE7C1",
            };
        }

        /// <summary>
        /// 从某个项创建。
        /// </summary>
        /// <param name="item">某个项</param>
        /// <returns>视图模型</returns>
        public static DetailViewModel From(object item)
        {
            if (item is FeedItem feed) return From(feed);
            else if (item is IMessageItem msg) return From(msg);
            else throw new System.InvalidOperationException();
        }
    }
}