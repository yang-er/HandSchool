﻿using System.Net;
using System.Threading.Tasks;
using HandSchool.Models;
using Xamarin.Forms;

namespace HandSchool.Controls
{
    public class HSWebView : WebView
    {
        public HSWebView()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    HSCookies = new CookieContainer();
                    break;

                default:
                    Cookies = HSCookies = new CookieContainer();
                    break;
            }
        }

        public TaskCompletionSource<TaskResp> Result = new TaskCompletionSource<TaskResp>();
        private HSWebViewEvents _events;

        public void SendReceivingJsData(string data)
        {
            _events.OnReceivingJsData(data);
        }

        public CookieContainer HSCookies { get; }
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