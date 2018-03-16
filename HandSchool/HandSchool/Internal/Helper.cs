using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    class Helper
    {
        private static MD5 MD5p = new MD5CryptoServiceProvider();
        private static StringBuilder sb = new StringBuilder();
        private static JsonSerializer json = JsonSerializer.Create();
        private static bool auto_login = true;
        private static bool save_password = true;
        public static string DataBaseDir;
        public static string SegoeMDL2;

        public static string GetWebExceptionMessage(WebException e)
        {
            switch (e.Status)
            {
                case WebExceptionStatus.NameResolutionFailure:
                    return "域名解析失败，未连接到互联网";
                case WebExceptionStatus.ConnectFailure:
                    return "连接服务器失败，未连接到校内网络";
                case WebExceptionStatus.ReceiveFailure:
                case WebExceptionStatus.SendFailure:
                case WebExceptionStatus.PipelineFailure:
                case WebExceptionStatus.RequestCanceled:
                case WebExceptionStatus.ConnectionClosed:
                    return "数据包传输出现错误";
                case WebExceptionStatus.TrustFailure:
                case WebExceptionStatus.SecureChannelFailure:
                case WebExceptionStatus.ServerProtocolViolation:
                case WebExceptionStatus.KeepAliveFailure:
                    return "网络沟通出现错误";
                case WebExceptionStatus.Pending:
                case WebExceptionStatus.Timeout:
                    return "连接超时，可能是您的网络不太好";
                default:
                    return e.Status.ToString() + "\n" + e.StackTrace;
            }
        }

        public static int GetDeviceSpecified(string name)
        {
            return 0;
        }
        
        public static bool AutoLogin
        {
            get
            {
                return auto_login;
            }
            set
            {
                auto_login = value;
                if (value) save_password = true;
            }
        }

        public static bool SavePassword
        {
            get
            {
                return save_password;
            }
            set
            {
                save_password = value;
                if (!save_password) auto_login = false;
            }
        }

        public static string ReadConfFile(string name)
        {
            string fn = Path.Combine(DataBaseDir, name);
            if (File.Exists(fn))
                return File.ReadAllText(Path.Combine(DataBaseDir, name));
            else
                return "";
        }

        public static void WriteConfFile(string name, string value)
        {
            File.WriteAllText(Path.Combine(DataBaseDir, name), value);
        }

        public static byte[] MD5(byte[] source)
        {
            byte[] bytHash;
            try
            {
                bytHash = MD5p.ComputeHash(source);
            }
            catch (ObjectDisposedException)
            {
                MD5p = new MD5CryptoServiceProvider();
                bytHash = MD5p.ComputeHash(source);
            }
            MD5p.Clear();
            return bytHash;
        }
        
        public static string MD5(string source, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return HexDigest(MD5(encoding.GetBytes(source)), true);
        }

        public static T JSON<T>(string jsonString)
        {
            if (jsonString == "") throw new JsonReaderException();
            return json.Deserialize<T>(new JsonTextReader(new StringReader(jsonString)));
        }

        public static string HexDigest(byte[] source, bool lower = false)
        {
            char[] chars = (lower ? "0123456789abcdef" : "0123456789ABCDEF").ToCharArray();
            int bit;
            for (int i = 0; i < source.Length; i++)
            {
                bit = (source[i] & 0x0f0) >> 4;
                sb.Append(chars[bit]);
                bit = source[i] & 0x0f;
                sb.Append(chars[bit]);
            }
            string ret = sb.ToString();
            sb.Clear();
            return ret;
        }

        public static string HttpBuildQuery(Dictionary<string, string> dict, string startupDelimiter = "")
        {
            string result = string.Empty;
            foreach (var item in dict)
            {
                if (string.IsNullOrEmpty(result))
                    result += startupDelimiter;
                else
                    result += "&";
                result += string.Format("{0}={1}", item.Key, item.Value);
            }
            return result;
        }
    }
}
