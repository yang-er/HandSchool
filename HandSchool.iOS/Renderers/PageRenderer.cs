using HandSchool.iOS;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewObject), typeof(ViewPageRenderer))]
namespace HandSchool.iOS
{
    public class ViewPageRenderer : PageRenderer
    {
        private UIActivityIndicatorView Spinner;
        private List<MenuEntry> RealMenu { get; set; }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (Spinner == null)
            {
                Spinner = new UIActivityIndicatorView
                {
                    ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                    BackgroundColor = UIColor.Gray,
                };

                Spinner.Layer.CornerRadius = 10;
                NativeView.AddSubview(Spinner);

                Spinner.TranslatesAutoresizingMaskIntoConstraints = false;
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    Spinner, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                    NativeView, NSLayoutAttribute.CenterX, (nfloat)1.0, (nfloat)0.0));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    Spinner, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal,
                    NativeView, NSLayoutAttribute.CenterY, (nfloat)1.0, (nfloat)0.0));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    Spinner, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                    (nfloat)1.0, (nfloat)100));
                NativeView.AddConstraint(NSLayoutConstraint.Create(
                    Spinner, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                    (nfloat)1.0, (nfloat)100));
            }

            MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);

            if (e.NewElement is ViewObject page)
            {
                MessagingCenter.Subscribe<Page, bool>(this, Page.BusySetSignalName, SetIsBusy, page);

                if (page.Navigation == null)
                {
                    page.RegisterNavigation(new NavigateImpl(page));
                }

                RealMenu = new List<MenuEntry>();
                MenuEntry main = null;

                foreach (var entry in page.ToolbarMenu)
                {
                    if (entry.HiddenForPull) continue;
                    RealMenu.Add(entry);
                    if (entry.Order == ToolbarItemOrder.Primary)
                        main = main ?? entry;
                }

                if (main != null && RealMenu.Count == 1)
                {
                    var tbi = new ToolbarItem { BindingContext = main };
                    tbi.SetBinding(MenuItem.TextProperty, "Title", BindingMode.OneWay);
                    tbi.SetBinding(MenuItem.CommandProperty, "Command", BindingMode.OneWay);
                    page.ToolbarItems.Add(tbi);
                }
                else
                {
                    Core.Logger.WriteLine("PageRenderer", "QAQ");
                }
            }
        }
        
        private void SetIsBusy(Page page, bool isBusy)
        {
            if (Element is ViewObject pg && isBusy)
            {
                Spinner.StartAnimating();
            }
            else
            {
                Spinner.StopAnimating();
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            }
        }
    }
}