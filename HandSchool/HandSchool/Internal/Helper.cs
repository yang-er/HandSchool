using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;
using Xamarin.Forms;
using FeedItem = HandSchool.Models.FeedItem;

namespace HandSchool.Internal
{
    static partial class Helper
    {
        private static StringBuilder sb = new StringBuilder();
        private static JsonSerializer json = JsonSerializer.Create();
        public static string[] ScheduleColors = { "#59e09e", "#f48fb1", "#ce93d8", "#ff8a65", "#9fa8da", "#42a5f5", "#80deea", "#c6de7c" };
        
        [Obsolete("Use Core.ReadConfig instead.")]
        public static string ReadConfFile(string name)
        {
            return Core.ReadConfig(name);
        }

        [Obsolete("Use Core.WriteConfig instead.")]
        public static void WriteConfFile(string name, string value)
        {
            Core.WriteConfig(name, value);
        }
        
        public static IEnumerable<FeedItem> ParseRSS(string report)
        {
            var xdoc = XDocument.Parse(report);
            var id = 0;
            return (from item in xdoc.Root.Element("channel").Descendants("item")
                    select new FeedItem
                    {
                        Title = (string)item.Element("title"),
                        Description = (string)item.Element("description"),
                        PubDate = (string)item.Element("pubDate"),
                        Category = (string)item.Elements("category").Last(),
                        Link = (string)item.Element("link"),
                        Id = id++
                    });
        }

        public static byte[] MD5(byte[] source)
        {
            byte[] bytHash;
            using (MD5 MD5p = new MD5CryptoServiceProvider())
            {
                bytHash = MD5p.ComputeHash(source);
            }
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

        public static string Serialize(object value)
        {
            json.Serialize(new JsonTextWriter(new StringWriter(sb)), value);
            var ret = sb.ToString();
            sb.Clear();
            return ret;
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
