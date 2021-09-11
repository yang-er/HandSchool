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
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public bool IsPushing { get; set; } = false;
    }
}