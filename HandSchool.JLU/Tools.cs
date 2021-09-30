using HandSchool.Internals;
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
        public static List<RecordInfo> AnalyzeHTMLToRecordInfos(string htmlSources)
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
        public static string AnalyzeHTMLToPhara(string htmlSources)
        {
            if (htmlSources.Contains("<table>")) return "通知含有表格，不支持表格解析";
            var html = new HtmlDocument();
            html.LoadHtml(htmlSources.Trim());
            var spans = html.DocumentNode.SelectNodes("//div[@class='content_font fontsize immmge']//span");
            var ps = html.DocumentNode.SelectNodes("//div[@class='content_font fontsize immmge']//p");
            if (ps == null)
            {
                ps = html.DocumentNode.SelectNodes("//div[@class='content_font fontsize immmge']//text()");
            }
            try
            {
                var sb = new StringBuilder();
                var sp = new StringBuilder();
                foreach (var text in ps)
                {
                    sb.Append(text.InnerText.Replace("&nbsp;", "").Replace("&nbsp", "").Trim()).Append('\n');
                }
                foreach (var text in spans)
                {
                    sp.Append(text.InnerText.Replace("&nbsp;", "").Replace("&nbsp", "").Trim()).Append('\n');
                }
                if (sb.Length < sp.Length / 4) return sp.ToString();
                return sb.ToString().Trim();
            }
            catch
            {
                try
                {
                    var texts = html.DocumentNode.SelectNodes("//div[@class='content_font fontsize immmge']//node()[not(node())]");
                    var stringBuilder = new StringBuilder();
                    foreach (var i in texts)
                    {
                        var text = i.InnerText.Replace("&nbsp;", "").Replace("&nbsp", "").Trim();
                        if (!string.IsNullOrEmpty(text))
                            stringBuilder.Append(" ").Append(text).Append('\n');
                    }
                    return stringBuilder.ToString();
                }
                catch
                {
                    return "解析引擎出现错误";
                }
            }
        }
        public static Thread GetThreadAddVpnCookie(this IWebClient webClient)
        {
            return new Thread(new ThreadStart(() =>
            {
                if (!Loader.UseVpn) return;
                while (Loader.Vpn == null || (Loader.Vpn != null && !Loader.Vpn.IsLogin))
                    Thread.Sleep(100);
                webClient.Cookie.Add(new Uri("https://vpns.jlu.edu.cn"), new System.Net.Cookie("remember_token", Loader.Vpn.RememberToken, "/"));
            }));
        }


    }
    public class JLUClassSimplifier : ClassInfoSimplifier
    {

        static Dictionary<string, int> chineseNums = null;
        static void InitCnNums()
        {
            chineseNums = new Dictionary<string, int>();
            chineseNums.Add("零", 0);
            chineseNums.Add("一", 1);
            chineseNums.Add("二", 2);
            chineseNums.Add("三", 3);
            chineseNums.Add("四", 4);
            chineseNums.Add("五", 5);
            chineseNums.Add("六", 6);
            chineseNums.Add("七", 7);
            chineseNums.Add("八", 8);
            chineseNums.Add("九", 9);
            chineseNums.Add("十", 10);
            chineseNums.Add("十一", 11);
            chineseNums.Add("十二", 12);
            chineseNums.Add("十三", 13);
            chineseNums.Add("十四", 14);
            chineseNums.Add("十五", 15);
        }
        static string ChineseToNum(string str)
        {
            if (chineseNums == null) InitCnNums();
            if (chineseNums.ContainsKey(str)) return chineseNums[str].ToString();
            return str;
        }
        public override string SimplifyName(string roomName)
        {
            var ruler = new Regex("第.+-.+周");
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


            ruler = new Regex("[A-Za-z0-9]区第.+阶梯");
            var room = ruler.Match(res);
            if (room.Length != 0)
            {
                var str = room.Value;
                char area = str[str.IndexOf("区") - 1];
                var index1 = str.IndexOf("第");
                var index2 = str.IndexOf("阶");
                var area2 = str.Substring(index1 + 1, index2 - index1 - 1);
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
                    var index1 = str.IndexOf("第");
                    var index2 = str.IndexOf("阶");
                    var area2 = str.Substring(index1 + 1, index2 - index1 - 1);
                    var str2 = ChineseToNum(area2) + "阶";
                    res = res.Replace(str, str2);
                }
                else
                {
                    ruler = new Regex("第.+?教学楼");
                    room = ruler.Match(roomName);
                    if (room.Length != 0)
                    {
                        var str = room.Value;
                        var index1 = str.IndexOf("第");
                        var index2 = str.IndexOf("教学楼");
                        var area2 = str.Substring(index1 + 1, index2 - index1 - 1);
                        var str2 = area2 + "教";
                        res = res.Replace(str, str2);
                    }
                }
            }

            return res.Replace("教学楼", "楼").Replace('-', '\n').Replace('#', '\n');
        }
    }
}
