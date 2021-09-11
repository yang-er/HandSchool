using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
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
        private readonly MessagePiece piece;
        private bool _unread;
        public Xamarin.Forms.Color ReadState => Unread ? Xamarin.Forms.Color.Red : Xamarin.Forms.Color.Gray;
        public int Id => int.Parse(piece.msgInboxId);
        public string Title => piece.message.title;
        public string Body => piece.message.body;
        public string Detail => piece.message.body;
        public DateTime Time => piece.message.dateCreate;
        public string Sender => (piece.message.sender is null ? "系统" : piece.message.sender.name);
        public string Date => Core.Platform.RuntimeName == "Android" ? piece.message.dateCreate.ToString("yyyy/MM/dd\nhh:ss:ss") : piece.message.dateCreate.ToString("d");
        public bool Unread { get => _unread; set => SetProperty(ref _unread, value); }
        public ICommand SetRead { get; }
        public ICommand SetUnread { get; }
        public ICommand Delete { get; }
        public ICommand ItemLongPressCommand { get;}
        public ICommand ItemTappedCommand { get; }

        public MessageItem(MessagePiece p)
        {
            piece = p;
            _unread = piece.hasReaded == "N";
            SetRead = new CommandAction(SetReadAsync);
            SetUnread = new CommandAction(SetUnreadAsync);
            Delete = new CommandAction(DeleteAsync);
            ItemLongPressCommand = new CommandAction(ItemLongPress);
            ItemTappedCommand = new CommandAction(ItemTapped);
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

        private async Task ItemLongPress()
        {
            string[] opts = new string[] { "设为已读", "设为未读", "删除" };
            var opt = await MessageViewModel.Instance.RequestActionAsync("你要将此消息", "取消", null,opts);
            if (string.IsNullOrEmpty(opt)) return;
            if(opt == opts[0])
            {
                SetRead.Execute(null);
            }
            else if(opt == opts[1])
            {
                SetUnread.Execute(null);
            }
            else if(opt == opts[2])
            {
                Delete.Execute(null);
            }
        }
        private async Task ItemTapped()
        {
            if (this == null || MessagePage.Instance.IsPushing) return;
            MessagePage.Instance.IsPushing = true;
            SetRead.Execute(null);
            await MessagePage.Instance.Navigation.PushAsync<DetailPage>(this);
            MessagePage.Instance.IsPushing = false;
        }
    }
}
