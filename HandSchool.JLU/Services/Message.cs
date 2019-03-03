using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.Models;
using HandSchool.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandSchool.JLU.Services
{
    [Entrance("JLU", "系统收件箱", "提供了UIMS的收件箱功能，可以查看成绩发布通知等。")]
    internal sealed class MessageEntrance : IMessageEntrance
    {
        const string getMessageUrl = "siteMessages/get-message-in-box.do";
        const string messageReadUrl = "siteMessages/read-message.do";
        const string messageDeleteUrl = "siteMessages/delete-recv-message.do";

        private ISchoolSystem Connection { get; }

        public MessageEntrance(ISchoolSystem connection)
        {
            Connection = connection;
        }
        
        public async Task<IEnumerable<IMessageItem>> ExecuteAsync()
        {
            try
            {
                var lastReport = await Connection.Post(getMessageUrl, "{}");
                var ro = lastReport.ParseJSON<MessageBox>();
                return from asv in ro.items select new MessageItem(asv);
            }
            catch (WebsException ex)
            {
                throw new ServiceException(ex.Status.ToDescription(), ex);
            }
            catch (JsonException ex)
            {
                throw new ServiceException("数据解析出错。", ex);
            }
        }
        
        public async Task SetReadState(int id, bool read)
        {
            try
            {
                var postArgs = "{\"read\":\"" + (read ? "Y" : "N") + "\",\"idList\":[\"" + id + "\"]}";
                await Connection.Post(messageReadUrl, postArgs);
            }
            catch (WebsException ex)
            {
                throw new ServiceException(ex.Status.ToDescription(), ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await Connection.Post(messageDeleteUrl, $"{{\"idList\":[\"{id}\"]}}");
            }
            catch (WebsException ex)
            {
                throw new ServiceException(ex.Status.ToDescription(), ex);
            }
        }
    }
}