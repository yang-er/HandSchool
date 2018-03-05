using System;
using System.Net;
using System.Net.Sockets;

namespace HandSchool.Services.Drcom
{

    /// <summary>
    /// 构成 Drcom 连接协议
    /// </summary>
    public class DrProtocol
    {

        /// <summary>
        /// 绑定日志输出
        /// </summary>
        public event DrLogger OnMakeLog;

        public DrCert Cert { get; set; }

        byte[] salt = new byte[4];
        byte[] md5a = new byte[16];
        byte[] tail = new byte[16];
        byte[] tail2 = new byte[4];
        byte[] flux = new byte[4];
        byte[] buffer;
        bool isWireless = true;
        IPEndPoint ep;
        int randtimes;
        Random rand = new Random();
        // int alivesum = 0;
        Socket socket;
        public string InnerSource { get; set; }

        void NullVoid(string a, object b, bool c) { }

        /// <summary>
        /// 初始化
        /// </summary>
        public bool Initialize()
        {
            try
            {
                InnerSource = "";
                ep = new IPEndPoint(0x033d640a, 61440);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect(ep);
                OnMakeLog.Invoke("socket", "Initialized with " + socket.Connected.ToString(), false);
                return socket.Connected;
            }
            catch (SocketException)
            {
                OnMakeLog.Invoke("socket", "invalid socket, exitting...", false);
                InnerSource = "Socket初始化失败。";
                return false;
            }
        }

        /// <summary>
        /// 输出一个 Challenge 用的数据包
        /// </summary>
        public void MakeChallengePacket(out byte[] data, int len, int trial)
        {
            /* set challenge */
            int random = rand.Next(0, 32767) % 0xF0 + 0xF;
            int i = 0;
            data = new byte[len];
            /* 0x01 challenge request */
            data[i++] = 0x01;
            /* clg_try_count first 0x02, then increment */
            data[i++] = (byte)(0x02 + trial);
            /* two byte of challenge_data */
            data[i++] = (byte)(random % 0xFFFF);
            data[i++] = (byte)((random % 0xFFFF) >> 8);
            /* end with 0x09 */
            data[i++] = 0x09;
#if DEBUG
            OnMakeLog.Invoke("challenge", data, true);
#else
            OnMakeLog.Invoke("challenge", "Making challenge packet...", false);
#endif
        }

        /// <summary>
        /// 输出一个 Keep-Alive 用的数据包
        /// </summary>
        public void MakeKeepAlivePacket(out byte[] data, int type, int ran)
        {
            if (type == 0)
            {
                data = new byte[38];
                data[0] = 0xff;
                Buffer.BlockCopy(Cert.MD5A, 0, data, 1, 16);
                Buffer.BlockCopy(tail, 0, data, 20, 16);
                data[36] = (byte)(ran >> 8);
                data[37] = (byte)(ran & 0xFFFFFFFF);
            }
            else
            {
                data = new byte[40];
                data[0] = 0x07;
                data[1] = 0x00;
                data[2] = 0x28;
                data[4] = 0x0b;
                data[5] = (byte)(2 * type - 1);
                // keepaliveVer
                data[6] = 0xdc;
                data[7] = 0x02;
                data[9] = (byte)(ran >> 8);
                data[10] = (byte)(ran);
                Buffer.BlockCopy(flux, 0, data, 16, 4);
                if (type == 2)
                    Buffer.BlockCopy(Cert.host_ip, 0, data, 28, 4);
            }
#if DEBUG
            OnMakeLog.Invoke("alive", data, true);
#else
            OnMakeLog.Invoke("alive", "Sending keep-alive-" + type.ToString() + " packet...", false);
#endif
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        public int SendPacket(byte[] buffer, int length)
        {
            try
            {
                return socket.Send(buffer, length, SocketFlags.None);
            }
            catch (ObjectDisposedException)
            {
                InnerSource = "Socket被异常关闭。";
                return -5;
            }
        }

        /// <summary>
        /// 接受数据包
        /// </summary>
        public int ReceivePacket()
        {
            try
            {
                int ret;
                buffer = new byte[1024];
                while (true)
                {
                    socket.ReceiveTimeout = 3000;
                    try
                    {
                        ret = socket.Receive(buffer, 1024, SocketFlags.None);
                    }
                    catch (SocketException e)
                    {
                        if (e.ErrorCode == 10060)
                        {
                            ret = -1;
                            OnMakeLog.Invoke("recv", "failed to connect to remote host", false);
                        }
                        else
                            throw e;
                    }
                    if (ret < 0)
                    {
                        OnMakeLog.Invoke("recv", "recv() failed", false);
                        return -2;
                    }

                    if (buffer[0] == 0x4D)
                    {
                        if (buffer[1] == 0x15)
                        {
                            OnMakeLog.Invoke("login", "! Others logined.", false);
                            InnerSource = "账户在其他地方登录。";
                            return -5;
                        }

                        continue;
                    }

                    break;
                }
                return ret;
            }
            catch (ObjectDisposedException)
            {
                InnerSource = "Socket被异常关闭。";
                return -5;
            }
        }

        /// <summary>
        /// 关闭 Socket
        /// </summary>
        public void CloseSocket()
        {
            socket.Close();
        }

        /// <summary>
        /// 发送 Keep-Alive 心跳包
        /// </summary>
        public int Alive()
        {
            randtimes = rand.Next(0, 32767) % 0xFFFF;
            for (int i = 0; i < 3; i++)
            {
                MakeKeepAlivePacket(out buffer, i, randtimes);
                SendPacket(buffer, i == 0 ? 38 : 40);
                int r = ReceivePacket();
                if (r == -5) return -5;
                if (r < 0)
                {
                    OnMakeLog.Invoke("alive", "Alive(" + i + ") timeout.", false);
                    return -1;
                }

                if (buffer[0] != 0x07)
                {
                    OnMakeLog.Invoke("alive", string.Format("Alive fail, unrecognized responese: {0:x}", buffer[0]), false);
                    return -1;
                }

                if (i > 0)
                {
                    Buffer.BlockCopy(buffer, 16, flux, 0, 4);
                }

            }
            return 0;
        }

        /// <summary>
        /// 发送 Login 数据包
        /// </summary>
        public int Login()
        {
            Cert.MakeLoginPacket(salt, out buffer, out int len);
            SendPacket(buffer, len);
            int recv_len = ReceivePacket();
            if (recv_len == -5) return -5;
            if (recv_len <= 0)
            {
                OnMakeLog.Invoke("login", "Login timeout.", false);
                return -1;
            }

            if (buffer[0] != 0x04)
            {
                if (buffer[0] == 0x05)
                {
                    OnMakeLog.Invoke("login", "Login fail, wrong password or username!", false);
                }
                else
                {
                    OnMakeLog.Invoke("login", string.Format("Login fail, unrecognized responese: {0:x}", buffer[0]), false);
                }
                CloseSocket();
                return -1;
            }

            Buffer.BlockCopy(buffer, 0x17, tail, 0, 16);
            OnMakeLog.Invoke("login", "Login success!", false);
            return 0;
        }

        /// <summary>
        /// 发送 Challenge 数据包
        /// </summary>
        public int Challenge(int times)
        {
            buffer = new byte[20];
            buffer[0] = 0x01;
            buffer[1] = (byte)(0x02 + times);
            buffer[2] = (byte)rand.Next(0, 255);
            buffer[3] = (byte)rand.Next(0, 255);
            buffer[4] = 0x68;
#if DEBUG
            OnMakeLog.Invoke("challenge", buffer, true);
#else
            OnMakeLog.Invoke("challenge", "Sending challenge packet...", false);
#endif
            SendPacket(buffer, 20);
            int recv_len = ReceivePacket();
            if (recv_len == -5) return -5;
            if (recv_len <= 0)
            {
                OnMakeLog.Invoke("challenge", "Challenge timeout.", false);
                return -1;
            }

            if (buffer[0] != 0x02)
            {
                OnMakeLog.Invoke("challenge", string.Format("Challenge fail, unrecognized responese: {0:x}", buffer[0]), false);
                return -1;
            }

            Buffer.BlockCopy(buffer, 4, salt, 0, 4);
            OnMakeLog.Invoke("login-salt", salt, true);

            if (isWireless && recv_len >= 25)
            {
                Buffer.BlockCopy(buffer, 20, Cert.host_ip, 0, 4);
                OnMakeLog.Invoke("login-ip", Cert.ClientIP, false);
            }

            return 0;
        }

    }

}
