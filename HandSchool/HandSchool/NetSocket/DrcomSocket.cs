using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using static HandSchool.Internal.Helper;

namespace HandSchool.Internal
{
    public class DrcomSocket : INetSocket
    {
        public DrcomProtocol Protocol { get; set; }
        public Thread Thread;

        public void LoginAndKeepAlive()
        {
            if (Thread != null && Thread.IsAlive)
                throw new InvalidOperationException();
            Thread = new Thread(ThreadProcess);
            Thread.Start();
        }

        public void Logout()
        {
            Thread.Abort("logout");
        }

        public virtual void SaveCredential(Dictionary<string, string> infomation)
        {
            Protocol = new DrcomProtocol
            {
                Username = infomation["username"],
                Password = infomation["password"],
                MAC = PhysicalAddress.Parse(infomation["mac"])
            };
            Protocol.Logrotate += Log;
        }
        
        public event NetSocketLogDelegate Logrotate;
        public event NetSocketLogDelegate IpUpdated;
        public event NetSocketStateDelegate StateChanged;

        int trytimes = 0;

        private void ThreadProcess()
        {
            StateChanged.Invoke(true, "");
            try
            {
                Protocol.Initialize();
                int ret = 0;
                trytimes = 5;
                while (true)
                {
                    if (trytimes >= 0 && ret++ > trytimes)
                    {
                        Log("login", "! try over times, login fail!", false);
                        throw new NetSocketException() { Source = "登录失败次数过多。" };
                    }

                    int p = Protocol.Challenge(ret);
                    if (p == -5) throw new NetSocketException() { Source = Protocol.InnerSource };
                    if (p < 0) continue;

                    p = Protocol.Login();
                    if (p == -5) throw new NetSocketException() { Source = Protocol.InnerSource };
                    if (p < 0) continue;

                    IpUpdated.Invoke(Protocol.ClientIP.ToString());
                    break;
                }

                while (true)
                {
                    ret = 0;
                    int p;
                    while ((p = Protocol.Alive()) != 0)
                    {
                        if (p == -5) throw new NetSocketException() { Source = Protocol.InnerSource };
                        if (trytimes >= 0 && ret++ > trytimes)
                        {
                            Log("alive", "alive(): fail;", false);
                            throw new NetSocketException() { Source = "Keep-alive包发送超时多次。" };
                        }
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(20000);
                }
            }
            catch (ThreadAbortException)
            {
                Log("logout", "logging out...", false);
                StateChanged.Invoke(false, "");
            }
            catch (NetSocketException e)
            {
                Log("drcom", "Socket closed. Please redail. ", false);
                StateChanged.Invoke(false, e.Source);
            }
        }

        private void Log(string app, object args, bool toHex)
        {
            if (Logrotate.GetInvocationList().Length == 0)
                return;
            string ept;
            if (toHex)
            {
                ept = HexDigest((byte[])args);
            }
            else
            {
                ept = (string)args;
            }
            Logrotate.Invoke(string.Format("[{0}] {1}", app, ept));
        }
    }
}
