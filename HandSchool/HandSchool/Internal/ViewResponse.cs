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
    }

    public class ViewResponse : IViewResponse
    {
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
    }
}
