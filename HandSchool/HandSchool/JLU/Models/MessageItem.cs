using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using Xamarin.Forms;

namespace HandSchool.JLU.Models
{
    class MessageItem : NotifyPropertyChanged, IMessageItem
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
        public Command SetRead { get; }
        public Command SetUnread { get; }
        public Command Delete { get; }
        public bool IsShowed = true;

        public MessageItem(MessagePiece p)
        {
            piece = p;
            _unread = piece.hasReaded == "N";

            SetRead = new Command(async () =>
            {
                await Core.App.Message.SetReadState(Id, true);
                Unread = false;
            });

            SetUnread = new Command(async () =>
            {
                await Core.App.Message.SetReadState(Id, false);
                Unread = true;
            });

            Delete = new Command(async () =>
            {
                await Core.App.Message.Delete(Id);
                MessageViewModel.Instance.Remove(this);
            });
        }
    }
}
