using Android.App;
using Android.Content;
using Android.Content.PM;
using HandSchool.Internal;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.Droid
{
    /// <summary>
    /// 更新管理器，提供更新通知的服务。
    /// </summary>
    public class UpdateManager
    {
        const string UpdateSource = "https://raw.githubusercontent.com/" +
            "yang-er/HandSchool/master/HandSchool/HandSchool.Android/";

        public UpdateManager(Context context)
        {
            Context = context;
        }

        public Context Context { get; set; }

        private string[] Arvgs;
        
        public int GetVersionCode()
        {
            int versionCode = 999;

            try
            {
                versionCode = Context.PackageManager
                    .GetPackageInfo(Context.PackageName, 0)
                    .VersionCode;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                e.PrintStackTrace();
            }

            return versionCode;
        }

        private async Task<string> GetUpdateString()
        {
            try
            {
                using (var wc = new AwaredWebClient(UpdateSource, Encoding.UTF8))
                {
                    wc.Timeout = 3000;
                    return await wc.DownloadStringTaskAsync("version.txt");
                }
            }
            catch (WebException)
            {
                return "";
            }
        }

        public async void Update(bool displayNone = false, Context context = null)
        {
            context = context ?? Context;
            string UpdateMsg = await GetUpdateString();
            if (UpdateMsg == "") return;
            Arvgs = UpdateMsg.Split(new[] { ' ' }, 3, StringSplitOptions.None);

            if (int.Parse(Arvgs[0]) > GetVersionCode())
            {
                Core.Platform.EnsureOnMainThread(() =>
                {
                    string Detail = Arvgs[2];
                    new AlertDialog.Builder(context)
                        .SetTitle("应用更新")
                        .SetMessage(Detail)
                        .SetNegativeButton("取消", (IDialogInterfaceOnClickListener)null)
                        .SetPositiveButton("确认", (s, e) => Core.Platform.OpenUrl(Arvgs[1]))
                        .Show();
                });
            }
            else if (displayNone)
            {
                Core.Platform.EnsureOnMainThread(() =>
                {
                    new AlertDialog.Builder(context)
                        .SetTitle("应用更新")
                        .SetMessage("您的应用已经是最新的啦！")
                        .SetNegativeButton("确认", (IDialogInterfaceOnClickListener)null)
                        .Show();
                });
            }
        }
    }
}