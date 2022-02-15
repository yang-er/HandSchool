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
        private UIActivityIndicatorView _spinner;
        private List<MenuEntry> RealMenu { get; set; }

        private readonly HashSet<ToolbarItem> _embeddedToolbarItems;

        public ViewPageRenderer()
        {
            _embeddedToolbarItems = new HashSet<ToolbarItem>();
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement is ViewObject oldObject)
            {
                oldObject.IsBusyChanged -= SetIsBusy;
                _embeddedToolbarItems.ForEach(toolbarItem =>
                {
                    toolbarItem.RemoveBinding(MenuItem.TextProperty);
                    toolbarItem.RemoveBinding(MenuItem.CommandProperty);
                    oldObject.ToolbarItems.Remove(toolbarItem);
                });
                _embeddedToolbarItems.Clear();
            }

            base.OnElementChanged(e);

            if (_spinner is null)
            {
                _spinner = new UIActivityIndicatorView
                {
                    ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                    BackgroundColor = UIColor.Gray,
                };

                _spinner.Layer.CornerRadius = 10;
                NativeView.AddSubview(_spinner);

                _spinner.TranslatesAutoresizingMaskIntoConstraints = false;
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    _spinner, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                    NativeView, NSLayoutAttribute.CenterX, (nfloat) 1.0, (nfloat) 0.0));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    _spinner, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal,
                    NativeView, NSLayoutAttribute.CenterY, (nfloat) 1.0, (nfloat) 0.0));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    _spinner, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                    (nfloat) 1.0, 100));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    _spinner, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                    (nfloat) 1.0, 100));
            }

            if (e.NewElement is ViewObject newElement)
            {
                newElement.IsBusyChanged += SetIsBusy;

                if (newElement.Navigation == null)
                {
                    newElement.RegisterNavigation(new NavigateImpl(newElement));
                }

                RealMenu = newElement.ToolbarMenu?.Where(entry => !entry.HiddenForPull).ToList() ??
                           new List<MenuEntry>();
                var main = RealMenu.FirstOrDefault(entry => entry.Order == ToolbarItemOrder.Primary);

                if (main is { } && RealMenu.Count == 1)
                {
                    var tbi = new ToolbarItem {BindingContext = main};
                    tbi.SetBinding(MenuItem.TextProperty, "Title", BindingMode.OneWay);
                    tbi.SetBinding(MenuItem.CommandProperty, "Command", BindingMode.OneWay);
                    _embeddedToolbarItems.Add(tbi);
                    newElement.ToolbarItems.Add(tbi);
                }
                else
                {
                    Core.Logger.WriteLine("PageRenderer", "QAQ");
                }
            }
        }

        private void SetIsBusy(object sender, IsBusyEventArgs isBusy)
        {
            if (Element is ViewObject && isBusy.IsBusy)
            {
                _spinner.StartAnimating();
            }
            else
            {
                _spinner.StopAnimating();
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
        }

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (Element is ViewObject element)
            {
                element.IsBusyChanged -= SetIsBusy;
                _embeddedToolbarItems.ForEach(toolbarItem =>
                {
                    toolbarItem.RemoveBinding(MenuItem.TextProperty);
                    toolbarItem.RemoveBinding(MenuItem.CommandProperty);
                    element.ToolbarItems.Remove(toolbarItem);
                });
                _embeddedToolbarItems.Clear();
            }

            base.Dispose(disposing);
        }
    }
}