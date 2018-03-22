using HandSchool.JLU.JsonObject;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    public class MessageItem : IMessageItem
    {
        private MessagePiece piece;

        public int Id => int.Parse(piece.msgInboxId);
        public string Title => piece.message.title;
        public string Body => piece.message.body;
        public DateTime Time => piece.message.dateCreate;
        public bool Readed => (piece.hasReaded != "N");
        public string Show => $"{Time.ToString()} {Body}";
        public string MsgReadPageUri = "http://uims.jlu.edu.cn/ntms/siteMessages/read-message.do";
        public string PostValue = "{\"read\":\"Y\",\"idList\":[\"";     //+Idstring+"\"]}";
        public Command testcommand { get; set; }
        public async void Onreaded()
        {
            string a=await Execute();
        }
        public async Task<string>Execute()
        {
                PostValue += Id.ToString() + "\"]}";
                await App.Current.Service.Post(MsgReadPageUri, PostValue);                     //异步执行一些任务
                return "Hello World";                               //异步执行完成标记
        }
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
