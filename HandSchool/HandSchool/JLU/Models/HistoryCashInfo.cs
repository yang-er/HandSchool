using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HandSchool.JLU.Models
{
    class HistoryCashInfo
    {
        public string Time { get; set; }
        public string StoreName { get; set; }
        public string Type { get; set; }
        public string DeltaMoney { get; set; }
        public string Balance { get; set; }

        public static IEnumerable<HistoryCashInfo> EnumerateFromHtml(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");
            var xdoc = XDocument.Parse(html);
            return (from item in xdoc.Root.Elements()
                    let inner = item.Elements().First()
                    select new HistoryCashInfo
                    {
                        Time = (string)inner.Elements().ElementAt(0).Elements().ElementAt(1),
                        StoreName = (string)inner.Elements().ElementAt(1).Elements().ElementAt(1),
                        Type = (string)inner.Elements().ElementAt(2).Elements().ElementAt(1),
                        DeltaMoney = (string)inner.Elements().ElementAt(3).Elements().ElementAt(1),
                        Balance = (string)inner.Elements().ElementAt(4).Elements().ElementAt(1)
                    });
        }
    }
}
