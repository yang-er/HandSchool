﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HandSchool.JLU.Models
{
    /// <summary>
    /// 校园卡消费记录的储存类。
    /// </summary>
    internal class RecordInfo : SchoolCardInfoPiece
    {
        public string RecordTime { get; set; }
        public string RecordPlace { get; set; }
        public string RecordName { get; set; }
        public string RecordCost { get; set; }
        public string RemainMoney { get; set; }

        public string CostShow => (RecordCost.StartsWith("-") ? "" : "+") + RecordCost;
        public string RemainMoneyShow => RemainMoney + "元";
        public string DescriptionShow => $"商户名称：{(string.IsNullOrWhiteSpace(RecordPlace) ? "不详" : RecordPlace)}";

        public override string Title => $"{RecordName} {CostShow}";
        public override string Description => $"{DescriptionShow}\n卡余额：{RemainMoney}";
        
        public static IEnumerable<RecordInfo> EnumerateFromHtml(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");

            var xDoc = XDocument.Parse(html);
            return from item in xDoc.Root.Elements()
                   let inner = item.Elements().First()
                   select new RecordInfo
                   {
                       RecordTime = (string)inner.Elements().ElementAt(0).Elements().ElementAt(1),
                       RecordPlace = (string)inner.Elements().ElementAt(1).Elements().ElementAt(1),
                       RecordName = (string)inner.Elements().ElementAt(2).Elements().ElementAt(1),
                       RecordCost = (string)inner.Elements().ElementAt(3).Elements().ElementAt(1),
                       RemainMoney = (string)inner.Elements().ElementAt(4).Elements().ElementAt(1)
                   };
        }
    }
}

