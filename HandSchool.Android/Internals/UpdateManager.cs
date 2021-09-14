using Android.App;
using Android.Content;
using Android.Content.PM;
using HandSchool.Internals;
using System;
using System.Threading.Tasks;

namespace HandSchool.Droid
{
    /// <summary>
    /// 更新管理器，提供更新通知的服务。
    /// </summary>
    public class UpdateManager
    {
        private IWebClient UpdateClient { get; set; }
        Context Activity { get; set; }
        public UpdateManager(Context context)
        {
            Activity = context;
        }
        bool IsLatestVersion(string latest)
        {
            var cur = Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0).VersionName;
            var curNums = cur.Split('.');
            var lastestNums = latest.Split('.');
            var len = Math.Min(curNums.Length, lastestNums.Length);
            for (var i = 0; i < len; i++)
            {
                if (int.Parse(lastestNums[i]) > int.Parse(curNums[i])) return false;
            }
            return true;
        }
        public void GetUpdateClient()
        {
            UpdateClient = Core.New<IWebClient>();
            UpdateClient.Timeout = 5000;
        }
        public async Task<AlertDialog> CheckUpdate()
        {
            if (UpdateClient != null) UpdateClient.Dispose();
            GetUpdateClient();
            try
            {
                var str = await UpdateClient.GetStringAsync("https://www.coolapk.com/apk/com.x90yang.HandSchool");
                var updateHtml = new HtmlAgilityPack.HtmlDocument();
                updateHtml.LoadHtml(str);
                var version = updateHtml.DocumentNode.SelectSingleNode("//span[@class='list_app_info']").InnerText.Trim();
                if (IsLatestVersion(version))
                {
                    return new AlertDialog.Builder(Activity)
                    .SetTitle("提示")
                    .SetMessage("已经是最新版了")
                    .SetPositiveButton("好", listener: null).Create();
                }
                else
                {
                    return new AlertDialog.Builder(Activity)
                        .SetTitle("提示")
                        .SetMessage("检测到新版本" + version + "\n点按更新前往酷安下载更新")
                        .SetPositiveButton("更新", (s, e) =>
                        {
                            Core.Platform.OpenUrl("https://www.coolapk.com/apk/com.x90yang.HandSchool");
                        })
                        .SetNegativeButton("取消", listener: null).Create();
                }
            }
            catch
            {
                return new AlertDialog.Builder(Activity).SetTitle("提示")
                    .SetMessage("出了点问题，请稍后再试").Create();
            }
            finally
            {
                UpdateClient.Dispose();
                UpdateClient = null;
            }
        }
    }

}