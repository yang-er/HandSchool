using System;
using System.Collections.Generic;

namespace HandSchool
{
    public interface INetSocket<T>
    {
        T Protocol { get; set; }
        void SaveCredential(Dictionary<string, string> infomation);
        void LoginAndKeepAlive();
        void Logout();
    }

    namespace Internal
    {
        public delegate void NetSocketLogDelegate(string str);
        public delegate void NetSocketStateDelegate(bool isLogin, string tip = "");
        // notifyIcon1.ShowBalloonTip(10000, "校园网", "已登出。" + toolTip, toolTip.Length == 0 ? ToolTipIcon.Info : ToolTipIcon.Error);
        public class NetSocketException : Exception { }
    }
}
