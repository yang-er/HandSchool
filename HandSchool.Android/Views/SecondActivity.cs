using Android.App;
using Android.OS;
using HandSchool.Views;
using System;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class SecondActivity : BaseActivity
    {
        protected override void OnNavigatedParameter(object obj)
        {
            var param2 = ((Type, object))obj;
            var type = NavMenuItemV2.Judge(param2.Item1);

            switch (type)
            {
                case NavMenuItemType.FragmentCore:
                    var fg = Core.Reflection.CreateInstance<SupportFragment>(param2.Item1);
                    Transaction(fg);
                    break;
                case NavMenuItemType.Fragment:
                    var vf = Core.Reflection.CreateInstance<ViewFragment>(param2.Item1);
                    vf.SetNavigationArguments(param2.Item2);
                    Transaction(vf);
                    break;
                case NavMenuItemType.Object:
                    var vo = Core.Reflection.CreateInstance<ViewObject>(param2.Item1);
                    vo.SetNavigationArguments(param2.Item2);
                    Transaction(vo);
                    break;
                default:
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ContentViewResource = Resource.Layout.activity_popup;
            base.OnCreate(savedInstanceState);
            var bar = SupportActionBar;
            bar.SetDisplayHomeAsUpEnabled(true);
            bar.SetHomeButtonEnabled(true);
        }
    }
}