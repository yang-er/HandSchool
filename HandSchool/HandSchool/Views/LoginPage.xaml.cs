using HandSchool.Internal;
using HandSchool.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : PopContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = LoginViewModel.Instance;
        }
    }
}
