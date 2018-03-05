using System.Text;
using System.Threading;

namespace HandSchool.Services.Drcom
{
    public class DrAlive
    {

        DrProtocol proc = new DrProtocol();

        public event DrDelegate OnLog;
        public event DrDelegate OnIpUpdated;
        public event DrStateDelegate OnStateChanged;
        
        int trytimes = 0;
        
        public void ThreadProcess(object args)
        {
            OnStateChanged.Invoke(true, "");
            try
            {
                proc = (DrProtocol)args;
                proc.Initialize();
                int ret = 0;
                trytimes = 5;
                while (true)
                {
                    if (trytimes >= 0 && ret++ > trytimes)
                    {
                        Log("login", "! try over times, login fail!", false);
                        throw new DrException() { Source = "登录失败次数过多。" };
                    }

                    int p = proc.Challenge(ret);
                    if (p == -5) throw new DrException() { Source = proc.InnerSource };
                    if (p < 0) continue;

                    p = proc.Login();
                    if (p == -5) throw new DrException() { Source = proc.InnerSource };
                    if (p < 0) continue;

                    OnIpUpdated.Invoke(proc.Cert.ClientIP);
                    break;
                }
                while (true)
                {
                    ret = 0;
                    int p;
                    while ((p = proc.Alive()) != 0)
                    {
                        if (p == -5) throw new DrException() { Source = proc.InnerSource };
                        if (trytimes >= 0 && ret++ > trytimes)
                        {
                            Log("alive", "alive(): fail;", false);
                            throw new DrException() { Source = "Keep-alive包发送超时多次。" };
                        }
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(20000);
                }
            }
            catch (ThreadAbortException)
            {
                Log("logout", "logging out...", false);
                OnStateChanged.Invoke(false, "");
            }
            catch (DrException e)
            {
                Log("drcom", "Socket closed. Please redail. ", false);
                OnStateChanged.Invoke(false, e.Source);
            }
        }

        public void Log(string app, object args, bool toHex)
        {
            if (OnLog.GetInvocationList().Length == 0)
                return;
            string ept;
            if (toHex)
            {
                char[] chars = "0123456789ABCDEF".ToCharArray();
                byte[] bs = (byte[])args;
                StringBuilder sb = new StringBuilder("");
                int bit;
                for (int i = 0; i < bs.Length; i++)
                {
                    bit = (bs[i] & 0x0f0) >> 4;
                    sb.Append(chars[bit]);
                    bit = bs[i] & 0x0f;
                    sb.Append(chars[bit]);
                }
                ept = sb.ToString();
                sb.Clear();
            }
            else
            {
                ept = (string)args;
            }
            OnLog.Invoke(string.Format("[{0}] {1}", app, ept));
        }
        
        public delegate void DrDelegate(string str);
        public delegate void DrStateDelegate(bool isLogin, string tip = "");
        // notifyIcon1.ShowBalloonTip(10000, "校园网", "已登出。" + toolTip, toolTip.Length == 0 ? ToolTipIcon.Info : ToolTipIcon.Error);

    }
}
