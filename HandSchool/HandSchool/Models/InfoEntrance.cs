using HandSchool.Internal;
using System;
using System.Collections.Generic;
using Bootstrap = HandSchool.Internal.HtmlObject.Bootstrap;
using System.Collections.Specialized;
using System.Text;
using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool
{
    public interface IInfoEntrance : ISystemEntrance
    {
        string Description { get; }
        List<string> TableHeader { get; set; }
        Bootstrap HtmlDocument { get; set; }
        void Receive(string data);
        WebViewPage Binding { get; set; }
        Dictionary<string, Command> Menu { get; set; }
    }
}
