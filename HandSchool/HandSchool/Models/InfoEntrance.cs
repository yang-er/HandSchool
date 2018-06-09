using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Bootstrap = HandSchool.Internal.HtmlObject.Bootstrap;
using HFAttr = HandSchool.Models.HotfixAttribute;

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

            public InfoEntranceWrapper(string name, string description, Type type)
            {
                Name = name;
                Description = description;
                if (type.GetCustomAttribute(typeof(HFAttr)) is HFAttr hfattr)
                    hfattr.CheckUpdate();
                Load = () => Assembly.GetExecutingAssembly()
                    .CreateInstance(type.FullName) as IInfoEntrance;
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