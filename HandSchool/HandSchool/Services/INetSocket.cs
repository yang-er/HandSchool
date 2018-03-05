using System.Collections.Generic;

namespace HandSchool.Services
{
    interface INetSocket
    {
        void SaveCredential(Dictionary<string, string> infomation);
        void LoginAndKeepAlive();
        void Logout();
    }
}
