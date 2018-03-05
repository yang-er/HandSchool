using HandSchool.Services.Drcom;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;

namespace HandSchool.Services
{
    class DrcomSocket : INetSocket
    {
        DrAlive controller = new DrAlive();
        DrProtocol protocol = new DrProtocol();
        DrCert cert;
        Thread pThread;

        public void LoginAndKeepAlive()
        {
            pThread = new Thread(controller.ThreadProcess);
            pThread.Start(protocol);
        }

        public void Logout()
        {
            pThread.Abort("logout");
        }

        public void SaveCredential(Dictionary<string, string> infomation)
        {
            protocol.Cert = new DrCert(infomation["username"], infomation["password"], PhysicalAddress.Parse(infomation["mac"]).GetAddressBytes(), controller.Log);
        }
    }
}
