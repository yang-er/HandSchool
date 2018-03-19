using HandSchool.JLU.JsonObject;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    public class MessageItem : IMessageItem
    {
        private MessagePiece piece;

        public int Id => int.Parse(piece.message.messageId);
        public string Title => piece.message.title;
        public string Body => piece.message.body;
        public DateTime Time => piece.message.dateCreate;
        public bool Readed => (piece.hasReaded != "N");
        public string Show => $"{Time.ToString()} {Body}";

        public MessageItem(MessagePiece p)
        {
            piece = p;
        }
    }

    public class MessageEntrance : ISystemEntrance
    {
        public string Name => "系统收件箱";

        public string ScriptFileUri => "siteMessages/get-message-in-box.do";
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
    }
}
