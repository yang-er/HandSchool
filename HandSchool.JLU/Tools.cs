﻿using HandSchool.Internals;
using HandSchool.JLU.Models;
using HandSchool.JLU.ViewModels;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HandSchool.JLU
{
    static class Tools
    {
        public static string HtmlTrim(this string str) => str.Replace("&nbsp;", "").Replace("&nbsp", "").Trim();
    
        public static List<RecordInfo> AnalyzeHtmlToRecordInfos(string htmlSources)
        {
            if (htmlSources.Contains("当前查询条件内没有流水记录")) return null;
            var html = new HtmlDocument();
            html.LoadHtml(htmlSources);
            var tds = html.DocumentNode.SelectNodes("//table[@class='table_show']/tbody/tr/td");
            var infoLists = new List<string>();
            foreach (var td in tds)
            {
                infoLists.Add(td.InnerText.Trim());
            }
            var res = new List<RecordInfo>();
            var i = 1;
            while (i < infoLists.Count)
            {
                var info = new RecordInfo();
                info.RecordTime = infoLists[i++];
                info.RecordPlace = infoLists[i++];
                info.RecordName = infoLists[i++];
                info.RecordCost = infoLists[i++];
                info.RemainMoney = infoLists[i++];
                res.Add(info);
            }

            return res.Count == 0 ? null : res;
        }
        public static string DateFormatter(DateTime date)
        {
            return date.Year + "-" +
                (date.Month < 10 ? "0" + date.Month : date.Month.ToString()) + "-" +
                (date.Day < 10 ? "0" + date.Day : date.Day.ToString());
        }
        public static string EncodingPwd(string pwd, string keyboard)
        {
            var sb = new StringBuilder();
            foreach (var ch in pwd)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (ch == keyboard[i])
                    {
                        sb.Append(i);
                        break;
                    }
                }
            }
            int len = sb.Length >> 1;
            for (int i = 0; i < len; i++)
            {
                var temp = sb[i];
                var index = sb.Length - i - 1;
                sb[i] = sb[index];
                sb[index] = temp;
            }
            return sb.ToString();
        }

        public static string AnalyzeHtmlToOaDetail(string htmlSources)
        {
            if (htmlSources.Contains("<table>")) return "通知含有表格，不支持表格解析";
            var html = new HtmlDocument();
            html.LoadHtml(htmlSources.Trim());
            var xpathBase = "//div[contains(@class,'content_font')]";
            var spans = html.DocumentNode.SelectNodes(xpathBase + "//span");
            var ps = html.DocumentNode.SelectNodes(xpathBase + "//p");
            ps ??= html.DocumentNode.SelectNodes(xpathBase + "//text()");
            try
            {
                StringBuilder sb = null, sp = null;
                if (ps != null)
                {
                    sb = new StringBuilder();
                    foreach (var text in ps)
                    {
                        sb.Append(text.InnerText.HtmlTrim()).Append('\n');
                    }
                }

                if (spans != null)
                {
                    sp = new StringBuilder();
                    foreach (var text in spans)
                    {
                        sp.Append(text.InnerText.HtmlTrim()).Append('\n');
                    }
                }

                if (sb == null || sp == null)
                {
                    if (sb == null && sp == null)
                        throw new NullReferenceException("Failed to analyze");
                    return (sb ?? sp).ToString().Trim();
                }

                return (sb.Length < sp.Length / 5) ? sp.ToString().Trim() : sb.ToString().Trim();
            }
            catch
            {
                try
                {
                    var texts = html.DocumentNode.SelectNodes(xpathBase + "//node()[not(node())]");
                    var stringBuilder = new StringBuilder();
                    foreach (var i in texts)
                    {
                        var text = i.InnerText.HtmlTrim();
                        if (!string.IsNullOrEmpty(text))
                            stringBuilder.Append(text).Append('\n');
                    }
                    return stringBuilder.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
    public class JLUClassSimplifier : ClassInfoSimplifier
    {
        private static Dictionary<string, int> _chineseNums;
        static string ChineseToNum(string str)
        {
            _chineseNums ??= new Dictionary<string, int>
            {
                {"零", 0}, {"一", 1}, {"二", 2}, {"三", 3}, {"四", 4},
                {"五", 5}, {"六", 6}, {"七", 7}, {"八", 8}, {"九", 9},
                {"十", 10}, {"十一", 11}, {"十二", 12}, {"十三", 13}, {"十四", 14}, {"十五", 15}
            };
            
            if (_chineseNums.ContainsKey(str)) return _chineseNums[str].ToString();
            return str;
        }

        /// <summary>
        /// JLU版简化课程描述的方法
        /// </summary>
        /// <returns>简化后的描述</returns>
        private static string _SimplifyName(string roomName)
        {
            //首先判断是否是在”所有周“情况下，此时没有上课地点信息
            var ruler = new Regex("第.+?-.+?周");
            string res = roomName;

            var m = ruler.Match(res);
            bool allWeek = false;
            while (m.Length != 0)
            {
                var str = m.Value;
                var str2 = str.Replace("第", "").Replace("周", "");
                res = res.Replace(str, str2);
                m = ruler.Match(res);
                allWeek = true;
            }
            if (allWeek)
            {
                return res;
            }
            
            //接下来，简化教学楼的名称
            ruler = new Regex("第.+?教学楼");
            var room = ruler.Match(roomName);
            if (room.Length != 0)
            {
                var str = room.Value;
                var index1 = res.IndexOf(str, StringComparison.Ordinal);
                var index2 = res.IndexOf("教学楼", index1, StringComparison.Ordinal);
                
                var area = res.Substring(index1 + 1, index2 - index1 - 1);
                var str2 = area + "教";
                res = res.Replace(str, str2);
            }

            res = res.Replace("教学楼", "楼");
            
            //最后，简化教室的名称
            ruler = new Regex("[A-Za-z]区第.+阶梯");
            room = ruler.Match(res);
            if (room.Length != 0)
            {
                var str = room.Value;
                var index = res.IndexOf(str, StringComparison.Ordinal);
                var area = res[index];
                
                var index1 = index + 2;
                var index2 = res.IndexOf("阶",index1, StringComparison.Ordinal);
                
                var area2 = res.Substring(index1 + 1, index2 - index1 - 1);
                var str2 = area + ChineseToNum(area2);
                res = res.Replace(str, str2);
            }
            else
            {
                ruler = new Regex("第.+阶梯");
                room = ruler.Match(res);
                if (room.Length != 0)
                {
                    var str = room.Value;
                    var index1 = res.IndexOf(str, StringComparison.Ordinal);
                    var index2 = res.IndexOf("阶", index1, StringComparison.Ordinal);
                    var area = res.Substring(index1 + 1, index2 - index1 - 1);
                    var str2 = ChineseToNum(area) + "阶";
                    res = res.Replace(str, str2);
                }
                
            }

            return res.Replace('-', '\n').Replace('#', '\n');
        }
        public override string SimplifyName(string roomName)
        {
            string res;
            try
            {
                res = _SimplifyName(roomName);
            }
            catch
            {
                res = roomName;
            }

            return res;
        }
    }
}
