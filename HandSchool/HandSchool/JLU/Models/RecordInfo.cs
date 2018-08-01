using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace HandSchool.JLU.Models
{
    class RecordInfo
    {
        public string RecordTime { get; set; }
        public string RecordPlace { get; set; }
        public string RecordName { get; set; }
        public string RecordCost { get; set; }
        public string RemainMoney { get; set; }

        public string Title => RecordTime;
        public string Description => "消费时间：" + RecordTime + (RecordPlace == "无" ? "" : "\n商户名称：" + RecordPlace) +"\n交易金额:"+RecordCost+"\n交易名称:"+RecordName+"\n卡余额:"+RemainMoney;

        public RecordInfo() { }

        public static IEnumerable<RecordInfo> EnumerateFromHtml(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");
            var xdoc = XDocument.Parse(html);
            return (from item in xdoc.Root.Elements()
                    let inner = item.Elements().First()
                    select new RecordInfo
                    {
                        RecordTime = (string)inner.Elements().ElementAt(0).Elements().ElementAt(1),
                        RecordPlace = (string)inner.Elements().ElementAt(1).Elements().ElementAt(1),
                        RecordName = (string)inner.Elements().ElementAt(2).Elements().ElementAt(1),
                        RecordCost = (string)inner.Elements().ElementAt(3).Elements().ElementAt(1),
                        RemainMoney = (string)inner.Elements().ElementAt(4).Elements().ElementAt(1)
                    });
        }

        public RecordInfo(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");
            var xd = new XmlDocument();
            xd.Load(new StringReader(html));
            var rootobj = xd.FirstChild;
            RecordTime = rootobj.ChildNodes[0].ChildNodes[1].InnerText;
            RecordPlace = rootobj.ChildNodes[1].ChildNodes[1].InnerText;
            RecordName = rootobj.ChildNodes[2].ChildNodes[1].InnerText;
            RecordCost = rootobj.ChildNodes[3].ChildNodes[1].InnerText;
            RemainMoney = rootobj.ChildNodes[4].ChildNodes[1].InnerText;
        }
    }
}

