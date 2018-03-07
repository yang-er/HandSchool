using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HandSchool.Internal
{
    class Helper
    {
        private static MD5 MD5p = new MD5CryptoServiceProvider();
        private static StringBuilder sb = new StringBuilder();

        public static byte[] MD5(byte[] source)
        {
            byte[] bytHash = MD5p.ComputeHash(source);
            MD5p.Clear();
            return bytHash;
        }
        
        public static string MD5(string source, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return HexDigest(MD5(encoding.GetBytes(source)), true);
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
