using System.Threading.Tasks;
using UIKit;
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

        public void SetIsBusy(bool value, string tips)
        {
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
        }
    }
}
