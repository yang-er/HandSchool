using CoreGraphics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XPage = Xamarin.Forms.Page;

namespace HandSchool.Internal
{
    public class ViewResponse : IViewResponse
    {
        // private UIActivityIndicatorView Spinner;
        // private UIViewController ViewControl;

        public ViewResponse(XPage page)
        {
            Binding = page;
            // ViewControl = page.CreateViewController();
        }

        public XPage Binding { get; }

        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Binding.DisplayAlert(title, message, button);
        }

        public void SetIsBusy(bool value, string tips)
        {
            Binding.IsBusy = value;
            // UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
        }
    }
}
