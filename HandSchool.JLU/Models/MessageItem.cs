using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;

namespace HandSchool.JLU.Models
{
    /// <summary>
    /// UIMS收件箱信息项目。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    /// <inheritdoc cref="IMessageItem" />
    internal class MessageItem : NotifyPropertyChanged, IMessageItem
    {
        private readonly MessagePiece piece;
        private bool _unread;

        public int Id => int.Parse(piece.msgInboxId);
        public string Title => piece.message.title;
        public string Body => piece.message.body;
        public DateTime Time => piece.message.dateCreate;
        public string Sender => (piece.message.sender is null ? "系统" : piece.message.sender.name);
        public string Date => piece.message.dateCreate.ToShortDateString();
        public bool Unread { get => _unread; set => SetProperty(ref _unread, value); }
        public CommandAction SetRead { get; }
        public CommandAction SetUnread { get; }
        public CommandAction Delete { get; }

        public MessageItem(MessagePiece p)
        {
            piece = p;
            _unread = piece.hasReaded == "N";
            SetRead = new CommandAction(SetReadAsync);
            SetUnread = new CommandAction(SetUnreadAsync);
            Delete = new CommandAction(DeleteAsync);
        }

        private async Task SetReadAsync()
        {
            await Core.App.Message.SetReadState(Id, true);
            Unread = false;
        }

        private async Task SetUnreadAsync()
        {
            await Core.App.Message.SetReadState(Id, false);
            Unread = true;
        }

        private async Task DeleteAsync()
        {
            await Core.App.Message.Delete(Id);
            MessageViewModel.Instance.Remove(this);
        }
    }
}
