using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Android.Webkit;
using HandSchool.Internals;
using HandSchool.Models;
using SQLite;
using Xamarin.Forms.Internals;

namespace HandSchool.Droid.Renderers
{
    public interface IGetCookiesStrategy
    {
        /// <summary>
        /// 从本机获取WebView中指定url的Cookie
        /// </summary>
        public IEnumerable<Cookie> GetCookiesFromNative(string url);
    }

    /// <summary>
    /// CookieManager策略，比较通用，但只能获取Cookie的部分信息
    /// </summary>
    public class CookieManagerStrategy : IGetCookiesStrategy
    {
        private readonly CookieManager _manager;

        public CookieManagerStrategy()
        {
            _manager = CookieManager.Instance;
        }

        public IEnumerable<Cookie> GetCookiesFromNative(string url)
        {
            if (_manager is null) yield break;
            if (!_manager.HasCookies) yield break;
            var uri = new Uri(url);

            //将路径拆开
            var ps = new List<string> {""};
            uri.AbsolutePath.Trim()
                .Split('/')
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ForEach(p => ps.Add($"{ps[^1]}/{p}"));

            //将域名拆开
            var hosts = uri.Host.Trim().Split('.');
            var dps = new List<(string, string)>();

            //将所有带有点前缀的父域名加入查询队列
            var superHost = hosts[^1];
            for (var i = hosts.Length - 2; i >= 0; i--)
            {
                superHost = $"{hosts[i]}.{superHost}";
                ps.ForEach(p => dps.Add(($".{superHost}", p)));
            }

            //将域名本身加入查询队列
            ps.ForEach(p => dps.Add(($"{uri.Host}", p)));
            var res = new Dictionary<string, List<Cookie>>();

            //查询所有的domain-path对，并进行合并处理
            dps.ForEach(dp =>
            {
                var (domain, path) = dp;
                ParseCookies(
                        domain,
                        path == "" ? "/" : path,
                        _manager.GetCookie($"{uri.Scheme}://{domain}{path}"))
                    .ForEach(c => AddCookie(res, c));
            });

            foreach (var c in res.Values.SelectMany(cs => cs))
            {
                yield return c;
            }
        }

#nullable enable
        private static void AddCookie(IDictionary<string, List<Cookie>> dic, Cookie c)
        {
            if (dic.ContainsKey(c.Name))
            {
                var cur = dic[c.Name];
                if (!cur.Any(cookie => c.Name == cookie.Name && c.Value == cookie.Value))
                {
                    cur.Add(c);
                }
            }
            else
            {
                dic[c.Name] = new List<Cookie> {c};
            }
        }

        private static IEnumerable<Cookie> ParseCookies(string domain, string path, string? str)
        {
            return str?.Split(";")
                ?.Select(s => s?.Trim().Split("="))
                .Where(ss =>
                {
                    if (ss is null) return false;
                    return ss.Length != 0 && ss.Length <= 2;
                })
                .Select(ss =>
                {
                    var res = new Cookie
                    {
                        Domain = domain,
                        Path = path
                    };
                    switch (ss!.Length)
                    {
                        case 1:
                            res.Name = ss[0];
                            break;
                        case 2:
                            res.Name = ss[0];
                            res.Value = ss[1];
                            break;
                    }

                    return res;
                }) ?? Array.Empty<Cookie>();
        }
#nullable disable
    }

    /// <summary>
    /// 数据库策略，依赖厂商的WebView实现，能从底层获取WebView中Cookie的全部信息；
    /// 有些WebView实现中，只存储了加密后的Value，不能通过此策略获取；
    /// 有些WebView实现中，Cookie的数据库文件不在"webview/Default/Cookie"中，不能通过此策略获取；
    /// </summary>
    public class DataBaseStrategy : IGetCookiesStrategy
    {
        [Table("cookies")]
        private class AndroidCookieEntity
        {
            [Column("host_key")] public string HostKey { get; set; }

            [Column("name")] public string Name { get; set; }

            [Column("value")] public string Value { get; set; }

            [Column("path")] public string Path { get; set; }

            [Column("expires_utc")] public string ExpiresUtc { get; set; }

            [Column("is_secure")] public bool IsSecure { get; set; }

            [Column("is_httponly")] public bool IsHttpOnly { get; set; }

            [Column("last_access_utc")] public string LastAccessUtc { get; set; }

            [Column("has_expires")] public bool HasExpires { get; set; }

            [Column("is_persistent")] public bool IsPersistent { get; set; }

            [Column("priority")] public string Priority { get; set; }

            [Column("samesite")] public string SameSite { get; set; }

            [Column("source_scheme")] public string SourceScheme { get; set; }

            [Column("creation_utc")] public string CreationUtc { get; set; }
        }

        private SQLiteTableManager<AndroidCookieEntity> _cookieSql;
        private string _webViewFilesPath;
        private const string ConfigWebViewFiles = "webview_files_path";

        private void TryGetCookies()
        {
            if (_cookieSql != null) return;
            _webViewFilesPath ??= Core.Configure.Configs.GetItemWithPrimaryKey(ConfigWebViewFiles)?.Value;

            if (_webViewFilesPath is null)
            {
                _webViewFilesPath ??= Directory.GetDirectories(BaseActivity.InternalFileRootPath)
                    .Where(d => d.ToLower().Contains("webview"))
                    .FirstOrDefault(d =>
                    {
                        var current = Path.Combine(BaseActivity.InternalFileRootPath, d);
                        var dft = Path.Combine(current, "Default", "Cookies");
                        return File.Exists(dft);
                    });
                Core.Configure.Configs.InsertOrUpdateTable(new Config
                {
                    ConfigName = ConfigWebViewFiles,
                    Value = _webViewFilesPath
                });
            }

            if (_webViewFilesPath is null) return;
            _cookieSql = new SQLiteTableManager<AndroidCookieEntity>(false,
                _webViewFilesPath, "Default", "Cookies");
        }

        public IEnumerable<Cookie> GetCookiesFromNative(string url)
        {
            TryGetCookies();
            if (_cookieSql?.HasTable() != true) yield break;
            var list = _cookieSql.GetItems();
            foreach (var c in list.Select(ac => new Cookie
                     {
                         Name = ac.Name,
                         Value = ac.Value,
                         Domain = ac.HostKey,
                         Path = ac.Path,
                         HttpOnly = ac.IsHttpOnly,
                         Secure = ac.IsSecure,
                     }))
            {
                yield return c;
            }
        }
    }
}