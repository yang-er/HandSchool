using CoreGraphics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using XPage = Xamarin.Forms.Page;

namespace HandSchool.Internal
{
    public class ViewResponse : IViewResponse
    {
        private UIActivityIndicatorView Spinner;
        private UIViewController ViewControl;

        public ViewResponse(XPage page)
        {
            Binding = page;
            ViewControl = page.CreateViewController();
        }

        public XPage Binding { get; }

        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Binding.DisplayAlert(title, message, button);
        }

        public void SetIsBusy(bool value, string tips)
        {
            if (value)
            {
                if (Spinner is null)
                {
                    Spinner = new UIActivityIndicatorView(new CGRect(0, 0, 100, 100))
                    {
                        Center = ViewControl.View.Center,
                        ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                        BackgroundColor = UIColor.Gray
                    };

                    Spinner.Layer.CornerRadius = 10;
                    ViewControl.View.AddSubview(Spinner);
                }

                Spinner.StartAnimating();
            }
            else
            {
                Spinner.StopAnimating();
            }

            // UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
        }
    }
}
