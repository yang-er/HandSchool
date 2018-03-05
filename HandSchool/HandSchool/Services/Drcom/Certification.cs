using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace HandSchool.Services.Drcom
{

    public class DrMacAddress
    {

        public string Name { get; private set; }
        public PhysicalAddress MAC { get; private set; }

        public DrMacAddress(NetworkInterface i)
        {
            MAC = i.GetPhysicalAddress();
            Name = i.Name;
        }

        public DrMacAddress(string Nick, long Addr)
        {
            Name = Nick;
            byte[] p = new byte[6];
            p[5] = (byte)(Addr & 0xFF);
            Addr >>= 8;
            p[4] = (byte)(Addr & 0xFF);
            Addr >>= 8;
            p[3] = (byte)(Addr & 0xFF);
            Addr >>= 8;
            p[2] = (byte)(Addr & 0xFF);
            Addr >>= 8;
            p[1] = (byte)(Addr & 0xFF);
            Addr >>= 8;
            p[0] = (byte)(Addr & 0xFF);
            MAC = new PhysicalAddress(p);
        }

        public override string ToString()
        {
            return Name + " - " + MAC.ToString();
        }

    }

    public class DrException : Exception {}

    public delegate void DrLogger(string app, object args, bool toHex);

    /// <summary>
    /// Drcom 客户端认证内容处理
    /// </summary>
    public class DrCert
    {
        byte[] server = { 10, 100, 61, 3 };
        public byte[] host_ip = { 0, 0, 0, 0 };
        byte[] default_dns = { 10, 10, 10, 10 };
        byte[] default_dhcp = { 0, 0, 0, 0 };
        byte[] secondary_dns = { 202, 98, 18, 3 };
        MD5 md5 = new MD5CryptoServiceProvider();

        public byte[] MD5A;

        /// <summary>
        /// 客户机的以太网卡 Ethernet 的MAC地址，16进制形式。
        /// </summary>
        public byte[] MAC { get; set; }

        /// <summary>
        /// 客户的认证账号，不含@mail.jlu.edu.cn
        /// </summary>
        public byte[] Username { get; set; }

        /// <summary>
        /// 客户的认证密码
        /// </summary>
        public byte[] Password { get; set; }

        /// <summary>
        /// 认证服务器IP地址，默认为 10.100.61.3（auth.jlu.edu.cn）
        /// </summary>
        public string ServerIP
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}",
                    (int)server[0],
                    (int)server[1],
                    (int)server[2],
                    (int)server[3]);
            }
            set
            {
                string[] p = value.Split('.');
                server[0] = byte.Parse(p[0]);
                server[1] = byte.Parse(p[1]);
                server[2] = byte.Parse(p[2]);
                server[3] = byte.Parse(p[3]);
            }
        }

        /// <summary>
        /// 客户机IP地址，例如 49.140.59.254
        /// </summary>
        public string ClientIP
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}",
                    (int)host_ip[0],
                    (int)host_ip[1],
                    (int)host_ip[2],
                    (int)host_ip[3]);
            }
            set
            {
                string[] p = value.Split('.');
                host_ip[0] = byte.Parse(p[0]);
                host_ip[1] = byte.Parse(p[1]);
                host_ip[2] = byte.Parse(p[2]);
                host_ip[3] = byte.Parse(p[3]);
            }
        }

        /// <summary>
        /// 客户机的计算机名
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 客户机的系统
        /// </summary>
        public string HostOS { get; set; }

        /// <summary>
        /// 初始化 Drcom 客户端参数
        /// </summary>
        public DrCert(string username, string password, byte[] mac, DrLogger func)
        {
            Username = Encoding.Default.GetBytes(username);
            Password = Encoding.Default.GetBytes(password);
            MAC = mac;
            host_ip = new byte[] { 0, 0, 0, 0 };
            HostName = "DESKTOP-0GJ7Q83";
            HostOS = "Windows 10";
            OnMakeLog += func;
        }

        /// <summary>
        /// 绑定日志输出
        /// </summary>
        public event DrLogger OnMakeLog;

        /// <summary>
        /// 制造握手成功数据包
        /// </summary>
        public byte[] MakeLoginPacket(byte[] salt, out byte[] data, out int len)
        {
            int pwlen = Password.Length;
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
            Buffer.BlockCopy(Password, 0, md5_src, 6, pwlen);
            MD5A = md5.ComputeHash(md5_src);
            Buffer.BlockCopy(MD5A, 0, data, 4, 16);
            i += 16;

            // Username
            Buffer.BlockCopy(Username, 0, data, i, Username.Length);
            i += Username.Length > 36 ? Username.Length : 36;

            // CONTROLCHECKSTATUS ADAPTERNUM
            data[i++] = 0x20;
            data[i++] = 0x03;

            /* (data[4:10].encode('hex'),16)^mac */
            Buffer.BlockCopy(MD5A, 0, data, i, 6);
            for (int j = 0; j < 6; j++)
                data[i + j] ^= MAC[j];
            i += 6;

            // MD5 B
            md5_src = new byte[9 + pwlen];
            md5_src[0] = 1;
            Buffer.BlockCopy(Password, 0, md5_src, 1, pwlen);
            Buffer.BlockCopy(salt, 0, md5_src, 1 + pwlen, 4);
            Buffer.BlockCopy(md5.ComputeHash(md5_src), 0, data, i, 16);
            i += 16;

            // Ip number, 1, 2, 3, 4
            data[i++] = 0x01;
            Buffer.BlockCopy(host_ip, 0, data, i, 4);
            i += 16;

            // MD5 C
            md5_src = new byte[i + 4];
            Buffer.BlockCopy(data, 0, md5_src, 0, i);
            md5_src[i + 0] = 0x14;
            md5_src[i + 1] = 0x00;
            md5_src[i + 2] = 0x07;
            md5_src[i + 3] = 0x0b;
            Buffer.BlockCopy(md5.ComputeHash(md5_src), 0, data, i, 8);
            i += 8;

            // IPDOG(0x01) 0x00*4
            data[i++] = 0x01;
            i += 4;

            // Host Name
            md5_src = Encoding.Default.GetBytes(HostName);
            Buffer.BlockCopy(md5_src, 0, data, i, md5_src.Length > 32 ? 32 : md5_src.Length);
            i += 32;

            // primary dns 10.10.10.10
            data[i++] = 0x0a;
            data[i++] = 0x0a;
            data[i++] = 0x0a;
            data[i++] = 0x0a;

            // DHCP Server
            i += 4;

            // Secondary dns 202.98.18.3
            data[i++] = 0xca;
            data[i++] = 0x62;
            data[i++] = 0x12;
            data[i++] = 0x03;

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
                x = MD5A[k] ^ Password[k];
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
            Buffer.BlockCopy(MAC, 0, data, i, 6);
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
            OnMakeLog.Invoke("mkpkt", data, true);
#else
            OnMakeLog.Invoke("login", "Making login packet...", false);
#endif
            return data;
        }

    }

}
