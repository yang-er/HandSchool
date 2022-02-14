using HandSchool.iOS;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using HandSchool.Models;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewObject), typeof(ViewPageRenderer))]

namespace HandSchool.iOS
{

    public class ViewPageRenderer : PageRenderer
    {
        private UIActivityIndicatorView _spinner;
        private List<MenuEntry> RealMenu { get; set; }
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (_spinner == null)
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
                    NativeView, NSLayoutAttribute.CenterX, (nfloat)1.0, (nfloat)0.0));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    _spinner, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal,
                    NativeView, NSLayoutAttribute.CenterY, (nfloat)1.0, (nfloat)0.0));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    _spinner, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                    (nfloat)1.0, 100));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    _spinner, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                    (nfloat)1.0, 100));
            }

            if (e.OldElement is ViewObject oldElement)
            {
                oldElement.IsBusyChanged -= SetIsBusy;
            }

            if (e.NewElement is ViewObject newElement)
            {
                newElement.IsBusyChanged += SetIsBusy;

                if (newElement.Navigation == null)
                {
                    newElement.RegisterNavigation(new NavigateImpl(newElement));
                }

                RealMenu = new List<MenuEntry>();
                MenuEntry main = null;

                foreach (var entry in newElement.ToolbarMenu)
                {
                    if (entry.HiddenForPull) continue;
                    RealMenu.Add(entry);
                    if (entry.Order == ToolbarItemOrder.Primary)
                        main ??= entry;
                }

                if (main != null && RealMenu.Count == 1)
                {
                    var tbi = new ToolbarItem { BindingContext = main };
                    tbi.SetBinding(MenuItem.TextProperty, "Title", BindingMode.OneWay);
                    tbi.SetBinding(MenuItem.CommandProperty, "Command", BindingMode.OneWay);
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
    }
}