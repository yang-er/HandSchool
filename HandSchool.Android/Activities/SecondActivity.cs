using Android.App;
using HandSchool.Views;
using System;
using Android.Content.PM;
using SupportFragment = AndroidX.Fragment.App.Fragment;

namespace HandSchool.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [BindView(Resource.Layout.activity_popup)]
    public class SecondActivity : BaseActivity
    {
        protected override void OnNavigatedParameter(object obj)
        {
            switch (obj)
            {
                case SupportFragment fragment:
                    Transaction(fragment);
                    break;
                case ViewObject viewObject:
                    Transaction(viewObject);
                    break;
                case ValueTuple<Type, object> typeAndParams:
                    NavTypeAndParams(typeAndParams);
                    break;
                case ValueTuple<object, object> instanceAndParams:
                    NavInstanceAndParams(instanceAndParams);
                    break;
            }

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        private void NavTypeAndParams((Type, object) typeAndParams)
        {
            var (pageType, @params) = typeAndParams;
            var type = NavMenuItemV2.Judge(pageType);

            switch (type)
            {
                case NavMenuItemType.FragmentCore:
                    var fg = Core.Reflection.CreateInstance<SupportFragment>(pageType);
                    Transaction(fg);
                    break;

                case NavMenuItemType.Fragment:
                    var vf = Core.Reflection.CreateInstance<ViewFragment>(pageType);
                    vf.SetNavigationArguments(@params);
                    Transaction(vf);
                    break;

                case NavMenuItemType.Object:
                    var vo = Core.Reflection.CreateInstance<ViewObject>(pageType);
                    vo.SetNavigationArguments(@params);
                    Transaction(vo);
                    break;

                case NavMenuItemType.Presenter:
                    var vp = Core.Reflection.CreateInstance<IViewPresenter>(pageType);
                    Transaction(new TabbedFragment(vp));
                    break;
            }
        }

        private void NavInstanceAndParams((object, object) instanceAndParams)
        {
            var (instance, @params) = instanceAndParams;
            var type = NavMenuItemV2.Judge(instance.GetType());

            switch (type)
            {
                case NavMenuItemType.FragmentCore:
                    var fg = (SupportFragment) instance;
                    Transaction(fg);
                    break;

                case NavMenuItemType.Fragment:
                    var vf = (ViewFragment) instance;
                    vf.SetNavigationArguments(@params);
                    Transaction(vf);
                    break;

                case NavMenuItemType.Object:
                    var vo = (ViewObject) instance;
                    vo.SetNavigationArguments(@params);
                    Transaction(vo);
                    break;

                case NavMenuItemType.Presenter:
                    Transaction(new TabbedFragment((IViewPresenter) instance));
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var fragment in SupportFragmentManager.Fragments)
            {
                fragment.OnDestroy();
                fragment.Dispose();
            }
        }
    }
}