using HandSchool.Views;
using System;
using Xamarin.Forms;
using iOS_Nav = Xamarin.Forms.PlatformConfiguration.iOSSpecific.NavigationPage;
using iOS_Page = Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page;

namespace HandSchool.Internal
{
    public static class PlatformExtensions
    {
        public static readonly BindableProperty ShowLoadingProperty =
            BindableProperty.Create(
                propertyName: nameof(ShowLoading),
                returnType: typeof(bool),
                declaringType: typeof(ViewPage),
                defaultValue: false);

        public static readonly BindableProperty ShowLeftCancelProperty =
            BindableProperty.Create(
                propertyName: nameof(ShowLeftCancel),
                returnType: typeof(bool),
                declaringType: typeof(ViewPage),
                defaultValue: false);

        public static readonly BindableProperty UseTabletModeProperty =
            BindableProperty.Create(
                propertyName: nameof(UseTabletMode),
                returnType: typeof(bool),
                declaringType: typeof(ViewPage),
                defaultValue: false);

        public static IPlatformElementConfiguration<_iOS_, Page> UseSafeArea(
            this IPlatformElementConfiguration<_iOS_, Page> registry,
            bool use = true)
        {
            registry.Element.SetValue(iOS_Page.UseSafeAreaProperty, use);
            return registry;
        }

        public static IPlatformElementConfiguration<_Each_, Page> ShowLoading(
            this IPlatformElementConfiguration<_Each_, Page> registry,
            bool show = true)
        {
            registry.Element.SetValue(ShowLoadingProperty, show);
            return registry;
        }

        public static IPlatformElementConfiguration<_iOS_, Page> ShowLeftCancel(
            this IPlatformElementConfiguration<_iOS_, Page> registry,
            bool show = true)
        {
            registry.Element.SetValue(ShowLeftCancelProperty, show);
            return registry;
        }

        public static IPlatformElementConfiguration<_iOS_, Page> HideFrameShadow(
            this IPlatformElementConfiguration<_iOS_, Page> registry,
            bool show = true)
        {
            var themer = new Style(typeof(Frame));
            themer.Setters.Add(Frame.HasShadowProperty, false);
            registry.Element.Resources.Add(themer);
            return registry;
        }

        public static IPlatformElementConfiguration<_iOS_, Page> UseTabletMode(
            this IPlatformElementConfiguration<_iOS_, Page> registry,
            bool use = true)
        {
            registry.Element.SetValue(UseTabletModeProperty, use);
            return registry;
        }

        public static IPlatformElementConfiguration<_iOS_, Page> StatusTranslucent(
            this IPlatformElementConfiguration<_iOS_, Page> registry,
            bool translucent = true)
        {
            registry.Element.SetValue(iOS_Nav.IsNavigationBarTranslucentProperty, translucent);
            return registry;
        }
    }
    
    public class _iOS_ : IConfigPlatform { }
    public class _Android_ : IConfigPlatform { }
    public class _Each_ : IConfigPlatform { }
}
