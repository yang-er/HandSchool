using Android.App;
using Android.Content;
using Android.Content.PM;
using HandSchool.Internal;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Droid
{
    public class UpdateManager
    {
        const string UpdateSource = "https://raw.githubusercontent.com/yang-er/HandSchool/master/HandSchool/HandSchool.Android/";

        public UpdateManager(Context activity)
        {
            context = activity;
        }

        private Context context;

        private string[] Arvgs;
        
        public int GetVersionCode()
        {
            int versionCode = 999;

            try
            {
                versionCode = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
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
                    return await wc.DownloadStringTaskAsync("version.txt");
                }
            }
            catch (WebException)
            {
                return "";
            }
        }

        public async void Update()
        {
            string UpdateMsg = await GetUpdateString();
            if (UpdateMsg == "") return;
            Arvgs = UpdateMsg.Split(new char[] { ' ' }, 3, StringSplitOptions.None);

            if (int.Parse(Arvgs[0]) > GetVersionCode())
            {
                Core.Platform.EnsureOnMainThread(() =>
                {
                    string Detail = Arvgs[2];
                    AlertDialog.Builder Alert = new AlertDialog.Builder(context);
                    Alert.SetTitle("应用更新");
                    Alert.SetMessage(Detail);
                    Alert.SetNegativeButton("取消", (IDialogInterfaceOnClickListener)null);
                    Alert.SetPositiveButton("确认", (s, e) => Device.OpenUri(new Uri(Arvgs[1])));
                    Alert.Show();
                });
            }
        }
    }
}
