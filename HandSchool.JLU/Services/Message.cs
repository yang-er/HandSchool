﻿using HandSchool.Internals;
using HandSchool.JLU.JsonObject;
using HandSchool.JLU.Models;
using HandSchool.JLU.Services;
using HandSchool.Services;
using HandSchool.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using HandSchool.Models;

[assembly: RegisterService(typeof(MessageEntrance))]
namespace HandSchool.JLU.Services
{
    [Entrance("JLU", "系统收件箱", "提供了UIMS的收件箱功能，可以查看成绩发布通知等。")]
    [UseStorage("JLU")]
    internal sealed class MessageEntrance : IMessageEntrance
    {
        const string getMessageUrl = "siteMessages/get-message-in-box.do";
        const string messageReadUrl = "siteMessages/read-message.do";
        const string messageDeleteUrl = "siteMessages/delete-recv-message.do";
        
        public async Task Execute()
        {
            try
            {
                var lastReport = await Core.App.Service.Post(getMessageUrl, "{}");
                var ro = lastReport.ParseJSON<MessageBox>();
                Core.Platform.EnsureOnMainThread(() =>
                {
                    MessageViewModel.Instance.Clear();
                    MessageViewModel.Instance.AddRange(from asv in ro.items select new MessageItem(asv));
                });
            }
            catch (WebsException ex)
            {
                await MessageViewModel.Instance.RequestMessageAsync("错误", ex.Status.ToDescription() + "。");
            }
        }
        
        public async Task SetReadState(int id, bool read)
        {
            try
            {
                var postArgs = "{\"read\":\"" + (read ? "Y" : "N") + "\",\"idList\":[\"" + id + "\"]}";
                await Core.App.Service.Post(messageReadUrl, postArgs);
            }
            catch (WebsException ex)
            {
                await MessageViewModel.Instance.RequestMessageAsync("错误", ex.Status.ToDescription() + "。");
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await Core.App.Service.Post(messageDeleteUrl, $"{{\"idList\":[\"{id}\"]}}");
            }
            catch (WebsException ex)
            {
                await MessageViewModel.Instance.RequestMessageAsync("错误", ex.Status.ToDescription() + "。");
            }
        }
    }
}