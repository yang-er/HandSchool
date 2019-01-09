using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageTabbedPage : TabbedPage
    {
        private bool FirstOpen = true;

        public MessageTabbedPage()
        {
            InitializeComponent();
        }

        private void TabbedPage_Appearing(object sender, EventArgs e)
        {
            if (!FirstOpen) return;

            FirstOpen = false;
            MessageViewModel.Instance.LoadItemsCommand.Execute(null);
        }
    }
}