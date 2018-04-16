using HandSchool.Internal;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Bootstrap = HandSchool.Internal.HtmlObject.Bootstrap;

namespace HandSchool
{
    namespace Services
    {
        public interface IInfoEntrance : ISystemEntrance
        {
            string Description { get; }
            List<string> TableHeader { get; set; }
            Bootstrap HtmlDocument { get; set; }
            void Receive(string data);
            IViewResponse Binding { get; set; }
            Action<string> Evaluate { get; set; }
            Dictionary<string, Command> Menu { get; set; }
        }
    }

    namespace Models
    {
        public delegate IInfoEntrance EntranceCreator();

        public class InfoEntranceGroup : ObservableCollection<InfoEntranceWrapper>
        {
            public string GroupTitle { get; set; }

            public override string ToString()
            {
                return GroupTitle;
            }
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
}