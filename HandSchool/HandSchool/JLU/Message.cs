using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class MessageItem : IMessageItem
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
        public event PropertyChangedEventHandler PropertyChanged;

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
        
        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value)) return;
            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class MessageEntrance : IMessageEntrance
    {
        public string Name => "系统收件箱";

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
            await Task.Run(() => { });
            /*
            LastReport = "{\"count\":2,\"errno\":0,\"identifier\":\"msgInboxId\",\"items\":" +
                "[{\"message\":{\"sender\":{\"name\":\"田苗\"},\"body\":\"this is test msg2<br>\"," +
                "\"title\":\"testmsg2\",\"messageId\":\"243467\",\"dateCreate\":\"2018-04-14T18:50:39\"}," +
                "\"msgInboxId\":\"9055099\",\"receiver\":{\"school\":{\"schoolName\":\"软件学院\"},\"name\":\"张煜松\"}," +
                "\"dateRead\":null,\"activeStatus\":\"103\",\"hasReaded\":\"N\",\"dateReceive\":null},{\"message\"" +
                ":{\"sender\":{\"name\":\"田苗\"},\"body\":\"this is a test message<br >\",\"title\":\"test message\"," +
                "\"messageId\":\"243465\",\"dateCreate\":\"2018-04-13T23:52:14\"},\"msgInboxId\":\"9055097\",\"receiver\":" +
                "{\"school\":{\"schoolName\":\"软件学院\"},\"name\":\"张煜松\"},\"dateRead\":null,\"activeStatus\":\"103\"," +
                "\"hasReaded\":\"N\",\"dateReceive\":null}],\"label\":\"\",\"msg\":\"\",\"status\":0}";
                */
            WriteConfFile(StorageFile, LastReport);
            Parse();
        }

        public void Parse()
        {
            var ro = JSON<MessageBox>(LastReport);
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
