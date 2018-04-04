using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FeedPage : ContentPage
    {
        public FeedPage()
        {
            InitializeComponent();
            BindingContext = FeedViewModel.Instance;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            var a = e.Item as IMessageItem;
            //Task.Run(async () => { await App.Current.Message.SetReadState(a.Id, true); a.Unread = false; });

            await (new MessageDetailPage(e.Item as IMessageItem)).ShowAsync(Navigation);

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

    }
}