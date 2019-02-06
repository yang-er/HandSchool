using HandSchool.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace HandSchool.UnitTest
{
    public abstract class WebClientTest
    {
        protected IWebClient WebClient { get; set; }

        [TestMethod]
        public async Task Response200Test()
        {
            var response = await WebClient.GetAsync("https://www.90yang.com");
            Assert.AreEqual(WebStatus.Success, response.Status);
            Assert.IsTrue(response.ContentType.StartsWith("text/html"));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task Response301Test()
        {
            var response = await WebClient.GetAsync("http://www.90yang.com");
            Assert.AreEqual(WebStatus.Success, response.Status);
            Assert.AreEqual("https://www.90yang.com/", response.Location);
            Assert.AreEqual(HttpStatusCode.Moved, response.StatusCode);
        }

        [TestMethod]
        public async Task Response404Test()
        {
            var response = await WebClient.GetAsync("https://www.xylab.fun/test404");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual("text/html", response.ContentType);
            Assert.AreNotEqual(WebStatus.ProtocolError, response.Status);
        }

        [TestMethod]
        public async Task StatusNameResolutionFailureTest()
        {
            var ex = await Assert.ThrowsExceptionAsync<WebsException>(async () =>
            {
                await WebClient.GetAsync("http://test.xylab.fun");
            });

            Assert.AreEqual(WebStatus.NameResolutionFailure, ex.Status);
        }

        [TestMethod]
        public async Task StatusConnectFailureTest()
        {
            WebClient.Timeout = 180000; // no timeout

            var ex = await Assert.ThrowsExceptionAsync<WebsException>(async () =>
            {
                await WebClient.GetAsync("http://127.0.0.1:43872");
            });

            Assert.AreEqual(WebStatus.ConnectFailure, ex.Status);
        }

        [TestMethod]
        public async Task StatusTimeoutTest()
        {
            WebClient.Timeout = 3000;

            var ex = await Assert.ThrowsExceptionAsync<WebsException>(async () =>
            {
                await WebClient.GetAsync("http://192.168.12.121/");
            });
            
            Assert.AreEqual(WebStatus.Timeout, ex.Status);
        }

        [TestMethod]
        public async Task PerformanceTest()
        {
            for (int i = 0; i < 1000; i++)
                await WebClient.GetAsync("http://localhost/");
        }
    }

    [TestClass]
    public class HttpClientImpl : WebClientTest
    {
        public HttpClientImpl()
        {
            WebClient = new Internals.HttpClientImpl();
        }
    }

    [TestClass]
    public class AwaredWebClientImpl : WebClientTest
    {
        public AwaredWebClientImpl()
        {
            WebClient = new Internals.AwaredWebClientImpl();
        }
    }
}