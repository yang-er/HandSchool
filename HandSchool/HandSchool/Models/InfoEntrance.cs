using HandSchool.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace HandSchool
{
    public interface IInfoEntrance : ISystemEntrance
    {
        string Description { get; set; }
        List<string> TableHeader { get; set; }
        string ReturnData { get; set; }
        string JsStr { get; set; }
        List<IHtmlInput> ParamList { get; }
        void ReFreshPage();
        void ReciveData();
        event Action DataUpdated;
    }
}
