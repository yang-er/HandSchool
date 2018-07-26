using HandSchool.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace HandSchool.JLU.Models
{
    class PickCardInfo
    {
        public string CardNumber;
        public string Picker;
        public string Contact;
        public string Time;
        public string Address;

        public PickCardInfo() { }

        public static IEnumerable<PickCardInfo> EnumerateFromHtml(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");
            var xdoc = XDocument.Parse(html);
            return (from item in xdoc.Root.Elements()
                    let inner = item.Elements().First()
                    select new PickCardInfo
                    {
                        CardNumber = (string)inner.Elements().ElementAt(0).Elements().ElementAt(1),
                        Picker = (string)inner.Elements().ElementAt(1).Elements().ElementAt(1),
                        Contact = (string)inner.Elements().ElementAt(2).Elements().ElementAt(1),
                        Time = (string)inner.Elements().ElementAt(3).Elements().ElementAt(1),
                        Address = (string)inner.Elements().ElementAt(4).Elements().ElementAt(1)
                    });
        }

        public PickCardInfo(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");
            var xd = new XmlDocument();
            xd.Load(new StringReader(html));
            var rootobj = xd.FirstChild;
            CardNumber = rootobj.ChildNodes[0].ChildNodes[1].InnerText;
            Picker = rootobj.ChildNodes[1].ChildNodes[1].InnerText;
            Contact = rootobj.ChildNodes[2].ChildNodes[1].InnerText;
            Time = rootobj.ChildNodes[3].ChildNodes[1].InnerText;
            Address = rootobj.ChildNodes[4].ChildNodes[1].InnerText;
        }
    }
}
