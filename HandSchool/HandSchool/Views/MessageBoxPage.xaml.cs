using HandSchool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageBoxPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public MessageBoxPage()
        {
            InitializeComponent();
            BindingContext = MessageViewModel.Instance;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("成绩详情", "666", "确定");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
