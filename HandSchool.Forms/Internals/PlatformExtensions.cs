using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
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

        public static PlatformRegistry<iOS, ViewPage> UseSafeArea(
            this PlatformRegistry<iOS, ViewPage> registry,
            bool use = true)
        {
            registry.Element.SetValue(iOS_Page.UseSafeAreaProperty, use);
            return registry;
        }

        public static PlatformRegistry<Each, ViewPage> ShowLoading(
            this PlatformRegistry<Each, ViewPage> registry,
            bool show = true)
        {
            registry.Element.SetValue(ShowLoadingProperty, show);
            return registry;
        }

        public static PlatformRegistry<iOS, ViewPage> ShowLeftCancel(
            this PlatformRegistry<iOS, ViewPage> registry,
            bool show = true)
        {
            registry.Element.SetValue(ShowLeftCancelProperty, show);
            return registry;
        }

        public static PlatformRegistry<iOS, ViewPage> HideFrameShadow(
            this PlatformRegistry<iOS, ViewPage> registry,
            bool show = true)
        {
            var themer = new Style(typeof(Frame));
            themer.Setters.Add(Frame.HasShadowProperty, false);
            registry.Element.Resources.Add(themer);
            return registry;
        }

        public static PlatformRegistry<iOS,ViewPage> UseTabletMode(
            this PlatformRegistry<iOS, ViewPage> registry,
            bool use = true)
        {
            registry.Element.SetValue(UseTabletModeProperty, use);
            return registry;
        }

        public static PlatformRegistry<TPlatform, TElement>
            On<TPlatform, TElement>(this TElement element)
            where TPlatform : IPlatform
            where TElement : Element
        {
            return new PlatformRegistry<TPlatform, TElement>(element);
        }
    }

    public interface IPlatform { }
    public class iOS : IPlatform { }
    public class Android : IPlatform { }
    public class Each : IPlatform { }

    public struct PlatformRegistry<TPlatform, TElement>
        where TPlatform : IPlatform
        where TElement : Element
    {
        public TElement Element { get; }

        public PlatformRegistry(TElement element)
        {
            Element = element;
        }
    }
}
