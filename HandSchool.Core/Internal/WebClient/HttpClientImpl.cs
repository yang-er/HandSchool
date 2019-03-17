using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.Internals
{
    public class HttpClientImpl : IWebClient
    {
        HttpClient HttpClient { get; }
        HttpClientHandler Handler { get; }

        /// <summary>
        /// Cookie的容器
        /// </summary>
        public CookieContainer Cookie { get; }

        /// <summary>
        /// 默认字符编码
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 是否允许自动导航
        /// </summary>
        public bool AllowAutoRedirect
        {
            get => Handler.AllowAutoRedirect;
            set => Handler.AllowAutoRedirect = value;
        }

        /// <summary>
        /// 网站基础地址
        /// </summary>
        public string BaseAddress
        {
            get => HttpClient.BaseAddress?.OriginalString ?? "";
            set => HttpClient.BaseAddress = new Uri(value);
        }

        /// <summary>
        /// 超时时长
        /// </summary>
        public int Timeout
        {
            get => HttpClient.Timeout.Milliseconds;
            set => HttpClient.Timeout = new TimeSpan(0, 0, 0, 0, value);
        }

        public void AttachHeader(string key, string value)
        {
            HttpClient.DefaultRequestHeaders.Add(key, value);
        }

        public HttpClientImpl()
        {
            Cookie = new CookieContainer();

            Handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = Cookie,
            };

            try
            {
                Handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            }
            catch
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }

            HttpClient = new HttpClient(Handler, true);
            Timeout = 15000;
            Encoding = Encoding.UTF8;
        }

        private async Task<IWebResponse> SendAsync(HttpRequestMessage msg, WebRequestMeta meta)
        {
            try
            {
                var response = await HttpClient.SendAsync(msg);
                if (response.IsSuccessStatusCode && meta.Accept != "*/*" && response.Content.Headers.ContentType.MediaType != meta.Accept)
                    throw new WebsException(new WebResponse(response, meta, WebStatus.MimeNotMatch, BaseAddress));
                if ((int)response.StatusCode >= 400)
                    throw new WebsException(new WebResponse(response, meta, WebStatus.ProtocolError, BaseAddress));
                return new WebResponse(response, meta, WebStatus.Success, BaseAddress);
            }
            catch (HttpRequestException ex)
            {
                var known = WebStatus.UnknownError;

                if (ex.InnerException is SocketException ex2)
                {
                    known = Convert(ex2);
                }
                else if (ex.InnerException is WebException ex3)
                {
                    known = ex3.Status.Convert();
                }

                throw new WebsException(new WebResponse(meta, known), ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new WebsException(new WebResponse(meta, WebStatus.Timeout), ex);
            }
            catch (OperationCanceledException ex)
            {
                throw new WebsException(new WebResponse(meta, WebStatus.Timeout), ex);
            }
        }

        internal static WebStatus Convert(SocketException ex)
        {
            switch (ex.SocketErrorCode)
            {
                case SocketError.ConnectionReset:
                case SocketError.ConnectionAborted:
                    return WebStatus.ReceiveFailure;

                case SocketError.NetworkUnreachable:
                case SocketError.NetworkDown:
                case SocketError.HostNotFound:
                    return WebStatus.NameResolutionFailure;

                case SocketError.ConnectionRefused:
                case SocketError.NetworkReset:
                case SocketError.HostUnreachable:
                case SocketError.TooManyOpenSockets:
                    return WebStatus.ConnectFailure;

                case SocketError.TimedOut:
                    return WebStatus.Timeout;

                default:
                    return WebStatus.UnknownError;
            }
        }

        public Task<IWebResponse> PostAsync(WebRequestMeta req, KeyValueDict value)
        {
            var realRequest = new HttpRequestMessage(HttpMethod.Post, req.Url);
            if (req.Accept != "*/*") realRequest.Headers.Accept.TryParseAdd(req.Accept);
            realRequest.Content = new FormUrlEncodedContent(value);

            foreach (var header in req.Headers)
            {
                realRequest.Headers.Add(header.Key, header.Value);
            }

            return SendAsync(realRequest, req);
        }

        public Task<IWebResponse> PostAsync(WebRequestMeta req, string value, string contentType)
        {
            var realRequest = new HttpRequestMessage(HttpMethod.Post, req.Url);
            if (req.Accept != "*/*") realRequest.Headers.Accept.TryParseAdd(req.Accept);
            realRequest.Content = new StringContent(value, Encoding, contentType);

            foreach (var header in req.Headers)
            {
                realRequest.Headers.Add(header.Key, header.Value);
            }

            return SendAsync(realRequest, req);
        }

        public Task<IWebResponse> GetAsync(WebRequestMeta req)
        {
            var realRequest = new HttpRequestMessage(HttpMethod.Get, req.Url);
            if (req.Accept != "*/*") realRequest.Headers.Accept.TryParseAdd(req.Accept);

            foreach (var header in req.Headers)
            {
                realRequest.Headers.Add(header.Key, header.Value);
            }

            return SendAsync(realRequest, req);
        }

        /// <summary>
        /// 对HttpResponseMessage的适配。
        /// </summary>
        private class WebResponse : IWebResponse
        {
            public WebResponse(HttpResponseMessage resp, WebRequestMeta meta, WebStatus stat, string baseUrl)
            {
                Request = meta;
                InnerResponse = resp;
                StatusCode = resp.StatusCode;
                Location = resp.Headers.Location?.OriginalString ?? "";
                if (!string.IsNullOrEmpty(baseUrl))
                    Location = Location.Replace(baseUrl, "");
                ContentType = resp.Content.Headers.ContentType?.MediaType ?? "*/*";
                Status = stat;
            }

            public WebResponse(WebRequestMeta meta, WebStatus stat)
            {
                Request = meta;
                StatusCode = HttpStatusCode.InternalServerError;
                Location = "";
                ContentType = "*/*";
                Status = stat;
            }

            private HttpResponseMessage InnerResponse { get; }
            
            public WebRequestMeta Request { get; }

            public HttpStatusCode StatusCode { get; }

            public string Location { get; }

            public string ContentType { get; }

            public WebStatus Status { get; }

            public Task<byte[]> ReadAsByteArrayAsync()
            {
                if (InnerResponse is null) return Task.FromResult<byte[]>(null);
                else return InnerResponse?.Content.ReadAsByteArrayAsync();
            }

            public Task<string> ReadAsStringAsync()
            {
                if (InnerResponse is null) return Task.FromResult<string>(null);
                else return InnerResponse?.Content.ReadAsStringAsync();
            }

            public async Task WriteToFileAsync(string path)
            {
                if (InnerResponse is null) return;
                using (var fs = new FileStream(path, FileMode.Create))
                    await InnerResponse.Content.CopyToAsync(fs);
            }

            public IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetHeaders()
            {
                return InnerResponse.Headers;
            }

            public void Dispose()
            {
                InnerResponse?.Dispose();
            }
        }

        /// <summary>
        /// 清理对象的资源占用。
        /// </summary>
        public void Dispose()
        {
            Core.Logger.WriteLine("HttpClient", "requested dispose.");
            // HttpClient.CancelPendingRequests();
        }
    }
}