using Android.Content;
using DanielStone.MaterialAboutLibrary;
using DanielStone.MaterialAboutLibrary.Items;
using DanielStone.MaterialAboutLibrary.Models;
using HandSchool.ViewModels;
using HandSchool.Views;

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
            var builderApp = new MaterialAboutCard.Builder();
            BuildApp(builderApp, context);
            return new MaterialAboutList(builderApp.Build());
        }

        private void BuildApp(MaterialAboutCard.Builder builder, Context context)
        {
            builder.AddItem(new MaterialAboutTitleItem.Builder()
                .Text("掌上校园")
                .Desc("描述述")
                .Icon(Resource.Drawable.abouticon)
                .Build());
            builder.AddItem(new MaterialAboutActionItem.Builder()
                .Text("版本")
                .SubText(Core.Version)
                .Icon(Resource.Drawable.ic_menu_about)
                .SetOnClickAction(new AboutMenuItemClick(Core.Platform.CheckUpdate))
                .Build());
        }
    }
}