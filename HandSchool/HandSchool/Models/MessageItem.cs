using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace HandSchool.Models
{
    /// <summary>
    /// 站内消息Message项，提供了设置读状态和删除的接口。
    /// </summary>
    public interface IMessageItem : INotifyPropertyChanged
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 消息标题
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 消息正文
        /// </summary>
        string Body { get; }

        /// <summary>
        /// 发送时间
        /// </summary>
        DateTime Time { get; }

        /// <summary>
        /// 发送日期
        /// </summary>
        string Date { get; }

        /// <summary>
        /// 发送者
        /// </summary>
        string Sender { get; }

        /// <summary>
        /// 是否未读
        /// </summary>
        bool Unread { get; set; }

        /// <summary>
        /// 设置为已读取
        /// </summary>
        Command SetRead { get; }

        /// <summary>
        /// 设置为未读取
        /// </summary>
        Command SetUnread { get; }

        /// <summary>
        /// 删除消息
        /// </summary>
        Command Delete { get; }
    }
}
