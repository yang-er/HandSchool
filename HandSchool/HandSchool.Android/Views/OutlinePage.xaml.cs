using HandSchool.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OutlinePage : ContentPage
	{
        public OutlinePage()
		{
			InitializeComponent();
            BindingContext = IndexViewModel.Instance;
            LayoutChanged += WSizeChanged;
        }

        private void WSizeChanged(object sender, EventArgs e)
        {
            infoBar.HeightRequest = Width * 0.625;
            stackOfInfo.Margin = new Thickness(20, Width * 0.625 - 70, 0, 0);
        }
    }
}
