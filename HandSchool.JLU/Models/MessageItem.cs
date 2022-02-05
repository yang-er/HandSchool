using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HandSchool.JLU.Models
{
    /// <summary>
    /// UIMS收件箱信息项目。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    /// <inheritdoc cref="IMessageItem" />
    internal class MessageItem : NotifyPropertyChanged, IMessageItem
    {
        private readonly MessagePiece _piece;
        private bool _unread;
        public Xamarin.Forms.Color ReadState => Unread ? Xamarin.Forms.Color.Red : Xamarin.Forms.Color.Gray;
        public int Id => int.Parse(_piece.msgInboxId);
        public string Title => _piece.message.title;
        public string Body => _piece.message.body;
        public string Detail => _piece.message.body;
        public DateTime Time => _piece.message.dateCreate;
        public string Sender => (_piece.message.sender is null ? "系统" : _piece.message.sender.name);
        public string Date => Device.RuntimePlatform == Device.Android ? _piece.message.dateCreate.ToString("yyyy/MM/dd\nhh:ss:ss") : _piece.message.dateCreate.ToString("d");
        public bool Unread { get => _unread; set => SetProperty(ref _unread, value); }
        public ICommand SetRead { get; }
        public ICommand SetUnread { get; }
        public ICommand Delete { get; }
        public MessageItem(MessagePiece p)
        {
            _piece = p;
            _unread = _piece.hasReaded == "N";
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
