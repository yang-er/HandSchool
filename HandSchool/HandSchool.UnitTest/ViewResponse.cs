using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    public class ViewResponse : IViewResponse
    {
        private Action alertCallback;

        public ViewResponse(Page page)
        {
            Binding = page;
        }

        public Page Binding { get; }

        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Binding.DisplayAlert(title, message, button);
        }

        public void SetIsBusy(bool value, string tips)
        {
            MessagingCenter.Send(this, tips, value);
        }
    }
}