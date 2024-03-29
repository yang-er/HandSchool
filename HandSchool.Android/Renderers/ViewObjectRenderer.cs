﻿using Android.Content;
using Android.Views;
using HandSchool.Droid.Renderers;
using HandSchool.Views;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ViewObject), typeof(ViewObjectRenderer))]
namespace HandSchool.Droid.Renderers
{
    public class ViewObjectRenderer : VisualElementRenderer<Page>
    {
        public ViewObjectRenderer(Context context) : base(context) { }
        
        public override bool OnTouchEvent(MotionEvent e)
        {
            base.OnTouchEvent(e);
            return true;
        }

        private double _previousHeight;
        
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            Page view = e.NewElement;
            base.OnElementChanged(e);
            UpdateBackgroundImage(view);
            Clickable = true;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Page.BackgroundImageSourceProperty.PropertyName)
                UpdateBackgroundImage(Element);
            else if (e.PropertyName == VisualElement.HeightProperty.PropertyName)
                UpdateHeight();
        }

        private void UpdateHeight()
        {
            var newHeight = Element.Height;
            
            if (_previousHeight > 0 && newHeight > _previousHeight)
            {
                var nav = Element.Navigation;
                
                if (nav?.NavigationStack != null && nav.NavigationStack.Count > 0 && Element == nav.NavigationStack[^1])
                {
                    RequestLayout();
                }
            }
            
            _previousHeight = newHeight;
        }

        private void UpdateBackgroundImage(Page view)
        {
            if (view.BackgroundImageSource != null)
            {
                try
                {
                    var im = view.BackgroundImageSource as FileImageSource;
                    this.SetBackground(Context.GetDrawable(im.File));
                }
                catch (Exception error)
                {
                    Core.Logger.WriteLine("error", error.Message);
                }
            }
            else
                this.SetBackground(null);
        }
    }
}