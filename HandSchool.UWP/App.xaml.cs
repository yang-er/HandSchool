﻿using HandSchool.Views;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HandSchool.UWP
{
    sealed partial class App : Application
    {
        public App()
        {
            PlatformImpl.Register();
            Forwarder.NormalWay.Begin();

            if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                RequestedTheme = ApplicationTheme.Dark;
            }
            else
            {
                RequestedTheme = ApplicationTheme.Light;
            }

            InitializeComponent();
            Suspending += OnSuspending;
        }
        
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                Xamarin.Forms.Forms.Init(e);

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                
                Window.Current.Content = rootFrame;
            }

            var loaded = Core.Initialize();

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(loaded ? typeof(MainPage) : typeof(SelectTypePage), e.Arguments);
            }
            
            Window.Current.Activate();
            SetupTitleBarStyles();
        }

        private void SetupTitleBarStyles()
        {
            var coreBar = CoreApplication.GetCurrentView().TitleBar;
            coreBar.ExtendViewIntoTitleBar = true;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.Black;
        }
        
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new NotImplementedException("Failed to load Page " + e.SourcePageType.FullName);
        }
        
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
