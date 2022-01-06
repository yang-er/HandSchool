using System;
using System.Net;
using System.Threading.Tasks;
using HandSchool.Models;
using Xamarin.Forms;

namespace HandSchool.Controls
{
    public class HSWebView : WebView
    {
        public HSWebView()
        {
            Cookies = new CookieContainer();
        }
        public TaskCompletionSource<TaskResp> Result = new TaskCompletionSource<TaskResp>();
        private HSWebViewEvents _events;

        public HSWebViewEvents Events
        {
            get => _events;
            set
            {
                if (_events != null)
                {
                    _events.WebView = null;
                    Navigating -= _events.OnNavigating;
                    Navigated -= _events.OnNavigated;
                }

                _events = value;
                if (value != null)
                {
                    value.WebView = this;
                    Navigating += value.OnNavigating;
                    Navigated += value.OnNavigated;
                }
            }
        }
    }
}