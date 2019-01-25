using HandSchool.Internal;
using HandSchool.Views;
using System;
using System.Collections.Generic;
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
        readonly List<IViewPage> stack = new List<IViewPage>();
        TaskCompletionSource<IViewPage> taskCompletionSource;
        public IViewPage CurrentStackTopPage { get; set; }
        public static NavigateImpl Impl { get; private set; }

        /// <summary>
        /// 程序导航使用的Frame
        /// </summary>
        public WFrame InnerFrame { get; }

        /// <summary>
        /// 导航视图栈
        /// </summary>
        public IReadOnlyList<IViewPage> NavigationStack => stack;

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
            Debug.Assert(args.SourcePageType.IsSubclassOf(typeof(ViewPage)));
            var currentPage = args.Content as IViewPage;
            currentPage.RegisterNavigation(this);
        }

        /// <summary>
        /// 弹出最上层页面。
        /// </summary>
        public Task<IViewPage> PopAsync()
        {
            if (InnerFrame.CanGoBack)
            {
                InnerFrame.GoBack();
                return Task.FromResult<IViewPage>(null);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 在导航栈内推入页面。
        /// </summary>
        /// <param name="page">推入栈内的页面</param>
        public Task PushAsync(IViewPage page)
        {
            if (page is ViewDialog dialog)
            {
                return dialog.ShowAsync().AsTask();
            }
            else
            {
                throw new InvalidOperationException();
            }
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
                InnerFrame.Navigate(typeof(PackagedPage), new ValueTuple<Type, object>(pageType, param));
            }

            return Task.CompletedTask;
        }
    }
}
