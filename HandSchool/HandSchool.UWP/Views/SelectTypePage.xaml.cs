using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.UWP
{
    public sealed partial class SelectTypePage : Page
    {
        private static int newViewId = 0;
        public Task<ISchoolWrapper> Wrapper { get; }

        public SelectTypePage()
        {
            InitializeComponent();
            MySchool.ItemsSource = Core.Schools;
            Wrapper = new Task<ISchoolWrapper>(() => MySchool.SelectedItem as ISchoolWrapper);
        }

        public static async void FetchAsync()
        {
            Task<ISchoolWrapper> task = null;
            CoreApplicationView newView = CoreApplication.CreateNewView();
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var newWindow = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();
                var frame = new Frame();
                frame.Navigate(typeof(SelectTypePage));
                task = (frame.Content as SelectTypePage).Wrapper;
                newWindow.Content = frame;
                newWindow.Activate();
                newViewId = newAppView.Id;
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            var res = await task;
            await ApplicationViewSwitcher.SwitchAsync(0, newViewId);
            res.PreLoad();
            res.PostLoad();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Wrapper?.Start();
        }
    }
}
