using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

[assembly: RegisterService(typeof(MessageEntrance))]
namespace HandSchool.JLU.Services
{
    [Entrance("JLU", "系统收件箱", "提供了UIMS的收件箱功能，可以查看成绩发布通知等。")]
    [UseStorage("JLU", configMsgBox)]
    internal sealed class MessageEntrance : IMessageEntrance
    {
        const string configMsgBox = "jlu.msgbox.json";

        const string getMessageUrl = "siteMessages/get-message-in-box.do";
        const string messageReadUrl = "siteMessages/read-message.do";
        const string messageDeleteUrl = "siteMessages/delete-recv-message.do";
        
        public async Task Execute()
        {
            try
            {
                var lastReport = await Core.App.Service.Post(getMessageUrl, "{}");
                Core.Configure.Write(configMsgBox, lastReport);
                var ro = lastReport.ParseJSON<MessageBox>();
                MessageViewModel.Instance.Clear();
                MessageViewModel.Instance.AddRange(from asv in ro.items select new MessageItem(asv));
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout) throw;
                await MessageViewModel.Instance.ShowTimeoutMessage();
            }
        }
        
        public async Task SetReadState(int id, bool read)
        {
            try
            {
                var postArgs = "{\"read\":\"" + (read ? "Y" : "N") + "\",\"idList\":[\"" + id + "\"]}";
                await Core.App.Service.Post(messageReadUrl, postArgs);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout) throw;
                await MessageViewModel.Instance.ShowTimeoutMessage();
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await Core.App.Service.Post(messageDeleteUrl, $"{{\"idList\":[\"{id}\"]}}");
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout) throw;
                await MessageViewModel.Instance.ShowTimeoutMessage();
            }
        }
    }
}
