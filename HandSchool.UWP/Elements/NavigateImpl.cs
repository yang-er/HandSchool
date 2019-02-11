using HandSchool.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using WFrame = Windows.UI.Xaml.Controls.Frame;

namespace HandSchool.UWP
{
    /// <summary>
    /// 针对 WFrame 实现的导航。
    /// </summary>
    internal class NavigateImpl : INavigate
    {
        public static NavigateImpl Impl { get; private set; }

        /// <summary>
        /// 程序导航使用的Frame
        /// </summary>
        public WFrame InnerFrame { get; }
        
        /// <summary>
        /// 为导航页面使用
        /// </summary>
        /// <param name="inner"></param>
        public NavigateImpl(WFrame inner)
        {
            InnerFrame = inner;
            InnerFrame.Navigated += Frame_Navigated;
            Impl = this;
        }

        /// <summary>
        /// 当Frame发生导航事件时，更新栈列表。
        /// </summary>
        /// <param name="sender">发送导航的Frame</param>
        /// <param name="args">导航时发生的事件</param>
        private void Frame_Navigated(object sender, NavigationEventArgs args)
        {
            Debug.Assert(typeof(ViewPage).IsAssignableFrom(args.SourcePageType));
            var currentPage = args.Content as IViewLifecycle;
            currentPage.RegisterNavigation(this);
        }
        
        public Task PushAsync(string pageType, object param)
        {
            var type = Core.Reflection.TryGetType(pageType);

            if (type is null)
            {
                Core.Logger.WriteLine("NavImpl", pageType + " not found.");
                return Task.CompletedTask;
            }
            
            return PushAsync(type, param);
        }

        public Task PushAsync(Type pageType, object param)
        {
            pageType = Core.Reflection.TryGetType(pageType);

            if (typeof(ViewPage).IsAssignableFrom(pageType))
            {
                InnerFrame.Navigate(pageType, param);
            }
            else if (typeof(ViewObject).IsAssignableFrom(pageType))
            {
                InnerFrame.Navigate(typeof(PackagedPage), new Tuple<Type, object>(pageType, param));
            }

            return Task.CompletedTask;
        }
    }
}
