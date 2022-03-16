using System.Threading;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    public interface IDetailPage
    {
    }
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ViewObject, IDetailPage
    {
        public DetailPage()
        {
            InitializeComponent();
        }

        public override void SetNavigationArguments(object param)
        {
            if (param is IMessageItem imi)
            {
                param = DetailViewModel.From(imi);
            }
            else if (param is FeedItem fi)
            {
                param = DetailViewModel.From(fi);
            }

            if (param is DetailViewModel vm)
            {
                ViewModel = vm;
                ToolbarMenu.Add(new MenuEntry
                {
                    Order = Xamarin.Forms.ToolbarItemOrder.Primary,
                    Command = vm.Command,
                    Title = vm.Operation,
                    UWPIcon = vm.UWPIcon
                });
            }
            else
            {
                this.WriteLog("No parameters passed.");
            }
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            text.Text = await (ViewModel as DetailViewModel).Content;
        }
    }
}