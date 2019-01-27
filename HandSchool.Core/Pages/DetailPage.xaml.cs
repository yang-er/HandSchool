using HandSchool.Internal;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ViewObject
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
    }
}