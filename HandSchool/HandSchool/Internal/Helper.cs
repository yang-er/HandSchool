﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using FeedItem = HandSchool.Models.FeedItem;

namespace HandSchool.Internal
{
    /// <summary>
    /// 提供基础静态方法的帮助类
    /// </summary>
    static partial class Helper
    {
        private static StringBuilder sb = new StringBuilder();
        private static JsonSerializer json = JsonSerializer.Create();
        
        /// <summary>
        /// 解析RSS文档
        /// </summary>
        /// <param name="report">RSS文档文件</param>
        /// <returns><see cref="FeedItem" /> 的枚举</returns>
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

        /// <summary>
        /// 对 <see cref="byte[]" /> 进行MD5运算
        /// </summary>
        /// <param name="source">源编码数组</param>
        /// <returns>加密后编码数组</returns>
        public static byte[] MD5(byte[] source)
        {
            byte[] bytHash;
            using (MD5 MD5p = new MD5CryptoServiceProvider())
            {
                bytHash = MD5p.ComputeHash(source);
            }
            return bytHash;
        }
        
        /// <summary>
        /// 对 <see cref="string[]" /> 进行MD5运算
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="encoding">指定编码，默认UTF-8</param>
        /// <returns>编码后字符串</returns>
        public static string MD5(string source, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return HexDigest(MD5(encoding.GetBytes(source)), true);
        }

        /// <summary>
        /// 将字符串转化为 <see cref="T" /> 的对象
        /// </summary>
        /// <typeparam name="T">目标转换类型</typeparam>
        /// <param name="jsonString">源JSON字符串</param>
        /// <returns>反序列化后的值</returns>
        /// <exception cref="JsonException" />
        public static T JSON<T>(string jsonString)
        {
            if (jsonString == "") throw new JsonReaderException();
            return json.Deserialize<T>(new JsonTextReader(new StringReader(jsonString)));
        }

        /// <summary>
        /// 将对象序列化为JSON文本
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns>JSON文本</returns>
        public static string Serialize(object value)
        {
            json.Serialize(new JsonTextWriter(new StringWriter(sb)), value);
            var ret = sb.ToString();
            sb.Clear();
            return ret;
        }

        /// <summary>
        /// 将 <see cref="byte[]" /> 转化为对应的十六进制码字符串
        /// </summary>
        /// <param name="source">源编码数组</param>
        /// <param name="lower">是否为小写字母</param>
        /// <returns>十六进制字符串</returns>
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

        /// <summary>
        /// 将字符串数组转为application/x-form-urlencoded
        /// </summary>
        /// <param name="dict">字符串数组</param>
        /// <param name="startupDelimiter">起始字符</param>
        /// <returns>application/x-form-urlencoded</returns>
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
