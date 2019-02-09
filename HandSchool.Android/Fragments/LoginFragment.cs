using Android.Content;
using Android.Graphics;
using Android.OS;
using HandSchool.Internals;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CharSequence = Java.Lang.ICharSequence;

namespace HandSchool.Droid
{
    public class LoginFragment : ViewFragment, ILoginPage
    {
        public LoginViewModel LoginViewModel { get; set; }
        public Task Completed { get; }

        #region UI BindViews

        [BindView(Resource.Id.login_username)]
        public TextInputEditText UsernameBox { get; set; }

        [BindView(Resource.Id.login_password)]
        public TextInputEditText PasswordBox { get; set; }

        [BindView(Resource.Id.login_captcha)]
        public TextInputEditText CaptchaBox { get; set; }

        [BindView(Resource.Id.login_captchaImg)]
        public ImageView CaptchaImage { get; set; }

        [BindView(Resource.Id.login_captchaPanel)]
        public LinearLayout CaptchaPanel { get; set; }

        [BindView(Resource.Id.login_savepwd)]
        public CheckBox SavePasswordBox { get; set; }

        [BindView(Resource.Id.login_autologin)]
        public CheckBox AutoLoginBox { get; set; }

        [BindView(Resource.Id.login_button)]
        public Button LoginButton { get; set; }

        [BindView(Resource.Id.login_tips)]
        public TextView TipsText { get; set; }

        [BindView(Resource.Id.login_progBar)]
        public ProgressBar ProgressBar { get; set; }

        [BindView(Resource.Id.login_cardView)]
        public Android.Support.V7.Widget.CardView CardView { get; set; }

        #endregion
        
        private bool _navArgNotSet;
        
        public LoginFragment()
        {
            FragmentViewResource = Resource.Layout.layout_login;
            Completed = new Task(() => { });
        }

        public async void Response(object sender, LoginStateEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.Processing:
                    await RequestMessageAsync("正在登录", "正在登录中，请稍后……", "知道了");
                    break;

                case LoginState.Succeeded:
                    ((LoginActivity)Context).Finish();
                    break;

                case LoginState.Failed:
                    await RequestMessageAsync("登录失败", $"登录失败，{e.InnerError}。", "知道了");
                    UpdateCaptchaInfomation();
                    break;
            }
        }

        public void SetNavigationArguments(LoginViewModel lvm)
        {
            ViewModel = LoginViewModel = lvm;
            Title = lvm.Title;
            _navArgNotSet = true;
        }

        public Task ShowAsync()
        {
            var context = PlatformImplV2.Instance.ContextStack.Peek();
            var navigate = context as INavigate;
            navigate.PushAsync<LoginActivity>(this);
            return Completed;
        }
        
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            
            LoginViewModel.Form.PropertyChanged += OnSwitchChanged;
            LoginViewModel.PropertyChanged += OnSwitchChanged;
            LoginButton.Click += OnLoginRequested;

            if (_navArgNotSet) UpdateCaptchaInfomation();
        }

        public override void SolveBindings()
        {
            base.SolveBindings();
            UsernameBox.Text = LoginViewModel.Form.Username;
            PasswordBox.Text = LoginViewModel.Form.Password;
            SavePasswordBox.Checked = LoginViewModel.Form.SavePassword;
            AutoLoginBox.Checked = LoginViewModel.Form.AutoLogin;
            TipsText.Text = LoginViewModel.Form.Tips;

            if (LoginViewModel.Form.CaptchaSource == null)
            {
                CaptchaPanel.Visibility = ViewStates.Gone;
                AutoLoginBox.Visibility = ViewStates.Visible;
            }
            else
            {
                CaptchaPanel.Visibility = ViewStates.Visible;
                AutoLoginBox.Visibility = ViewStates.Invisible;
                CaptchaImage.SetImageBitmap(CaptchaBitmap);
            }
        }

        public override void OnDestroyView()
        {
            LoginViewModel.Form.PropertyChanged -= OnSwitchChanged;
            LoginViewModel.PropertyChanged -= OnSwitchChanged;
            LoginButton.Click -= OnLoginRequested;
            base.OnDestroyView();
        }

        private void OnLoginRequested(object sender, EventArgs args)
        {
            LoginViewModel.Form.Username = UsernameBox.Text;
            LoginViewModel.Form.Password = PasswordBox.Text;
            LoginViewModel.Form.CaptchaCode = CaptchaBox.Text;
            LoginViewModel.LoginCommand.Execute(null);
        }

        private void OnSwitchChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "AutoLogin":
                    AutoLoginBox.Checked = LoginViewModel.Form.AutoLogin;
                    break;

                case "SavePassword":
                    SavePasswordBox.Checked = LoginViewModel.Form.SavePassword;
                    break;

                case "IsBusy":
                    ProgressBar.Visibility = LoginViewModel.IsBusy
                        ? ViewStates.Visible : ViewStates.Invisible;
                    LoginButton.Enabled = !LoginViewModel.IsBusy;

                    /* UsernameBox.Enabled = !LoginViewModel.IsBusy;
                    PasswordBox.Enabled = !LoginViewModel.IsBusy;
                    CaptchaBox.Enabled = !LoginViewModel.IsBusy;
                    SavePasswordBox.Enabled = !LoginViewModel.IsBusy;
                    AutoLoginBox.Enabled = !LoginViewModel.IsBusy
                        && LoginViewModel.Form.CaptchaSource == null; */
                    break;
            }
        }

        public Bitmap CaptchaBitmap { get; set; }

        public async void UpdateCaptchaInfomation()
        {
            LoginViewModel.IsBusy = true;
            _navArgNotSet = false;

            if (!await LoginViewModel.Form.PrepareLogin())
            {
                await RequestMessageAsync("登录失败", "登录失败，出现了一些问题。", "知道了");
            }

            if (LoginViewModel.Form.CaptchaSource == null)
            {
                CaptchaPanel.Visibility = ViewStates.Gone;
                AutoLoginBox.Enabled = true;
                if (CaptchaBitmap != null)
                    CaptchaBitmap.Recycle();
                CaptchaBitmap = null;
            }
            else
            {
                CaptchaPanel.Visibility = ViewStates.Visible;
                AutoLoginBox.Enabled = false;

                var src = LoginViewModel.Form.CaptchaSource;
                var orig = await BitmapFactory.DecodeByteArrayAsync(src, 0, src.Length);
                if (CaptchaBitmap != null)
                    CaptchaBitmap.Recycle();
                CaptchaBitmap = Bitmap.CreateScaledBitmap(orig,
                    Context.Dip2Px(99), Context.Dip2Px(33), true);
                orig.Recycle();
                CaptchaImage.SetImageBitmap(CaptchaBitmap);
            }

            LoginViewModel.IsBusy = false;
        }
    }
}