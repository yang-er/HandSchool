using HandSchool.JLU.JsonObject;
using HandSchool.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    public class MessageItem : IMessageItem
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

        public MessageItem(MessagePiece p)
        {
            piece = p;
            _unread = piece.hasReaded == "N";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value)) return;
            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MessageEntrance : IMessageEntrance
    {
        public string Name => "系统收件箱";

        public string ScriptFileUri => "siteMessages/get-message-in-box.do";
        public string MsgReadPageUri => "siteMessages/read-message.do";
        public string PostValue => "{}";
        public bool IsPost => true;
        public string StorageFile => "jlu.msgbox.json";
        public string LastReport { get; private set; }
        
        public async Task Execute()
        {
            LastReport = await App.Current.Service.Post(ScriptFileUri, PostValue);
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
            await App.Current.Service.Post(MsgReadPageUri, PostArgs);
            // throw new NotImplementedException();
        }
    }
}
