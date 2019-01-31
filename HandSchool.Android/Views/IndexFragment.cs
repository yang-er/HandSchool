using Android.Content;
using DanielStone.MaterialAboutLibrary;
using DanielStone.MaterialAboutLibrary.Items;
using DanielStone.MaterialAboutLibrary.Models;
using HandSchool.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;

namespace HandSchool.Droid
{
    public class IndexFragment : MaterialAboutFragment, IViewCore
    {
        public string Title { get; set; }

        public BaseViewModel ViewModel { get; set; }

        public IndexFragment()
        {
            Title = "关于";
            ViewModel = AboutViewModel.Instance;
        }

        protected override MaterialAboutList GetMaterialAboutList(Context context)
        {
            var builderApp = new MaterialAboutList.Builder();
            BuildApp(builderApp, context);
            return builderApp.Build();
        }

        private void BuildApp(MaterialAboutList.Builder builder, Context context)
        {
            builder.AddCard( (new MaterialAboutCard.Builder()
                .AddItem(new MaterialAboutTitleItem.Builder()
                .Text("掌上校园")
                .Desc("描述述")
                .Icon(Resource.Drawable.abouticon)
                .Build())
            .AddItem(new MaterialAboutActionItem.Builder()
                .Text("版本")
                .SubText(Core.Version)
                .Icon(Resource.Drawable.aboutpage_versionicon)
                .Build())
            .AddItem(new MaterialAboutActionItem.Builder()
                .Text("检查更新")
                .Icon(Resource.Drawable.aboutpage_updateicon)
                .SetOnClickAction(new AboutMenuItemClick(Core.Platform.CheckUpdate))
                .Build())).Build());
            
            builder.AddCard((new MaterialAboutCard.Builder()
                .AddItem(new MaterialAboutActionItem.Builder()
                .Text("源代码")
                .Icon(Resource.Drawable.aboutpage_codeicon)
                .SetOnClickAction(new AboutMenuItemClick(()=> { Device.OpenUri(new System.Uri("https://github.com/yang-er/HandSchool")); }))
                .Build())
            .AddItem(new MaterialAboutActionItem.Builder()
                .Text("开放源代码许可")
                .Icon(Resource.Drawable.aboutpage_githubicon)
                .Build())).Build());
            builder.AddCard(new MaterialAboutCard.Builder()
                .AddItem(new MaterialAboutActionItem.Builder()
                .Text("评分")
                .SubText("_(:з)∠)_")
                .SetOnClickAction(new AboutMenuItemClick(() => { Device.OpenUri(new System.Uri("https://www.coolapk.com/apk/com.x90yang.HandSchool")); }))
                .Icon(Resource.Drawable.aboutpage_rateicon)
                .Build()).Build());


        }
    }
}