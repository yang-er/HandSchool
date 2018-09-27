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
        public ViewResponse(XPage page)
        {
            Binding = page;
        }

        public XPage Binding { get; }

        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Binding.DisplayAlert(title, message, button);
        }

        public Task<bool> ShowActionSheet(string title, string description, string cancel, string accept)
        {
            return Binding.DisplayAlert(title, description, accept, cancel);
        }

        public void SetIsBusy(bool value, string tips)
        {
            Binding.IsBusy = value;
            // UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
        }
    }
}
