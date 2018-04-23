using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
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
            List<InfoEntranceMenu> Menu { get; set; }
        }
    }

    namespace Models
    {
        public delegate IInfoEntrance EntranceCreator();

        public class InfoEntranceGroup : List<InfoEntranceWrapper>
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

        public struct InfoEntranceMenu
        {
            public string Name;
            public Command Command;
            public string Icon;

            public InfoEntranceMenu(string name, Command cmd, string ico)
            {
                Name = name;
                Command = cmd;
                Icon = ico;
            }
        }
    }
}