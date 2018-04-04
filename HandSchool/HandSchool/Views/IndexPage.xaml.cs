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
        String WelcomeStr { get; set; }
        
        public IndexPage()
		{
            HandSchool.JLU.UIMS MessageGeter = new HandSchool.JLU.UIMS();
            var Mes=new JLU.MessageEntrance();
            WelcomeStr = "欢迎,"+Mes.Name;
            this.BindingContext =new StartPageMsg();
            InitializeComponent();
        }
        
	}

}