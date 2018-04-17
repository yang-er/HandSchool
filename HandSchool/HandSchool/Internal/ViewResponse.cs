using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    public interface IViewResponse
    {
        Task ShowMessage(string title, string message, string button = "确认");
        IList<Behavior> Behaviors { get; }
        void SetIsBusy(bool value, string tips = "");
    }

    public class ViewResponse : IViewResponse
    {
        private Action alertCallback;

        public ViewResponse(Page page)
        {
            Binding = page;
            Behaviors = page.Behaviors;
        }

        public Page Binding { get; }

        public IList<Behavior> Behaviors { get; }

        public Task ShowMessage(string title, string message, string button = "确认")
        {
            return Binding.DisplayAlert(title, message, button);
        }

        public void SetIsBusy(bool value, string tips)
        {
            if (value)
            {
                alertCallback = Helper.ShowLoadingAlert(tips);
            }
            else
            {
                alertCallback?.Invoke();
            }
        }
    }
}
