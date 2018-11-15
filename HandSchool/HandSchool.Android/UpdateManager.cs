using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Util;
using HandSchool.Internal;
using Java.Lang;
using Java.Net;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HandSchool.Droid
{
    public class UpdateManager
    {
        public UpdateManager(MainActivity activity)
        {
            context = activity;
        }

        private MainActivity context;
#pragma warning disable 0618
        private ProgressDialog ProgressDialog;
#pragma warning restore
        private string[] Arvgs;

        private void OnClick(object sender, DialogClickEventArgs args)
        {
#pragma warning disable 0618
            ProgressDialog = new ProgressDialog(context);
#pragma warning restore
            new Thread(InstallApk).Start();
        }

        public int GetVersionCode()
        {
            PackageManager packageManager = context.PackageManager;
            PackageInfo packageInfo;
            int versionCode = 999;
            try
            {
                packageInfo = packageManager.GetPackageInfo(context.PackageName, 0);
                versionCode = packageInfo.VersionCode;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                e.PrintStackTrace();
            }
            return versionCode;
        }

        private async Task<string> GetUpdateString()
        {
            AwaredWebClient wc = new AwaredWebClient("https://raw.githubusercontent.com/yang-er/HandSchool/master/HandSchool/HandSchool.Android/", Encoding.UTF8);

            try
            {
                return await wc.DownloadStringTaskAsync("version.txt");
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
                context.RunOnUiThread(() => {
                    string Detail = Arvgs[2];
                    AlertDialog.Builder Alert = new AlertDialog.Builder(context);
                    Alert.SetTitle("应用更新");
                    Alert.SetMessage(Detail);
                    Alert.SetNegativeButton("取消", (IDialogInterfaceOnClickListener)null);
                    Alert.SetPositiveButton("确认", OnClick);
                    Alert.Show();
                    return;
                });
            }
        }

        private void InstallApk()
        {
            URL url = new URL(Arvgs[1]);
            int receivedBytes = 0;
            int totalBytes = 0;
            string dirPath = "/sdcard/Android/data/com.x90yang.com/files";
            var filePath = Path.Combine(dirPath, $"com.x90yang.HandSchool_v{Arvgs[0]}.apk");
            ProgressDialog.SetTitle("下载更新包");
            ProgressDialog.SetProgressStyle(ProgressDialogStyle.Horizontal);
            HttpURLConnection conn = (HttpURLConnection)url.OpenConnection();
            conn.Connect();
            Stream Ins = conn.InputStream;
            totalBytes = conn.ContentLength;
            ProgressDialog.SetMessage($"正在下载更新包，共 {(totalBytes / 1048576.0).ToString("F")} MB……");
            ProgressDialog.Max = totalBytes / 1024;
            context.RunOnUiThread(ProgressDialog.Show);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            else if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream fos = new FileStream(filePath, FileMode.Create))
            {
                byte[] buf = new byte[65536];

                do
                {
                    int numread = Ins.Read(buf, 0, 65536);
                    receivedBytes += numread;
                    if (numread <= 0)
                    {
                        break;
                    }
                    fos.Write(buf, 0, numread);

                    Log.Debug("Received ", receivedBytes.ToString() + "bytes, together with " + totalBytes.ToString());
                    ProgressDialog.Progress = receivedBytes / 1024;

                } while (true);
            }

            context.RunOnUiThread(ProgressDialog.Dismiss);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.Parse("file://" + filePath), "application/vnd.android.package-archive");
            intent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
            Log.Debug("exception", "TargetInvocationException in update");
        }
    }
}
