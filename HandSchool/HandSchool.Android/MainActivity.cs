using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using HandSchool.Internal;
using Java.Lang;
using Java.Net;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace HandSchool.Droid
{
    [Activity(Label = "掌上校园", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Context ActivityContext;
        private string[] Arvgs;
        public EventHandler<DialogClickEventArgs> OnClick;
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Internal.Helper.DataBaseDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Internal.Helper.SegoeMDL2 = "segmdl2.ttf#Segoe MDL2 Assets";
            Internal.Helper.AndroidContext = this;
            ActivityContext = this;
            OnClick += RuninstallApk;
            Task.Run(Update);
            LoadApplication(new App() {});
        }
        #region Update
        public string GetVersionCode(Context context)
        {
            PackageManager packageManager = context.PackageManager;
            PackageInfo packageInfo;
            string versionCode = "";
            try
            {
                packageInfo = packageManager.GetPackageInfo(context.PackageName, 0);
                versionCode = packageInfo.VersionCode + "";
            }
            catch (PackageManager.NameNotFoundException e)
            {
                e.PrintStackTrace();
            }
            return versionCode;
        }
        private async Task<string>  GetUpdateString()
        {
            AwaredWebClient awaredWebClient = new AwaredWebClient("https://raw.githubusercontent.com/miasakachenmo/store/master/Update.txt", Encoding.UTF8);
            string a;
            a=await awaredWebClient.DownloadStringTaskAsync("");
            return a;
        }
        public async Task Update()
        {
            string UpdateMsg = await GetUpdateString();
            Arvgs= Regex.Split(UpdateMsg, "\\s+", RegexOptions.IgnoreCase);
            URL url = new URL(Arvgs[1]);//urlToDownload 下载文件的url地址
            string NewVersionCode = Arvgs[0];
            string VersionCode = GetVersionCode(this);
            if (int.Parse(NewVersionCode) > int.Parse(VersionCode))
            {
                RunOnUiThread(()=> {
                    string Detail = "";
                    for (int i = 2; i < Arvgs.Length; i++)
                        Detail += (Arvgs[i] + "\n");
                    AlertDialog.Builder Alert = new AlertDialog.Builder(this);
                    Alert.SetTitle("应用更新");
                    Alert.SetMessage(Detail);
                    Alert.SetNegativeButton("取消",(IDialogInterfaceOnClickListener) null);
                    Alert.SetPositiveButton("确认", OnClick);
                    Alert.Show();
                    return;
                });
            }
            return;
         }
        private  void RuninstallApk(System.Object sender, DialogClickEventArgs e)//必须要在用thread开的线程里..wtf
        {
            new Thread(InstallApk).Start();
        }
        private void  InstallApk ()
        {
            URL url = new URL(Arvgs[1]);
            int receivedBytes = 0;
            int totalBytes = 0;
            string dirPath = "/sdcard/updateVersion/version";
            var filePath = Path.Combine(dirPath, "com.x90yang.HandSchool.apk");
            HttpURLConnection conn = (HttpURLConnection)url.OpenConnection();
            conn.Connect();
            Stream Ins = conn.InputStream;
            totalBytes = conn.ContentLength;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            else
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            using (FileStream fos = new FileStream(filePath, FileMode.Create))
            {
                byte[] buf = new byte[512];

                do
                {
                    int numread = Ins.Read(buf, 0, 512);
                    receivedBytes += numread;
                    if (numread <= 0)
                    {
                        break;
                    }
                    fos.Write(buf, 0, numread);

                    Log.Debug("接收", receivedBytes.ToString() + "," + "总共" + totalBytes.ToString());
                    //进度条代码
                    /*
                    if (progessReporter != null)
                    {
                        DownloadBytesProgress args = new DownloadBytesProgress(urlToDownload, receivedBytes, totalBytes);
                        progessReporter.Report(args);
                    }
                    */
                } while (true);
            }
            var context = this;
            if (context == null)
                return;
            // 通过Intent安装APK文件
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.Parse("file://" + filePath), "application/vnd.android.package-archive");
            //Uri content_url = Uri.Parse(filePath);
            //intent.SetData(content_url);
            intent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
            Log.Debug("exception", "TargetInvocationException in update");
            return;
        }
    }
    #endregion
}
