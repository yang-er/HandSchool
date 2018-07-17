using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using static HandSchool.Internal.Helper;

namespace HandSchool.Internal
{
    /// <summary>
    /// DrCOM D版协议
    /// </summary>
    public class DrcomProtocol
    {
        // Protocol
        public bool IsWireless = true;
        public event DrcomLogger Logrotate;
        public string InnerSource { get; set; }
        
        // User Infomation
        public PhysicalAddress MAC { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public string HostOS { get; set; }
        public IPAddress ClientIP { get; set; }

        // Server Infomatiom
        public IPAddress PrimaryDNS { get; set; }
        public IPAddress DefaultDHCP { get; set; }
        public IPAddress SecondaryDNS { get; set; }
        public IPAddress ServerIP { get; set; }
        
        // Client
        byte[] md5a;
        byte[] salt = new byte[4];
        byte[] tail = new byte[16];
        byte[] tail2 = new byte[4];
        byte[] flux = new byte[4];
        byte[] buffer;
        IPEndPoint ep;
        int randtimes;
        Random rand = new Random();
        Socket socket;
        
        public bool Initialize()
        {
            try
            {
                InnerSource = "";
                ep = new IPEndPoint(ServerIP, 61440);
                ClientIP = IPAddress.Parse("0.0.0.0");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect(ep);
                Logrotate.Invoke("socket", "Initialized with " + socket.Connected.ToString(), false);
                return socket.Connected;
            }
            catch (SocketException)
            {
                Logrotate.Invoke("socket", "invalid socket, exitting...", false);
                InnerSource = "Socket初始化失败。";
                return false;
            }
        }
        
        #region Make Packet

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
            Logrotate.Invoke("challenge", data, true);
#else
            Logrotate.Invoke("challenge", "Making challenge packet...", false);
#endif
        }

        public byte[] MakeLoginPacket(byte[] salt, out byte[] data, out int len)
        {
            int pwlen = Password.Length;
            byte[] mac = MAC.GetAddressBytes();
            byte[] md5_src;
            data = new byte[400];

            int i = 0;

            // Begin
            data[i++] = 0x03;
            data[i++] = 0x01;
            data[i++] = 0x00;
            data[i++] = (byte)(Username.Length + 20);

            // MD5 A
            md5_src = new byte[6 + pwlen];
            md5_src[0] = 3;
            md5_src[1] = 1;
            Buffer.BlockCopy(salt, 0, md5_src, 2, 4);
            Buffer.BlockCopy(Encoding.Default.GetBytes(Password), 0, md5_src, 6, pwlen);
            md5a = MD5(md5_src);
            Buffer.BlockCopy(md5a, 0, data, 4, 16);
            i += 16;

            // Username
            Buffer.BlockCopy(Encoding.Default.GetBytes(Username), 0, data, i, Username.Length);
            i += Username.Length > 36 ? Username.Length : 36;

            // CONTROLCHECKSTATUS ADAPTERNUM
            data[i++] = 0x20;
            data[i++] = 0x03;

            /* (data[4:10].encode('hex'),16)^mac */
            Buffer.BlockCopy(md5a, 0, data, i, 6);
            for (int j = 0; j < 6; j++)
                data[i + j] ^= mac[j];
            i += 6;

            // MD5 B
            md5_src = new byte[9 + pwlen];
            md5_src[0] = 1;
            Buffer.BlockCopy(Encoding.Default.GetBytes(Password), 0, md5_src, 1, pwlen);
            Buffer.BlockCopy(salt, 0, md5_src, 1 + pwlen, 4);
            Buffer.BlockCopy(MD5(md5_src), 0, data, i, 16);
            i += 16;

            // Ip number, 1, 2, 3, 4
            data[i++] = 0x01;
            Buffer.BlockCopy(ClientIP.GetAddressBytes(), 0, data, i, 4);
            i += 16;

            // MD5 C
            md5_src = new byte[i + 4];
            Buffer.BlockCopy(data, 0, md5_src, 0, i);
            md5_src[i + 0] = 0x14;
            md5_src[i + 1] = 0x00;
            md5_src[i + 2] = 0x07;
            md5_src[i + 3] = 0x0b;
            Buffer.BlockCopy(MD5(md5_src), 0, data, i, 8);
            i += 8;

            // IPDOG(0x01) 0x00*4
            data[i++] = 0x01;
            i += 4;

            // Host Name
            md5_src = Encoding.Default.GetBytes(HostName);
            Buffer.BlockCopy(md5_src, 0, data, i, md5_src.Length > 32 ? 32 : md5_src.Length);
            i += 32;

            // primary dns
            Buffer.BlockCopy(PrimaryDNS.GetAddressBytes(), 0, data, i, 4);
            i += 4;

            // DHCP Server
            Buffer.BlockCopy(DefaultDHCP.GetAddressBytes(), 0, data, i, 4);
            i += 4;

            // secondary dns
            Buffer.BlockCopy(SecondaryDNS.GetAddressBytes(), 0, data, i, 4);
            i += 4;

            // delimeter
            i += 8;

            // unknown os_major os_minor OS_build os_unknown
            data[i++] = 0x94;
            i += 3;
            data[i++] = 0x06;
            i += 3;
            data[i++] = 0x02;
            i += 3;
            data[i++] = 0xf0;
            data[i++] = 0x23;
            i += 2;
            data[i++] = 0x02;
            i += 3;

            // unknown string
            data[i++] = 0x44;
            data[i++] = 0x72;
            data[i++] = 0x43;
            data[i++] = 0x4f;
            data[i++] = 0x4d;
            i++;
            data[i++] = 0xcf;
            data[i++] = 0x07;
            data[i++] = 0x68;
            i += 55;

            // unknown string 2
            data[i++] = 0x33;
            data[i++] = 0x64;
            data[i++] = 0x63;
            data[i++] = 0x37;
            data[i++] = 0x39;
            data[i++] = 0x66;
            data[i++] = 0x35;
            data[i++] = 0x32;
            data[i++] = 0x31;
            data[i++] = 0x32;
            data[i++] = 0x65;
            data[i++] = 0x38;
            data[i++] = 0x31;
            data[i++] = 0x37;
            data[i++] = 0x30;
            data[i++] = 0x61;
            data[i++] = 0x63;
            data[i++] = 0x66;
            data[i++] = 0x61;
            data[i++] = 0x39;
            data[i++] = 0x65;
            data[i++] = 0x63;
            data[i++] = 0x39;
            data[i++] = 0x35;
            data[i++] = 0x66;
            data[i++] = 0x31;
            data[i++] = 0x64;
            data[i++] = 0x37;
            data[i++] = 0x34;
            data[i++] = 0x39;
            data[i++] = 0x31;
            data[i++] = 0x36;
            data[i++] = 0x35;
            data[i++] = 0x34;
            data[i++] = 0x32;
            data[i++] = 0x62;
            data[i++] = 0x65;
            data[i++] = 0x37;
            data[i++] = 0x62;
            data[i++] = 0x31;
            i += 24;

            // auth version
            data[i++] = 0x68;
            data[i++] = 0x00;

            data[i++] = 0x00;
            data[i++] = (byte)(pwlen);

            // ror password
            for (int k = 0, x; k < pwlen; k++)
            {
                x = md5a[k] ^ Password[k];
                data[i++] = (byte)(((x << 3) & 0xFF) + (x >> 5));
            }

            data[i++] = 0x02;
            data[i++] = 0x0c;

            // checksum point
            int check_point = i;
            data[i++] = 0x01;
            data[i++] = 0x26;
            data[i++] = 0x07;
            data[i++] = 0x11;

            // 0x00 0x00 mac
            i += 2;
            Buffer.BlockCopy(mac, 0, data, i, 6);
            i += 6;

            // checksum
            ulong sum = 1234;
            ulong check = 0;
            for (int k = 0; k < i; k += 4)
            {
                check = 0;
                for (int j = 0; j < 4; j++)
                {
                    check = (check << 2) + data[k + j];
                }
                sum ^= check;
            }
            sum = (1968 * sum) & 0xFFFFFFFF;
            for (int j = 0; j < 4; j++)
            {
                data[check_point + j] = (byte)(sum >> (j * 8) & 0x000000FF);
            }


            if (pwlen / 4 != 4)
                i += (pwlen / 4);
            i += 4;
            data[i++] = 0x60;
            data[i++] = 0xa2;
            i += 28;
            len = i;

#if DEBUG
            Logrotate.Invoke("mkpkt", data, true);
#else
            Logrotate.Invoke("login", "Making login packet...", false);
#endif
            return data;
        }

        public void MakeKeepAlivePacket(out byte[] data, int type, int ran)
        {
            if (type == 0)
            {
                data = new byte[38];
                data[0] = 0xff;
                Buffer.BlockCopy(md5a, 0, data, 1, 16);
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
                    Buffer.BlockCopy(ClientIP.GetAddressBytes(), 0, data, 28, 4);
            }
#if DEBUG
            Logrotate.Invoke("alive", data, true);
#else
            Logrotate.Invoke("alive", "Sending keep-alive-" + type.ToString() + " packet...", false);
#endif
        }

        #endregion

        #region Exchange Packet

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
                            Logrotate.Invoke("recv", "failed to connect to remote host", false);
                        }
                        else
                            throw e;
                    }
                    if (ret < 0)
                    {
                        Logrotate.Invoke("recv", "recv() failed", false);
                        return -2;
                    }

                    if (buffer[0] == 0x4D)
                    {
                        if (buffer[1] == 0x15)
                        {
                            Logrotate.Invoke("login", "! Others logined.", false);
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
        
        public void CloseSocket()
        {
            socket.Close();
        }

        #endregion

        #region Connect Function

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
                    Logrotate.Invoke("alive", "Alive(" + i + ") timeout.", false);
                    return -1;
                }

                if (buffer[0] != 0x07)
                {
                    Logrotate.Invoke("alive", string.Format("Alive fail, unrecognized responese: {0:x}", buffer[0]), false);
                    return -1;
                }

                if (i > 0)
                {
                    Buffer.BlockCopy(buffer, 16, flux, 0, 4);
                }

            }
            return 0;
        }
        
        public int Login()
        {
            MakeLoginPacket(salt, out buffer, out int len);
            SendPacket(buffer, len);
            int recv_len = ReceivePacket();
            if (recv_len == -5) return -5;
            if (recv_len <= 0)
            {
                Logrotate.Invoke("login", "Login timeout.", false);
                return -1;
            }

            if (buffer[0] != 0x04)
            {
                if (buffer[0] == 0x05)
                {
                    Logrotate.Invoke("login", "Login fail, wrong password or username!", false);
                }
                else
                {
                    Logrotate.Invoke("login", string.Format("Login fail, unrecognized responese: {0:x}", buffer[0]), false);
                }
                CloseSocket();
                return -1;
            }

            Buffer.BlockCopy(buffer, 0x17, tail, 0, 16);
            Logrotate.Invoke("login", "Login success!", false);
            return 0;
        }
        
        public int Challenge(int times)
        {
            buffer = new byte[20];
            buffer[0] = 0x01;
            buffer[1] = (byte)(0x02 + times);
            buffer[2] = (byte)rand.Next(0, 255);
            buffer[3] = (byte)rand.Next(0, 255);
            buffer[4] = 0x68;
#if DEBUG
            Logrotate.Invoke("challenge", buffer, true);
#else
            Logrotate.Invoke("challenge", "Sending challenge packet...", false);
#endif
            SendPacket(buffer, 20);
            int recv_len = ReceivePacket();
            if (recv_len == -5) return -5;
            if (recv_len <= 0)
            {
                Logrotate.Invoke("challenge", "Challenge timeout.", false);
                return -1;
            }

            if (buffer[0] != 0x02)
            {
                Logrotate.Invoke("challenge", string.Format("Challenge fail, unrecognized responese: {0:x}", buffer[0]), false);
                return -1;
            }

            Buffer.BlockCopy(buffer, 4, salt, 0, 4);
            Logrotate.Invoke("login-salt", salt, true);

            if (IsWireless && recv_len >= 25)
            {
                byte[] host_ip = new byte[4];
                Buffer.BlockCopy(buffer, 20, host_ip, 0, 4);
                ClientIP = new IPAddress(host_ip);
                Logrotate.Invoke("login-ip", ClientIP.ToString(), false);
            }

            return 0;
        }

        #endregion

    }

    /// <summary>
    /// 记录DrCOM日志的委托
    /// </summary>
    /// <param name="app">程序部分</param>
    /// <param name="args">参数</param>
    /// <param name="toHex">转为16进制</param>
    public delegate void DrcomLogger(string app, object args, bool toHex);
}
