#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HandSchool.JLU.Services
{
    public class VpnUtil
    {
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        private static int GetDefaultPort(string sch)
        {
            return sch switch
            {
                "http" => 80,
                "https" => 443,
                _ => throw new ArgumentException("param must be 'http' or 'https'")
            };
        }

        private static string PreprocessDomain(string data)
        {
            const int @base = 16;
            var padCnt = @base - data.Length % @base;
            var sb = new StringBuilder(data);
            for (var i = 0; i < padCnt; i++)
            {
                sb.Append('0');
            }

            return sb.ToString();
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder();
            bytes.ToList().ForEach(b => { sb.Append(b.ToString("x2")); });
            return sb.ToString();
        }

        private ValueTuple<string?, string?> _lastKeys = (null, null);
        private ICryptoTransform? _encryptor;

        private string EncryptDomain(string strString, string key, string iv)
        {
            if (_lastKeys.Item1 != key || _lastKeys.Item2 != iv)
            {
                _lastKeys = (key, iv);
                var aes = Rijndael.Create();
                aes.Mode = CipherMode.CFB;
                aes.FeedbackSize = 128;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);
                _encryptor = aes.CreateEncryptor();
            }

            if (_encryptor is null)
                throw new NotSupportedException(
                    "This environment is not support because there is no supported encryptor implement");

            var len = strString.Length;
            strString = PreprocessDomain(strString);
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            var dataBytes = Encoding.UTF8.GetBytes(strString);
            var res = _encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return ToHex(ivBytes) + ToHex(res).Substring(0, 2 * len);
        }

        private string EncryptDomain(string strString)
        {
            if (_cache.TryGetValue(strString, out var proxy))
                return proxy;
            proxy = EncryptDomain(strString, EncryptKey, EncryptKey);
            _cache[strString] = proxy;
            return proxy;
        }
        
        public static string? EncryptKey { get; set; }
        public static string? EncryptIV { get; set; }

        public string ConvertUri(Uri uri)
        {
            return uri.Host.ToLower() == "vpn.jlu.edu.cn"
                ? uri.OriginalString
                : $"https://vpn.jlu.edu.cn/{uri.Scheme}{(uri.IsDefaultPort() ? "" : "-" + uri.Port)}/{EncryptDomain(uri.Host)}{uri.PathAndQuery}";
        }

        private static bool IsAbsolute(string str)
        {
            return str.StartsWith("https://") || str.StartsWith("http://");
        }

        /// <summary>
        /// 将基地址和path拼合成一个绝对url，
        /// 若给出的path已经是绝对url，则直接返回；
        /// 若path是一个相对url，则将其与绝对baseUrl拼合后返回。
        /// </summary>
        /// <exception cref="UriFormatException">path为相对url且未设置baseUrl时抛出</exception>
        public static Uri Combine(string baseUrl, string path)
        {
            var realBase = baseUrl.Trim();
            var realUrl = path.Trim();
            if (IsAbsolute(realUrl)) return new Uri(realUrl);
            if (string.IsNullOrWhiteSpace(realBase))
                throw new UriFormatException("baseUrl cannot be blank if the url is not absolute");
            if (realBase.EndsWith("/")) realBase = realBase.Substring(0, realBase.Length - 1);
            if (realUrl.StartsWith("/")) realUrl = realUrl.Substring(1, realUrl.Length - 1);
            return new Uri($"{realBase}/{realUrl}");
        }
    }
}