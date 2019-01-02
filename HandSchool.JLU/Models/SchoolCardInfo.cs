using HandSchool.Internal;
using System.Text.RegularExpressions;

namespace HandSchool.JLU.Models
{
    /// <summary>
    /// 校园卡信息的储存和解析。
    /// </summary>
    /// <inheritdoc cref="NotifyPropertyChanged" />
    internal class SchoolCardInfo : NotifyPropertyChanged
    {
        private string name = "未知";
        private string cardNo = "不知道";
        private string balance = "不清楚";
        private string bankNo = "不知晓";
        private string gdThis = "听不清";
        private string gdLast = "看不见";
        private string lostState = "可能正确吧";
        private string frozenState = "大概正常吧";
        private string idType = "你先登录";
        private string department = "猜不透";

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string CardNo
        {
            get => cardNo;
            set => SetProperty(ref cardNo, value);
        }

        public string Balance
        {
            get => balance;
            set => SetProperty(ref balance, value);
        }

        public string BankNo
        {
            get => bankNo;
            set => SetProperty(ref bankNo, value);
        }

        public string GdThis
        {
            get => gdThis;
            set => SetProperty(ref gdThis, value);
        }

        public string GdLast
        {
            get => gdLast;
            set => SetProperty(ref gdLast, value);
        }

        public string LostState
        {
            get => lostState;
            set => SetProperty(ref lostState, value);
        }

        public string FrozenState
        {
            get => frozenState;
            set => SetProperty(ref frozenState, value);
        }

        public string IdType
        {
            get => idType;
            set => SetProperty(ref idType, value);
        }

        public string Department
        {
            get => department;
            set => SetProperty(ref department, value);
        }

        public void ParseFromHtml(string html)
        {
            html = html.Replace("    ", "")
                       .Replace("\r", "")
                       .Replace("\n", "");

            Name = Regex.Match(html, @"<td class=""first"">姓名</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            CardNo = Regex.Match(html, @"<td class=""first"">学工号</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            Balance = Regex.Match(html, @"<td class=""first"">校园卡余额</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            BankNo = Regex.Match(html, @"<td class=""first"">银行卡号</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            GdThis = Regex.Match(html, @"<td class=""first"">当前过渡余额</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            GdLast = Regex.Match(html, @"<td class=""first"">上次过渡余额</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            LostState = Regex.Match(html, @"<td class=""first"">挂失状态</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            FrozenState = Regex.Match(html, @"<td class=""first"">冻结状态</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            IdType = Regex.Match(html, @"<td class=""first"">身份类型</td><td class=""second"">(\S+)</td>").Groups[1].Value;
            Department = Regex.Match(html, @"<td class=""first"">部门名称</td><td class=""second"">(\S+)</td>").Groups[1].Value;
        }
    }
}
