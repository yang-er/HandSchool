using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace HandSchool
{
    public interface IInfoEntrance : ISystemEntrance
    {
        string Description { get; set; }
        string ReturnData { get; set; }
        string JsStr { get; set; }
        List<HtmlInput> Paramaslist { get; }
        string HtmlCreator(List<HtmlInput> Paramslist);
        void ReFreshPage();
        void ReciveData();
        void OpenWebView();
        void SentHtmlData();
    }
    public struct HtmlInput
    {
        string Name;
        string Type;
        string PlaceHolder;
        string DefaultValue;
    };
}
