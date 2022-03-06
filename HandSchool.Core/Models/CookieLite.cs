using System.Net;

namespace HandSchool.Models
{
    /// <summary>
    /// 用来方便序列化Cookie
    /// </summary>
    public class CookieLite
    {
        private readonly Cookie _innerCookie;

        public CookieLite(Cookie c)
        {
            _innerCookie = c;
        }

        public string Domain => _innerCookie?.Domain;
        public string Path => _innerCookie?.Path;
        public string Name => _innerCookie?.Name;
        public string Value => _innerCookie?.Value;
    }
}