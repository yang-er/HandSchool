using HandSchool.Internal;
using System;
using System.Collections.Generic;
using Bootstrap = HandSchool.Internal.HtmlObject.Bootstrap;
using System.Collections.Specialized;
using System.Text;
using HandSchool.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;

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

    public delegate IInfoEntrance EntranceCreator();

    public class InfoEntranceGroup : ObservableCollection<InfoEntranceWrapper>
    {
        public string GroupTitle { get; set; }
    }
    
    public class InfoEntranceWrapper
    {
        public string Name { get; }
        public string Description { get; }
        public EntranceCreator Load { get; }

        public InfoEntranceWrapper(string name, string description, EntranceCreator creator)
        {
            Name = name;
            Description = description;
            Load = creator;
        }
    }
}
