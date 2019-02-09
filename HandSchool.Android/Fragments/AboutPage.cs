using Android.Content;
using Android.OS;
using Android.Views;
using DanielStone.MaterialAbout;
using DanielStone.MaterialAbout.Items;
using DanielStone.MaterialAbout.Models;
using HandSchool.Droid;
using HandSchool.Internals;
using HandSchool.ViewModels;
using Resource = HandSchool.Droid.Resource;

namespace HandSchool.Views
{
    public class AboutPage : MaterialAboutFragment, IViewCore, IViewLifecycle
    {
        #region IViewCore Impl

        public string Title { get; set; }

        public BaseViewModel ViewModel { get; set; }

        public ToolbarMenuTracker ToolbarTracker { get; }

        public bool IsBusy { get; set; }

        public AboutPage()
        {
            Title = "关于";
            ViewModel = AboutViewModel.Instance;
        }

        #endregion

        #region Small Functions

        private void OpenGitHub()
        {
            Core.Platform.OpenUrl("https://github.com/yang-er/HandSchool");
        }

        private async void OpenS1()
        {
            await Navigation.PushAsync<WebViewPage>(new AboutViewModel.LicenseInfo());
        }

        private async void OpenS2()
        {
            await Navigation.PushAsync<WebViewPage>(new AboutViewModel.PrivacyPolicy());
        }

        private void OpenStore()
        {
            Core.Platform.OpenUrl(Core.Platform.StoreLink);
        }

        #endregion

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            view.SetBackgroundColor(Android.Graphics.Color.Argb(255, 244, 244, 244));
        }

        protected override MaterialAboutList GetMaterialAboutList(Context context)
        {
            var builderApp = new MaterialAboutList.Builder();
            BuildApp(builderApp, context);
            BuildShare(builderApp, context);
            BuildDesc(builderApp, context);
            return builderApp.Build();
        }

        private void BuildApp(MaterialAboutList.Builder builder, Context context)
        {
            var title = new MaterialAboutTitleItem.Builder()
                .Text("掌上校园")
                .Desc("HandSchool.Android")
                .Icon(Resource.Drawable.abouticon)
                .Build();

            var version = new MaterialAboutActionItem.Builder()
                .Text("版本")
                .SubText(Core.Version)
                .Icon(Resource.Drawable.aboutpage_updateicon)
                .SetOnClickAction(new AboutMenuItemClick(Core.Platform.CheckUpdate))
                .Build();

            var card = new MaterialAboutCard.Builder()
                .AddItem(title)
                .AddItem(version)
                .Build();

            builder.AddCard(card);
        }

        private void BuildDesc(MaterialAboutList.Builder builder, Context context)
        {
            var source = new MaterialAboutActionItem.Builder()
                .Text("开源项目")
                .Icon(Resource.Drawable.aboutpage_githubicon)
                .SetOnClickAction(new AboutMenuItemClick(OpenGitHub))
                .Build();

            var license = new MaterialAboutActionItem.Builder()
                .Text("开放源代码许可")
                .Icon(Resource.Drawable.aboutpage_codeicon)
                .SetOnClickAction(new AboutMenuItemClick(OpenS1))
                .Build();

            var privacy = new MaterialAboutActionItem.Builder()
                .Text("隐私许可")
                .Icon(Resource.Drawable.aboutpage_privacyicon)
                .SetOnClickAction(new AboutMenuItemClick(OpenS2))
                .Build();

            var card = new MaterialAboutCard.Builder()
                .Title("版权 & 隐私")
                .AddItem(license)
                .AddItem(privacy)
                .AddItem(source)
                .Build();

            builder.AddCard(card);
        }

        private void BuildShare(MaterialAboutList.Builder builder, Context context)
        {
            var share = new MaterialAboutActionItem.Builder()
                .Text("分享给朋友")
                .Icon(Resource.Drawable.aboutpage_shareicon)
                .SetOnClickAction(null)
                .Build();

            var rating = new MaterialAboutActionItem.Builder()
                .Text("评分与评论")
                .SubText("觉得好用请给五星！_(:з)∠)_")
                .SetOnClickAction(new AboutMenuItemClick(OpenStore))
                .Icon(Resource.Drawable.aboutpage_rateicon)
                .Build();

            var group = new MaterialAboutActionItem.Builder()
                .Text("反馈")
                .SubText("QQ群 752277651")
                .Icon(Resource.Drawable.aboutpage_feedbackicon)
                .SetOnClickAction(null)
                .Build();

            var card = new MaterialAboutCard.Builder()
                .Title("分享 & 反馈")
                .AddItem(share)
                .AddItem(rating)
                .AddItem(group)
                .Build();

            builder.AddCard(card);
        }

        #region IViewLifecycle

        public void SendAppearing() { }

        public void SendDisappearing() { }

        public void SetNavigationArguments(object param) { }

        INavigate Navigation { get; set; }

        public void RegisterNavigation(INavigate navigate)
        {
            Navigation = navigate;
        }

        #endregion
    }
}