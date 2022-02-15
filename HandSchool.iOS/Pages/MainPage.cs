using System.Threading.Tasks;
using HandSchool.Internals;
using HandSchool.iOS;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MainPage2 : FlyoutPage
    {
        [Settings("使用平板模式", "将选项与页面并列显示")] public static bool UseTablet { get; set; }
        public static bool IsTablet { get; private set; }
        public MainPage2()
        {
            IsTablet = Device.Idiom == TargetIdiom.Tablet;
            Title = "掌上校园";
            NavigationViewModel.Instance.ToString();
            UseTablet = IsTablet && Core.Configure.Configs.GetItemWithPrimaryKey(nameof(UseTablet))?.Value?.ToLower() == "true";
            if (IsTablet)
            {
                SettingViewModel.Instance.Items.Add(
                    new SettingWrapper(typeof(MainPage2).GetProperty(nameof(UseTablet))));
                SettingViewModel.OnSaveSettings += () =>
                {
                    Core.Configure.Configs.InsertOrUpdateTable(new Config
                    {
                        ConfigName = nameof(UseTablet),
                        Value = UseTablet.ToString()
                    });
                    return Task.CompletedTask;
                };
            }

            var menu = new FlyoutNavMenu();
            Flyout = menu;
            Detail = PlatformImpl.Instance.MainNavigationMenu[0].GetNavigationPage();
            FlyoutLayoutBehavior = IsTablet && UseTablet
                ? FlyoutLayoutBehavior.SplitOnPortrait
                : FlyoutLayoutBehavior.Default;
            IsPresented = IsTablet && UseTablet;
            menu.ItemSelected += (s, e) =>
            {
                if (e.CurrentSelection.Count > 0)
                {
                    if (e.CurrentSelection[0] is NavMenuItemImpl navMenuItemImpl)
                    {
                        Detail = navMenuItemImpl.GetNavigationPage();
                        if (FlyoutLayoutBehavior != FlyoutLayoutBehavior.SplitOnPortrait)
                        {
                            IsPresented = false;
                        }
                    }
                }
            };
        }
    }
}