﻿using HandSchool.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.iOS
{
    /// <summary>
    /// 基于Xamarin.Forms的导航实现类。
    /// </summary>
    /// <inheritdoc cref="INavigate" />
    internal class NavigateImpl : INavigate
    {
        public NavigateImpl(Page navPage)
        {
            InnerNavigation = navPage.Navigation;
            
            // navPage.Pushed += NavigationOccured;
        }

        public void NavigationOccured(object sender, NavigationEventArgs args)
        {
            if (args.Page is TabbedPage tabbed)
            {
                foreach (IViewLifecycle core in tabbed.Children)
                {
                    core.RegisterNavigation(this);
                }
            }
            else if (args.Page is ViewObject core)
            {
                core.RegisterNavigation(this);
            }
        }

        private INavigation InnerNavigation { get; set; }

        public Task PushAsync(object view, object param)
        {
            if (view is Page page)
            {
                if (page is IViewLifecycle vlc)
                    vlc.SetNavigationArguments(param);
                return InnerNavigation.PushAsync(page);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        
        public Task PushAsync(Type pageType, object param)
        {
            pageType = Core.Reflection.TryGetImpl(pageType);

            if (typeof(Page).IsAssignableFrom(pageType))
            {
                var page = Core.Reflection.CreateInstance<Page>(pageType);
                if (page is IViewLifecycle vlc)
                    vlc.SetNavigationArguments(param);
                return InnerNavigation.PushAsync(page);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public Task<bool> PopAsync()
        {
            try
            {
                InnerNavigation.PopAsync();
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}