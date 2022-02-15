using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.iOS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutNavMenu
    {
        public event EventHandler<SelectionChangedEventArgs> ItemSelected; 

        public FlyoutNavMenu()
        {
            InitializeComponent();
            BindingContext = PlatformImpl.Instance;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemSelected?.Invoke(sender, e);
        }
    }
}