using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandSchool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IndexPage : ContentPage
	{
        public IndexPage()
		{
            InitializeComponent();
            BindingContext = IndexViewModel.Instance;
        }
	}
}