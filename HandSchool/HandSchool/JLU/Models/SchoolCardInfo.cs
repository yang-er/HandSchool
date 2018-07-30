using HandSchool.Internal;
using System.Text.RegularExpressions;

namespace HandSchool.JLU.Models
{
    class SchoolCardInfo : NotifyPropertyChanged
    {
        private string _name = "登录后可见";
        private string _cardno = "登录后可见";
        private string _balance = "登录后可见";
        private string _bankno = "登录后可见";
        private string _gdthis = "登录后可见";
        private string _gdlast = "登录后可见";
        private string _loststate = "登录后可见";
        private string _frozstate = "登录后可见";
        private string _idtype = "登录后可见";
        private string _department = "登录后可见";

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string CardNo
        {
            get => _cardno;
            set => SetProperty(ref _cardno, value);
        }

        public string Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }

        public string BankNo
        {
            get => _bankno;
            set => SetProperty(ref _bankno, value);
        }

        public string GdThis
        {
            get => _gdthis;
            set => SetProperty(ref _gdthis, value);
        }

        public string GdLast
        {
            get => _gdlast;
            set => SetProperty(ref _gdlast, value);
        }

        public string LostState
        {
            get => _loststate;
            set => SetProperty(ref _loststate, value);
        }

        public string FrozenState
        {
            get => _frozstate;
            set => SetProperty(ref _frozstate, value);
        }

        public string IdType
        {
            get => _idtype;
            set => SetProperty(ref _idtype, value);
        }

        public string Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
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
