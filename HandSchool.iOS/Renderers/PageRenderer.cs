#nullable enable
using HandSchool.iOS;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using HandSchool.Models;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewObject), typeof(ViewPageRenderer))]

namespace HandSchool.iOS
{
    public class ViewPageRenderer : PageRenderer
    {
        private UIActivityIndicatorView? _spinner;

        private readonly HashSet<ToolbarItem> _embeddedToolbarItems;

        public ViewPageRenderer()
        {
            _embeddedToolbarItems = new HashSet<ToolbarItem>();
        }

        private void SolveOldObject(ViewObject? viewObject)
        {
            if (viewObject is null) return;
            viewObject.IsBusyChanged -= SetIsBusy;
            _embeddedToolbarItems.ForEach(toolbarItem =>
            {
                toolbarItem.RemoveBinding(MenuItem.TextProperty);
                toolbarItem.RemoveBinding(MenuItem.CommandProperty);
                viewObject.ToolbarItems.Remove(toolbarItem);
            });
            _embeddedToolbarItems.Clear();
        }

        private void SolveNewObject(ViewObject? viewObject)
        {
            if (viewObject is null) return;
            viewObject.IsBusyChanged += SetIsBusy;

            if (viewObject.Navigation == null)
            {
                viewObject.RegisterNavigation(new NavigateImpl(viewObject));
            }

            var menu = viewObject.ToolbarMenu?.Where(entry => !entry.HiddenForPull).ToList() ??
                       new List<MenuEntry>();
            var main = menu.FirstOrDefault(entry => entry.Order == ToolbarItemOrder.Primary);

            if (main is { } && menu.Count == 1)
            {
                var tbi = new ToolbarItem {BindingContext = main};
                tbi.SetBinding(MenuItem.TextProperty, "Title", BindingMode.OneWay);
                tbi.SetBinding(MenuItem.CommandProperty, "Command", BindingMode.OneWay);
                _embeddedToolbarItems.Add(tbi);
                viewObject.ToolbarItems.Add(tbi);
            }
            else
            {
                Core.Logger.WriteLine("PageRenderer", "QAQ");
            }
        }

        private void AddSpinner()
        {
            if (_spinner is { }) return;
            _spinner = new UIActivityIndicatorView
            {
                ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                BackgroundColor = UIColor.Gray.ColorWithAlpha(0.8f)
            };
            NativeView.AddSubview(_spinner);
            _spinner.Layer.CornerRadius = 20;
            _spinner.TranslatesAutoresizingMaskIntoConstraints = false;
            NativeView.AddConstraint(NSLayoutConstraint.Create(
                _spinner, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                NativeView, NSLayoutAttribute.CenterX, (nfloat) 1.0, (nfloat) 0.0));
            NativeView.AddConstraint(NSLayoutConstraint.Create(
                _spinner, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal,
                NativeView, NSLayoutAttribute.CenterY, (nfloat) 1.0, (nfloat) 0.0));
            NativeView.AddConstraint(NSLayoutConstraint.Create(
                _spinner, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                (nfloat) 1.0, 90));
            NativeView.AddConstraint(NSLayoutConstraint.Create(
                _spinner, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                (nfloat) 1.0, 90));
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            SolveOldObject(e.OldElement as ViewObject);
            base.OnElementChanged(e);
            AddSpinner();
            SolveNewObject(e.NewElement as ViewObject);
        }

        private void SetIsBusy(object sender, IsBusyEventArgs isBusy)
        {
            if (Element is ViewObject && isBusy.IsBusy)
            {
                _spinner?.StartAnimating();
            }
            else
            {
                _spinner?.StopAnimating();
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
        }

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            SolveOldObject(Element as ViewObject);
            base.Dispose(disposing);
            _disposed = true;
        }
    }
}