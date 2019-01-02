using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HandSchool.JLU.Models
{
    /// <summary>
    /// 拾卡信息的储存类。
    /// </summary>
    internal class PickCardInfo
    {
        public string CardNumber { get; set; }
        public string Picker { get; set; }
        public string Contact { get; set; }
        public string Time { get; set; }
        public string Address { get; set; }

        public string Title => CardNumber;
        public string Description => "拾卡时间：" + Time + (Address == "无" ? "" : "\n地址：" + Address) + (Contact == "无" ? "" : "\n联系方式：" + Contact);
        
        public static IEnumerable<PickCardInfo> EnumerateFromHtml(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");
            var xDoc = XDocument.Parse(html);
            return from item in xDoc.Root.Elements()
                   let inner = item.Elements().First()
                   select new PickCardInfo
                   {
                       CardNumber = (string)inner.Elements().ElementAt(0).Elements().ElementAt(1),
                       Picker = (string)inner.Elements().ElementAt(1).Elements().ElementAt(1),
                       Contact = (string)inner.Elements().ElementAt(2).Elements().ElementAt(1),
                       Time = (string)inner.Elements().ElementAt(3).Elements().ElementAt(1),
                       Address = (string)inner.Elements().ElementAt(4).Elements().ElementAt(1)
                   };
        }
    }
}
