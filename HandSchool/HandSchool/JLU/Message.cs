using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using HandSchool.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.JLU
{
    class MessageItem : NotifyPropertyChanged, IMessageItem
    {
        private MessagePiece piece;
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

            SetRead = new Command(async () => {
                await Core.App.Message.SetReadState(Id, true);
                Unread = false;
            });

            SetUnread = new Command(async () => {
                await Core.App.Message.SetReadState(Id, false);
                Unread = true;
            });

            Delete = new Command(async () => {
                await Core.App.Message.Delete(Id);
                MessageViewModel.Instance.Items.Remove(this);
            });
        }
    }

    [Entrance("系统收件箱")]
    class MessageEntrance : IMessageEntrance
    {
        public string ScriptFileUri => "siteMessages/get-message-in-box.do";
        public string MsgReadPageUri => "siteMessages/read-message.do";
        public string DelPageUri => "siteMessages/delete-recv-message.do";
        public string PostValue => "{}";
        public bool IsPost => true;
        public string StorageFile => "jlu.msgbox.json";
        public string LastReport { get; private set; }
        
        public async Task Execute()
        {
            LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
            Core.WriteConfig(StorageFile, LastReport);
            Parse();
        }

        public void Parse()
        {
            var ro = LastReport.ParseJSON<MessageBox>();
            MessageViewModel.Instance.Items.Clear();
            foreach (var asv in ro.items)
            {
                MessageViewModel.Instance.Items.Add(new MessageItem(asv));
            }
        }

        public async Task SetReadState(int id, bool read)
        {
            var PostArgs = "{\"read\":\"" + (read ? "Y" : "N") + "\",\"idList\":[\"" + id.ToString() + "\"]}";
            await Core.App.Service.Post(MsgReadPageUri, PostArgs);
        }

        public async Task Delete(int id)
        {
            var PostArgs = "{\"idList\":[\""+id.ToString()+"\"]}";
            await Core.App.Service.Post(DelPageUri, PostArgs);
        }
    }
}
