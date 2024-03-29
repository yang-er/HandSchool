﻿using Android.Content;
using Android.OS;
using Android.Views;
using HandSchool.Internals;
using HandSchool.ViewModels;
using HandSchool.Views;
using System.ComponentModel;
using XForms = Xamarin.Forms.Platform.Android.PageExtensions;

namespace HandSchool.Droid
{
    public class EmbeddedFragment : ViewFragment, INotifyPropertyChanged
    {
        public ViewObject ViewObject { get; private set; }
        public AndroidX.Fragment.App.Fragment Renderer { get; set; }
        public bool SelfControl { get; }

        public override BaseViewModel ViewModel
        {
            get => ViewObject.ViewModel;
            set => ViewObject.ViewModel = value;
        }

        public override string Title
        {
            get => ViewObject.Title;
            set => ViewObject.Title = value;
        }

        public override bool IsBusy
        {
            get => ViewObject.IsBusy;
            set => ViewObject.IsBusy = value;
        }

        public override void SetNavigationArguments(object param)
        {
            ViewObject.SetNavigationArguments(param);
        }

        public override void RegisterNavigation(INavigate navigate)
        {
            base.RegisterNavigation(navigate);
            ViewObject.RegisterNavigation(navigate);
        }

        public override ToolbarMenuTracker ToolbarMenu => ViewObject.ToolbarTracker;
        
        public EmbeddedFragment()
        {
            ViewObject = new ViewObject();
            SelfControl = true;
        }

        public EmbeddedFragment(ViewObject obj, bool selfControl = false)
        {
            ViewObject = obj;
            SelfControl = selfControl;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => ViewObject.PropertyChanged += value;
            remove => ViewObject.PropertyChanged -= value;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Renderer ??= XForms.CreateSupportFragment(ViewObject, Context);
            return Renderer.OnCreateView(inflater, container, savedInstanceState);
        }

        protected override void Dispose(bool disposing)
        {
            Renderer?.Dispose();
            Renderer = null;
            ViewObject.BindingContext = null;
            ViewObject = null;
            base.Dispose(disposing);
        }

        public override void SendAppearing()
        {
            base.SendAppearing();
            if (!SelfControl) ViewObject.SendAppearing();
        }

        public override void SendDisappearing()
        {
            base.SendDisappearing();
            if (!SelfControl) ViewObject.SendDisappearing();
        }
    }
}