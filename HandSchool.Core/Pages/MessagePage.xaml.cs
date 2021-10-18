using System;
using HandSchool.Models;
using HandSchool.ViewModels;
using HandSchool.Internals;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Windows.Input;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePage : ViewObject
    {
        public static MessagePage Instance;
        public MessagePage()
        {
            InitializeComponent();
            ViewModel = MessageViewModel.Instance;
            Instance = this;

            if (Core.Platform.RuntimeName == "Android")
            {
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessageViewModel.Instance.FirstOpen();
        }

        public bool IsPushing { get; set; } = false;

        private void MessageClicked(object sender, EventArgs e)
        {
            ((sender as Frame)?.BindingContext as IMessageItem)?.ItemTappedCommand?.Execute(null);
        }

        private void MessageLongClicked(object sender, EventArgs e)
        {
            ((sender as Frame)?.BindingContext as IMessageItem)?.ItemLongPressCommand?.Execute(null);
        }
    }
}