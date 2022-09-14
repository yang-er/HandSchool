using System.Collections.Generic;
using HandSchool.Models;

namespace HandSchool.JLU.Services
{
    public static class LoginFiledExtend
    {
        private static readonly HashSet<ILoginField> LoginFields = new HashSet<ILoginField>();

        /// <summary>
        /// 当WebVpn掉线重连时，需要使表单退出登录
        /// </summary>
        public static void BindingVpnLoginState(this ILoginField loginField)
        {
            if (LoginFields.Contains(loginField)) return;
            if (!Vpn.UseVpn) return;
            LoginFields.Add(loginField);
            Vpn.Instance.LoginStateChanged += (s, e) =>
            {
                if (e.State != LoginState.Logout) return;
                var isLogin = loginField.GetType().GetDeclaredProperty("IsLogin");
                var needLogin = loginField.GetType().GetDeclaredProperty("NeedLogin");
                isLogin?.Let(p =>
                {
                    if (p.CanWrite)
                        p.SetValue(loginField, false);
                });
                needLogin?.Let(p =>
                {
                    if (p.CanWrite)
                        p.SetValue(loginField, true);
                });
            };
        }
    }
}