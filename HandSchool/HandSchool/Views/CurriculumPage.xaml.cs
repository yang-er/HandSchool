using HandSchool.Models;
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
	public partial class CurriculumPage : PopContentPage
	{
		public CurriculumPage(CurriculumLabel item)
		{
			InitializeComponent();
            BindingContext = item.Context;
            saveButton.Command = new Command(async () =>
            {
                item.Update();
                await Close();
            });
		}
	}
}